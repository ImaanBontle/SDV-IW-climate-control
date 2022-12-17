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
        private ModConfig Config;
        /// <summary>
        /// The Framework API.
        /// </summary>
        private IWAPI iWAPI;
        /// <summary>
        /// The GMCM API.
        /// </summary>
        private IGenericModConfigMenuApi gMCM;
        /// <summary>
        /// Cache of the standard model configuration data.
        /// </summary>
        internal static StandardModel standardModel;
        /// <summary>
        /// Cache of the custom model configuration data.
        /// </summary>
        internal static StandardModel customModel;
        /// <summary>
        /// The chosen model for this session.
        /// </summary>
        internal static IWAPI.WeatherModel modelChoice;
        /// <summary>
        /// Contains model probability data for this session.
        /// </summary>
        private ModelDefinition weatherChances;
        /// <summary>
        /// Handles all messages to SMAPI.
        /// </summary>
        public static EventLogger eventLogger = new();

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
            eventLogger.Send += AlertSMAPI;

            // -----------
            // DATA MODELS
            // -----------
            // At launch, SMAPI repeats the below process for each of the weather models.
            // Read files
            standardModel = this.Helper.Data.ReadJsonFile<StandardModel>("models/standard.json") ?? new StandardModel();
            customModel = this.Helper.Data.ReadJsonFile<StandardModel>("models/custom.json") ?? new StandardModel();
            // Save files (if needed)
            this.Helper.Data.WriteJsonFile("models/standard.json", standardModel);
            this.Helper.Data.WriteJsonFile("models/custom.json", customModel);
            this.Monitor.Log("Loaded weather templates.", LogLevel.Trace);

            // -----------
            // CONFIG FILE
            // -----------
            // At launch, SMAPI creates Config, copies values from config.json and updates any empty values,
            // or if config.json is missing, creates a new one using values from Config.
            // 3 paths: New config (just load), saved changes (leave, copy into models), reset (new models, copy into config)
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework.
            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, cache relevant weather model.
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_CacheModel;
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded_LoadData;

            // ---------
            // DAY START
            // ---------
            // When day begins, set tomorrow's weather.
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


        // -------------------
        // CACHE WEATHER MODEL
        // -------------------
        /// <summary>
        /// Loads the necessary weather model when the save is loaded.
        /// </summary>
        /// <remarks>Contacts the Framework first, to check which models need to be loaded, if any.</remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveLoaded_CacheModel(object sender, SaveLoadedEventArgs e)
        {
            this.Monitor.Log("Checking if weather model needs (re-)caching...", LogLevel.Trace);

            // Check if model needs to be reloaded
            ImmersiveWeathers.MessageContainer shouldUpdateModel = new();
            shouldUpdateModel.Message.MessageType = ImmersiveWeathers.IWAPI.MessageTypes.saveLoaded;
            shouldUpdateModel.Message.SisterMod = ImmersiveWeathers.IWAPI.SisterMods.ClimateControl;
            if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.custom;
            else if (Config.ModelChoice == IWAPI.WeatherModel.standard.ToString())
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.standard;
            else
                shouldUpdateModel.Message.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.none;
            this.iWAPI.ProcessMessage(shouldUpdateModel);

            // Does model need to change
            if (shouldUpdateModel.Response.GoAheadToLoad)
            {
                // If so, which model?
                if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                {
                    // Custom model created by player.
                    weatherChances = Config;
                    modelChoice = IWAPI.WeatherModel.custom;
                    this.Monitor.Log("Loading custom model...", LogLevel.Trace);
                }
                else
                {
                    // Standard model for generic climate.
                    weatherChances = standardModel;
                    modelChoice = IWAPI.WeatherModel.standard;
                    this.Monitor.Log("Loading standard model...", LogLevel.Trace);
                }
            }
            else
            {
                // If not, note this.
                this.Monitor.Log("No changes made.", LogLevel.Trace);
            }
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
        private void SaveLoaded_LoadData(object sender, EventArgs e)
        {
            // Only perform load if main player in multiplayer.
            if (Context.IsMainPlayer)
            {
                // Load save data
                Monitor.Log("Loading save data from file...", LogLevel.Trace);
                _weatherChanges = Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") ?? new SaveData();
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
            this.Monitor.Log("Attempting to change weather...", LogLevel.Trace);

            // Grab relevant info for calculation
            WorldDate currentDate = Game1.Date;
            ImmersiveWeathers.MessageContainer weatherWasChanged = new();
            weatherWasChanged.Message.MessageType = ImmersiveWeathers.IWAPI.MessageTypes.dayStarted;
            weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IWAPI.SisterMods.ClimateControl;

            // Attempt to change weather
            SaveData weatherChanges = this.Helper.Data.ReadSaveData<SaveData>("ClimateControl-WeatherData") ?? new SaveData();
            WeatherSlotMachine.AttemptChange(currentDate, weatherChanges, weatherChances, iWAPI);

            // Can weather be changed?
            weatherWasChanged.Message.CouldChange = _weatherChanges.ChangeTomorrow;
            if (weatherWasChanged.Message.CouldChange)
            {
                Monitor.Log("Attempting to change weather...", LogLevel.Trace);

                // Grab relevant info for calculation
                WorldDate currentDate = Game1.Date;
                ImmersiveWeathers.MessageContainer weatherWasChanged = new();
                weatherWasChanged.Message.MessageType = ImmersiveWeathers.IWAPI.MessageTypes.dayStarted;
                weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IWAPI.SisterMods.ClimateControl;

                // Attempt to change weather
                WeatherSlotMachine.AttemptChange(currentDate, _weatherChanges, _weatherChances, _iWAPI);

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
                    weatherWasChanged.Message.WeatherType = (ImmersiveWeathers.IWAPI.WeatherType)(int)_weatherChanges.WeatherTomorrow;
                }
                else if (_weatherChanges.WeatherTomorrow == IIWAPI.WeatherType.sunny)
                {
                    // No. Weather will remain Sunny.
                    Monitor.Log($"No weather types passed the dice roll. Weather changed to {_weatherChanges.WeatherTomorrow}. Updating framework...", LogLevel.Trace);
                }

                // Change tomorrow's weather.
                Game1.weatherForTomorrow = (int)_weatherChanges.WeatherTomorrow;

                // Check message was received by Framework.
                if (!weatherWasChanged.Response.Acknowledged)
                {
                    // Not received. Log an error for SMAPI.
                    Monitor.Log("Error: No acknowledgement received from framework. WHAT DO I DO??", LogLevel.Error);
                }
            }
            else
            {
                // If not, note this.
                Monitor.Log($"Weather could not be changed because {_weatherChanges.TomorrowReason} Updating framework...", LogLevel.Trace);
            }

            // Tell the Framework about the change.
            this.iWAPI.ProcessMessage(weatherWasChanged);

            // Check message was received by Framework.
            if (weatherWasChanged.Response.Acknowledged)
                // Received.
                this.Monitor.Log("Acknowledgement received.", LogLevel.Trace);
            else
            {
                // Not received. Log an error for SMAPI.
                this.Monitor.Log("Error: No acknowledgement received from framework. WHAT DO I DO??", LogLevel.Error);
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
        private void Saving_SaveWeather(object sender, EventArgs e)
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
                    this.Monitor.Log(e.Message, LogLevel.Trace);
                    break;
                case EventType.debug:
                    this.Monitor.Log(e.Message, LogLevel.Debug);
                    break;
                case EventType.info:
                    this.Monitor.Log(e.Message, LogLevel.Info);
                    break;
                case EventType.warn:
                    this.Monitor.Log(e.Message, LogLevel.Warn);
                    break;
                case EventType.error:
                    this.Monitor.Log(e.Message, LogLevel.Error);
                    break;
                default:
                    break;
            }
        }
    }
}
