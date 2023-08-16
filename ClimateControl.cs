using GenericModConfigMenu;
using IW_ClimateControl;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// TODO: Add more than one template <----- ???
// TODO: Fix bug of incorrect day for calculations (apparently one day too late???)
// TODO: Investigate accurate TV reporting
// TODO: Separate weather update (flags) and weather odds (dice rolls) so that odds are only transferred at end of day (means mods can check today's data until end of day)
// TODO: Update all flags directly when changing weather (prevents storm-rain bug)
// TODO: Finish updating weather calculations based on recent changes.

namespace IWClimateControl
{
    // ----------
    // MAIN CLASS
    // ----------
    /// <summary>
    /// The mod's entry point. SMAPI creates an instance of this at launch.
    /// </summary>
    internal class ClimateControl : Mod
    {
        // -----------------
        // FIELDS AND VALUES
        // -----------------
        // SMAPI initialises fields at launch
        /// <summary>
        /// The configuration data for this session.
        /// </summary>
        internal static ModConfig s_config;
        /// <summary>
        /// The Framework API.
        /// </summary>
        internal static IIWAPI s_iWAPI;
        /// <summary>
        /// The GMCM API.
        /// </summary>
        internal static IGenericModConfigMenuApi s_gMCM;
        /// <summary>
        /// Cache of the standard model configuration data.
        /// </summary>
        internal static StandardModel s_standardModel;
        /// <summary>
        /// Cache of the custom model configuration data.
        /// </summary>
        internal static StandardModel s_customModel;
        /// <summary>
        /// The chosen model for this session.
        /// </summary>
        internal static IIWAPI.WeatherModel s_modelChoice;
        /// <summary>
        /// Contains model probability data for this session.
        /// </summary>
        internal static ModelDefinition s_weatherChances;
        /// <summary>
        /// Contains daily interpolated probabilities for this model.
        /// </summary>
        internal static WeatherArrays s_weatherArrays = new();
        /// <summary>
        /// Contains relevant weather changes for this save game.
        /// </summary>
        internal static SaveData s_weatherChanges;
        /// <summary>
        /// Contains list of festival dates for this game sorted by season.
        /// </summary>
        internal static Dictionary<string, List<int>> s_festivalDates = new();
        /// <summary>
        /// Handles all messages to SMAPI.
        /// </summary>
        internal static EventLogger s_eventLogger = new();
        /// <summary>
        /// Determines where standard log messages go.
        /// </summary>
        internal static LogLevel s_logLevel;

        // -----------
        // MAIN METHOD 
        // -----------
        /// <summary>
        /// The mod's entry method, called after SMAPI first loads the mod.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // --------------
            // SMAPI MESSAGES
            // --------------
            // Listen for messages to send to SMAPI.
            s_eventLogger.Send += AlertSMAPI;

            // -----------
            // DATA MODELS
            // -----------
            // At launch, SMAPI repeats the below process for each of the weather models.
            // Read files
            s_standardModel = Helper.Data.ReadJsonFile<StandardModel>("models/standard.json") ?? new StandardModel();
            s_customModel = Helper.Data.ReadJsonFile<StandardModel>("models/custom.json") ?? new StandardModel();
            // Save files (if needed)
            Helper.Data.WriteJsonFile("models/standard.json", s_standardModel);
            Helper.Data.WriteJsonFile("models/custom.json", s_customModel);
            Monitor.Log("Loaded weather templates.", s_logLevel);

            // -----------
            // CONFIG FILE
            // -----------
            // At launch, SMAPI creates Config, copies values from config.json and updates any empty values,
            // or if config.json is missing, creates a new one using values from Config.
            // 3 paths: New config (just load), saved changes (leave, copy into models), reset (new models, copy into config)
            s_config = Helper.ReadConfig<ModConfig>();

            // Set SMAPI log levels
            s_logLevel = s_config.EnableDebugLogging ? LogLevel.Info : LogLevel.Trace;

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework.
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // GRAB ASSETS
            // -----------
            // When assets ready, check against existing data and update relevant fields.
            Helper.Events.Content.AssetReady += Content_AssetReady;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, load data.
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_LoadData;

            // ---------
            // DAY START
            // ---------
            // When day begins, interpolate model and set tomorrow's weather.
            Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;

            //
            //
            //
            // Before the day ends, prepare for tomorrow.
            Helper.Events.GameLoop.DayEnding += DayEnding_PrepareForTomorrow;

            // -----------
            // GAME SAVING
            // -----------
            // When game saves, update save data object.
            Helper.Events.GameLoop.Saving += Saving_SaveData;
        }

        // ---------
        // GRAB APIS
        // ---------
        /// <summary>
        /// Grabs external APIs at game launch.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            s_iWAPI = Helper.ModRegistry.GetApi<IIWAPI>("MsBontle.ImmersiveWeathers");
            s_gMCM = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            GMCMHelper.Register(ModManifest, Helper);
        }

        // -----------------------
        // GRAB MISCELLANEOUS DATA
        // -----------------------
        /// <summary>
        /// Grabs supporting data from game files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Content_AssetReady(object sender, AssetReadyEventArgs e)
        {
            // Only perform check if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Check festival dates.
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/FestivalDates"))
                {
                    Monitor.Log("Adding festival data...", LogLevel.Trace);
                    // Split festival data into season and date.
                    Regex festivalPattern = new(@"([a-zA-Z]+)(\d+)");
                    foreach (string festival in Helper.GameContent.Load<Dictionary<string, string>>("Data/Festivals/FestivalDates").Keys)
                    {
                        Match festivalData = festivalPattern.Match(festival);
                        if (festivalData.Success)
                        {
                            string festivalName = festivalData.Groups[1].Value;
                            int festivalDate = int.Parse(festivalData.Groups[2].Value);
                            // Store list of dates per season.
                            if (!s_festivalDates.ContainsKey(festivalName))
                            {
                                s_festivalDates[festivalName] = new List<int>() { festivalDate };
                            }
                            else if (!s_festivalDates[festivalName].Contains(festivalDate))
                            {
                                s_festivalDates[festivalName].Add(festivalDate);
                            }
                        }
                    }
                }
            }
        }

        // --------------
        // LOAD SAVE DATA
        // --------------
        /// <summary>
        /// Loads the necessary weather data when the save is loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveLoaded_LoadData(object sender, SaveLoadedEventArgs e)
        {
            // Only perform load if main player in multiplater.
            if (Context.IsMainPlayer)
            {
                // Cache relevant weather models
                if (s_config.ModelChoice == IIWAPI.WeatherModel.custom.ToString())
                {
                    // Custom model created by player.
                    Monitor.Log("Loading custom model...", s_logLevel);
                    s_weatherChances = s_config;
                    s_modelChoice = IIWAPI.WeatherModel.custom;
                    if (s_config.EnableInterpolation)
                    {
                        s_weatherArrays = Interpolator.InterpolateWeather();
                        Helper.Data.WriteJsonFile("data/custom.json", s_weatherArrays);
                    }
                }
                else
                {
                    // Standard model for generic climate.
                    Monitor.Log("Loading standard model...", s_logLevel);
                    s_weatherChances = s_standardModel;
                    s_modelChoice = IIWAPI.WeatherModel.standard;
                    if (s_config.EnableInterpolation)
                    {
                        s_weatherArrays = Interpolator.InterpolateWeather();
                        Helper.Data.WriteJsonFile("data/standard.json", s_weatherArrays);
                    }
                }

                // Load save data
                Monitor.Log("Loading save data from file...", s_logLevel);
                s_weatherChanges = Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") ?? new SaveData();

                // If weather has not been predicted thus far, generate tomorrow's weather prediction.
                if (Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") == null)
                {
                    Monitor.Log("Weather not yet calculated for this save. Calculating tomorrow's weather for the first time...", s_logLevel);
                    WeatherSlotMachine.GenerateTomorrowChanges(SDate.From(Game1.Date));
                    Monitor.Log("Done.", s_logLevel);
                }
            }
        }

        // --------------
        // CHANGE WEATHER
        // --------------
        /// <summary>
        /// Changes tomorrow's weather at the start of each day.
        /// </summary>
        /// <remarks>Weather will not change if tomorrow is a special day (e.g. tomorrow is a festival or a wedding).</remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Only perform change if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Attempt to change weather
                Monitor.Log("Attempting to change weather...", s_logLevel);
                WeatherSlotMachine.AttemptTomorrowChange();

                // Tell the Framework about the change.
                ImmersiveWeathers.MessageContainer weatherWasChanged = new();
                weatherWasChanged.Message.MessageType = ImmersiveWeathers.IIWAPI.MessageTypes.dayStarted;
                weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IIWAPI.SisterMods.ClimateControl;
                weatherWasChanged.Message.CouldChange = s_weatherChanges.ChangeTomorrow;
                s_iWAPI.ProcessMessage(weatherWasChanged);

                // Check message was received by Framework.
                if (!weatherWasChanged.Response.Acknowledged)
                {
                    // Not received. Log an error for SMAPI.
                    Monitor.Log("Error: No acknowledgement received from framework. This shouldn't be possible! WHAT DO I DO??", LogLevel.Error);
                }
            }
            else
            {
                Monitor.Log("This is not the main player. Not attempting weather change...", s_logLevel);
            }
        }

        // --------------------
        // PREPARE FOR TOMORROW
        // --------------------
        /// <summary>
        /// Perform end-of-day calculations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayEnding_PrepareForTomorrow(object sender, DayEndingEventArgs e)
        {
            // Only perform calculations if main player in multiplayer.
            if (Context.IsMainPlayer && SDate.From(Game1.Date).Day > 0)
            {
                // Calculate weather for day after tomorrow (referred to as 'tomorrow' in-game to avoid confusion).
                Monitor.Log($"Calculating weather changes for tomorrow...", s_logLevel);
                WeatherSlotMachine.GenerateTomorrowChanges(SDate.From(Game1.Date).AddDays(1));
            }
        }

        // ---------
        // SAVE DATA
        // ---------
        /// <summary>
        /// Save the weather changes so they are consistent upon reloading.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Saving_SaveData(object sender, SavingEventArgs e)
        {
            // Only perform save if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Save data to file.
                Monitor.Log("Saving weather data to file...", s_logLevel);
                Helper.Data.WriteSaveData("ClimateControl-WeatherData", s_weatherChanges);
            }
        }

        // -----------------
        // INTERPOLATE MODEL
        // -----------------
        /// <summary>
        /// Interpolates the probabilities onto a daily grid.
        /// </summary>
        internal static void InterpolateModel(IModHelper Helper)
        {
            s_eventLogger.SendToSMAPI("Model needs to be re-interpolated. Interpolating...");
            if (s_config.ModelChoice == IIWAPI.WeatherModel.custom.ToString())
            {
                s_weatherChances = s_config;
                s_weatherArrays = Interpolator.InterpolateWeather();
                Helper.Data.WriteJsonFile("data/custom.json", s_weatherArrays);
            }
            else if (s_config.ModelChoice == IIWAPI.WeatherModel.standard.ToString())
            {
                s_weatherChances = s_standardModel;
                s_weatherArrays = Interpolator.InterpolateWeather();
                Helper.Data.WriteJsonFile("data/standard.json", s_weatherArrays);
            }
            s_eventLogger.SendToSMAPI("Done.");
        }

        // --------------------
        // HANDLE INTERNAL LOGS
        // --------------------
        /// <summary>
        /// Records messages to the SMAPI's log and terminal.
        /// </summary>
        /// <param name="e">The message to send to the log/terminal.</param>
        internal void AlertSMAPI(object sender, EventMessage e)
        {
            Monitor.Log(e.Message, s_logLevel);
        }
    }
}
