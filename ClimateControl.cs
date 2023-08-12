using GenericModConfigMenu;
using IW_ClimateControl;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

// TODO: Add more than one template <----- ???
// TODO: Fix bug of incorrect day for calculations
// TODO: Investigate dynamic festival check
// TODO: Investigate accurate TV reporting
// TODO: Separate weather update (flags) and weather odds (dice rolls) so that odds are only transferred at end of day (means mods can check today's data until end of day)
// TODO: Update all flags directly when changing weather (prevents storm-rain bug)
// TODO: Investigate issues with SMAPI logging

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

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework.
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, load data.
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_LoadData;
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_CacheModel;
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_MiscData;

            // ---------
            // DAY START
            // ---------
            // When day begins, interpolate model and set tomorrow's weather.
            Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;

            // -----------
            // GAME SAVING
            // -----------
            // When game saves, update save data object.
            Helper.Events.GameLoop.Saving += Saving_SaveWeather;
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

        // --------------
        // LOAD SAVE DATA
        // --------------
        /// <summary>
        /// Loads the save data for the main player.
        /// </summary>
        /// <remarks>Contains the weather for tomorrow and the day after. Allows consistency on game-load.</remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveLoaded_LoadData(object sender, SaveLoadedEventArgs e)
        {
            // Only perform load if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Load save data
                Monitor.Log("Loading save data from file...", s_logLevel);
                s_weatherChanges = Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") ?? new SaveData();
            }
        }

        // -------------------
        // CACHE WEATHER MODEL
        // -------------------
        /// <summary>
        /// Loads the necessary weather model when the save is loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveLoaded_CacheModel(object sender, SaveLoadedEventArgs e)
        {
            // Only perform load if main player in multiplater.
            if (Context.IsMainPlayer)
            {
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
            }
        }

        // -----------------------
        // GRAB MISCELLANEOUS DATA
        // -----------------------
        /// <summary>
        /// Loads necessary supporting data when the save is loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveLoaded_MiscData(object sender, SaveLoadedEventArgs e)
        {

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

        // --------------
        // CHANGE WEATHER
        // --------------
        /// <summary>
        /// Changes tomorrow's weather at the start of each day.
        /// </summary>
        /// <remarks>Weather will not change if tomorrow is a special day(e.g. tomorrow is a festival or a wedding).</remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Only perform change if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                Monitor.Log("Attempting to change weather...", s_logLevel);

                // Grab relevant info for calculation
                WorldDate currentDate = Game1.Date;
                ImmersiveWeathers.MessageContainer weatherWasChanged = new();
                weatherWasChanged.Message.MessageType = ImmersiveWeathers.IIWAPI.MessageTypes.dayStarted;
                weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IIWAPI.SisterMods.ClimateControl;

                // Attempt to change weather
                WeatherSlotMachine.AttemptChange(currentDate);

                // Can weather be changed?
                weatherWasChanged.Message.CouldChange = s_weatherChanges.ChangeTomorrow;
                if (weatherWasChanged.Message.CouldChange)
                {
                    // Did any weather types pass the dice roll?
                    if (s_weatherChanges.WeatherTomorrow != IIWAPI.WeatherType.sunny)
                    {
                        // Yes. Weather will change to winner.
                        Monitor.Log($"Weather tomorrow changed to {s_weatherChanges.WeatherTomorrow}. Updating framework...", s_logLevel);
                    }
                    else if (s_weatherChanges.WeatherTomorrow == IIWAPI.WeatherType.sunny)
                    {
                        // No. Weather will remain Sunny.
                        Monitor.Log($"No weather types passed the dice roll for tomorrow. Weather changed to {s_weatherChanges.WeatherTomorrow}. Updating framework...", s_logLevel);
                    }

                    // Change tomorrow's weather.
                    Game1.weatherForTomorrow = (int)s_weatherChanges.WeatherTomorrow;

                    // Store information for the Framework.
                    weatherWasChanged.Message.WeatherType = (ImmersiveWeathers.IIWAPI.WeatherType)(int)s_weatherChanges.WeatherTomorrow;
                }
                else
                {
                    // If not, note this.
                    Monitor.Log($"Weather could not be changed because {s_weatherChanges.TomorrowReason} Updating framework...", s_logLevel);
                }

                // Tell the Framework about the change.
                s_iWAPI.ProcessMessage(weatherWasChanged);

                // Check message was received by Framework.
                if (!weatherWasChanged.Response.Acknowledged)
                {
                    // Not received. Log an error for SMAPI.
                    Monitor.Log("Error: No acknowledgement received from framework. WHAT DO I DO??", LogLevel.Error);
                }
            }
            else
            {
                Monitor.Log("This is not the main player. Not attempting weather change...", s_logLevel);
            }
        }

        // --------------
        // SAVE GAME DATA
        // --------------
        /// <summary>
        /// Save the weather changes so they are consistent upon loading.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Saving_SaveWeather(object sender, SavingEventArgs e)
        {
            // Only perform save if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Save data to file.
                Monitor.Log("Saving weather data to file...", s_logLevel);
                Helper.Data.WriteSaveData("ClimateControl-WeatherData", s_weatherChanges);
            }
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
