using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using GenericModConfigMenu;
using IW_ClimateControl;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

// TODO: Add interpolation between probabilities and cache the results <----- v1.0.0
// TODO: Add more than one template <----- ???

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
        private ModConfig _config;
        /// <summary>
        /// The Framework API.
        /// </summary>
        private IIWAPI _iWAPI;
        /// <summary>
        /// The GMCM API.
        /// </summary>
        private IGenericModConfigMenuApi _gMCM;
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
        private SaveData _weatherChanges;
        /// <summary>
        /// Handles all messages to SMAPI.
        /// </summary>
        public static EventLogger s_eventLogger = new();

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
            Monitor.Log("Loaded weather templates.", LogLevel.Trace);

            // -----------
            // CONFIG FILE
            // -----------
            // At launch, SMAPI creates Config, copies values from config.json and updates any empty values,
            // or if config.json is missing, creates a new one using values from Config.
            // 3 paths: New config (just load), saved changes (leave, copy into models), reset (new models, copy into config)
            _config = Helper.ReadConfig<ModConfig>();

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework.
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, load cached save-data.
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_LoadData;

            // ---------
            // DAY START
            // ---------
            // When day begins, set tomorrow's weather.
            Helper.Events.GameLoop.DayStarted += DayStarted_CacheModel;
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
            _iWAPI = Helper.ModRegistry.GetApi<IIWAPI>("MsBontle.ImmersiveWeathers");
            _gMCM = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            GMCMHelper.Register(_config, _gMCM, ModManifest, Helper);
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
                Monitor.Log("Loading save data from file...", LogLevel.Trace);
                _weatherChanges = Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") ?? new SaveData();
            }
        }

        // -------------------
        // CACHE WEATHER MODEL
        // -------------------
        /// <summary>
        /// Loads the necessary weather model when the day starts.
        /// </summary>
        /// <remarks>Contacts the Framework first, to check which models need to be loaded, if any.</remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void DayStarted_CacheModel(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("Checking if weather model needs (re-)caching...", LogLevel.Trace);

            // Check if model needs to be reloaded
            ImmersiveWeathers.MessageContainer shouldUpdateModel = new();
            shouldUpdateModel.Message.MessageType = ImmersiveWeathers.IIWAPI.MessageTypes.saveLoaded;
            shouldUpdateModel.Message.SisterMod = ImmersiveWeathers.IIWAPI.SisterMods.ClimateControl;
            if (_config.ModelChoice == IIWAPI.WeatherModel.custom.ToString())
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IIWAPI.WeatherModel.custom;
            else if (_config.ModelChoice == IIWAPI.WeatherModel.standard.ToString())
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IIWAPI.WeatherModel.standard;
            else
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IIWAPI.WeatherModel.none;
            _iWAPI.ProcessMessage(shouldUpdateModel);

            // Does model need to change
            if (shouldUpdateModel.Response.GoAheadToLoad)
            {
                // If so, which model?
                if (_config.ModelChoice == IIWAPI.WeatherModel.custom.ToString())
                {
                    // Custom model created by player.
                    s_weatherChances = _config;
                    s_modelChoice = IIWAPI.WeatherModel.custom;
                    Monitor.Log("Loading custom model...", LogLevel.Trace);
                    s_weatherArrays = Helper.Data.ReadJsonFile<WeatherArrays>("data/custom.json") ?? Interpolator.InterpolateWeather();
                    Helper.Data.WriteJsonFile("data/custom.json", s_weatherArrays);
                }
                else
                {
                    // Standard model for generic climate.
                    s_weatherChances = s_standardModel;
                    s_modelChoice = IIWAPI.WeatherModel.standard;
                    Monitor.Log("Loading standard model...", LogLevel.Trace);
                    s_weatherArrays = Helper.Data.ReadJsonFile<WeatherArrays>("data/standard.json") ?? Interpolator.InterpolateWeather();
                    Helper.Data.WriteJsonFile("data/standard.json", s_weatherArrays);
                }
            }
            else
            {
                // If not, note this.
                Monitor.Log("No changes made.", LogLevel.Trace);
            }
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
                Monitor.Log("Attempting to change weather...", LogLevel.Trace);

                // Grab relevant info for calculation
                WorldDate currentDate = Game1.Date;
                ImmersiveWeathers.MessageContainer weatherWasChanged = new();
                weatherWasChanged.Message.MessageType = ImmersiveWeathers.IIWAPI.MessageTypes.dayStarted;
                weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IIWAPI.SisterMods.ClimateControl;

                // Attempt to change weather
                WeatherSlotMachine.AttemptChange(currentDate, _weatherChanges, s_weatherChances, _iWAPI);

                // Can weather be changed?
                weatherWasChanged.Message.CouldChange = _weatherChanges.ChangeTomorrow;
                if (weatherWasChanged.Message.CouldChange)
                {
                    // Did any weather types pass the dice roll?
                    if (_weatherChanges.WeatherTomorrow != IIWAPI.WeatherType.sunny)
                    {
                        // Yes. Weather will change to winner.
                        Monitor.Log($"Weather tomorrow changed to {_weatherChanges.WeatherTomorrow}. Updating framework...", LogLevel.Trace);
                    }
                    else if (_weatherChanges.WeatherTomorrow == IIWAPI.WeatherType.sunny)
                    {
                        // No. Weather will remain Sunny.
                        Monitor.Log($"No weather types passed the dice roll for tomorrow. Weather changed to {_weatherChanges.WeatherTomorrow}. Updating framework...", LogLevel.Trace);
                    }

                    // Change tomorrow's weather.
                    Game1.weatherForTomorrow = (int)_weatherChanges.WeatherTomorrow;

                    // Store information for the Framework.
                    weatherWasChanged.Message.WeatherType = (ImmersiveWeathers.IIWAPI.WeatherType)(int)_weatherChanges.WeatherTomorrow;
                }
                else
                {
                    // If not, note this.
                    Monitor.Log($"Weather could not be changed because {_weatherChanges.TomorrowReason} Updating framework...", LogLevel.Trace);
                }

                // Tell the Framework about the change.
                _iWAPI.ProcessMessage(weatherWasChanged);

                // Check message was received by Framework.
                if (!weatherWasChanged.Response.Acknowledged)
                {
                    // Not received. Log an error for SMAPI.
                    Monitor.Log("Error: No acknowledgement received from framework. WHAT DO I DO??", LogLevel.Error);
                }
            }
            else
            {
                Monitor.Log("This is not the main player. Not attempting weather change...", LogLevel.Trace);
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
                Monitor.Log("Saving weather data to file...", LogLevel.Trace);
                Helper.Data.WriteSaveData("ClimateControl-WeatherData", _weatherChanges);
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
            switch (e.Type)
            {
                case EventType.trace:
                    Monitor.Log(e.Message, LogLevel.Trace);
                    break;
                case EventType.debug:
                    Monitor.Log(e.Message, LogLevel.Debug);
                    break;
                case EventType.info:
                    Monitor.Log(e.Message, LogLevel.Info);
                    break;
                case EventType.warn:
                    Monitor.Log(e.Message, LogLevel.Warn);
                    break;
                case EventType.error:
                    Monitor.Log(e.Message, LogLevel.Error);
                    break;
                default:
                    break;
            }
        }
    }
}
