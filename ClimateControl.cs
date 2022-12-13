﻿using System;
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

// TODO: Add multiple season values <----- v0.6.0
// TODO: Add mod config menu (make sure to reload any changes from player) <----- v0.7.0
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
            this.Helper.Events.GameLoop.SaveLoaded += SaveLoaded_CacheModel;

            // ---------
            // DAY START
            // ---------
            // When day begins, set tomorrow's weather.
            this.Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;
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
            this.iWAPI = this.Helper.ModRegistry.GetApi<IWAPI>("MsBontle.ImmersiveWeathers");
            this.Monitor.Log("ClimateControl enabled!", LogLevel.Trace);
            this.gMCM = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            this.Monitor.Log("GenericModConfigMenu detected. Registering config options.", LogLevel.Trace);
            GMCMHelper.Register(Config, gMCM, this.ModManifest, this.Helper);



            // Test
            this.Monitor.Log($"After a reload (of entire game): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 67;
            this.Monitor.Log($"After a change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.ResetModel(Config, Helper);
            this.Monitor.Log($"After a hard reset (resetting memory and storage): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 42;
            this.Monitor.Log($"After another change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.LoadModel(Config);
            this.Monitor.Log($"After a soft reset (loading from memory): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 19;
            this.Monitor.Log($"After a third change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.ChangeModel(Config, this.Helper);
            this.Monitor.Log($"After a model refresh (writing change to memory and storage): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 84;
            this.Monitor.Log($"After a fourth change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.LoadModel(Config);
            this.Monitor.Log($"After a soft reset (loading from memory): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 27;
            this.Monitor.Log($"After a fifth change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.ResetModel(Config, Helper);
            this.Monitor.Log($"After a hard reset (resetting memory and storage): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 13;
            this.Monitor.Log($"After a sixth change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.ModelChoice = "custom";
            ModConfig.ChangeModel(Config, Helper);
            this.Monitor.Log($"After a change in model type: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            Config.Spring.Rain.Early = 76;
            this.Monitor.Log($"After a seventh change: {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
            ModConfig.ChangeModel(Config, this.Helper);
            this.Monitor.Log($"After a model refresh (writing change to memory and storage): {Config.Spring.Rain.Early}, {standardModel.Spring.Rain.Early}", LogLevel.Info);
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
        // CHANGE WEATHER
        // --------------
        /// <summary>
        /// Changes tomorrow's weather at the start of each day.
        /// </summary>
        /// <remarks>Weather will not change if tomorrow is a special day(e.g. tomorrow is a festival or a wedding).</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            this.Monitor.Log("Attempting to change weather...", LogLevel.Trace);

            // Grab relevant info for calculation
            WorldDate currentDate = Game1.Date;
            ImmersiveWeathers.MessageContainer weatherWasChanged = new();
            weatherWasChanged.Message.MessageType = ImmersiveWeathers.IWAPI.MessageTypes.dayStarted;
            weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IWAPI.SisterMods.ClimateControl;

            // Check if weather is allowed to be changed
            WeatherSlotMachine.CheckCanChange(currentDate, out bool canChange, out string reason);
            weatherWasChanged.Message.CouldChange = canChange;

            // Can weather be changed?
            if (canChange)
            {
                // If so, roll a die for each weather type.
                IWAPI.WeatherType weatherJackpot = WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, iWAPI);

                // Did any weather types pass the dice roll?
                if (weatherJackpot != IWAPI.WeatherType.sunny)
                {
                    // Yes. Weather will change to winner.
                    this.Monitor.Log($"Weather changed to {weatherJackpot}. Updating framework...", LogLevel.Trace);
                }
                else if (weatherJackpot == IWAPI.WeatherType.sunny)
                {
                    // No. Weather will remain Sunny.
                    this.Monitor.Log($"No weather types passed the dice roll. Weather changed to {weatherJackpot}. Updating framework...", LogLevel.Trace);
                }

                // Change tomorrow's weather.
                Game1.weatherForTomorrow = (int)weatherJackpot;

                // Store information for the Framework.
                weatherWasChanged.Message.WeatherType = (ImmersiveWeathers.IWAPI.WeatherType)(int)weatherJackpot;
            }
            else
            {
                // If not, note this.
                this.Monitor.Log($"Weather could not be changed because {reason} Updating framework...", LogLevel.Trace);
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
