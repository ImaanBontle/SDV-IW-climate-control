using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using IW_ClimateControl;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

// TODO: Change priority of probabilities calculation <----- v0.6.0
// TODO: Add multiple season values <----- v0.6.0
// TODO: Add mod config menu (make sure to reload any changes from player) <----- v0.7.0
// TODO: Add interpolation between probabilities and cache the results <----- v1.0.0
// TODO: Add more than one template <----- ???
// TODO: Improve comments <----- ???

namespace IWClimateControl
{
    // ----------
    // MAIN CLASS
    // ----------
    // SMAPI creates instance of this at launch
    internal class ClimateControl : Mod
    {
        // -----------------
        // FIELDS AND VALUES
        // -----------------
        // SMAPI initialises fields at launch
        // Where to grab config
        private ModConfig Config;
        // Where to store API
        IWAPI iWAPI;
        // Where to grab models if necessary
        StandardModel standardModel = new();
        // Where to store current model choice
        IWAPI.WeatherModel modelChoice;
        // Where to store chosen model
        ModelDefinition weatherChances;

        // -----------
        // MAIN METHOD 
        // -----------
        // SMAPI executes this at launch
        public override void Entry(IModHelper helper)
        {
            // -----------
            // CONFIG FILE
            // -----------
            // At launch, SMAPI creates Config, copies values from config.json and updates any empty values,
            // or if config.json is missing, creates a new one using values from Config
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // -----------
            // DATA MODELS
            // -----------
            // At launch, SMAPI repeats the above process for each of the weather models
            // Read files
            standardModel = this.Helper.Data.ReadJsonFile<StandardModel>("models/standard.json") ?? new StandardModel();
            // Save files (if needed)
            this.Helper.Data.WriteJsonFile("models/standard.json", standardModel);
            this.Monitor.Log("Loaded weather templates.", LogLevel.Trace);

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework
            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, cache relevant weather model
            this.Helper.Events.GameLoop.SaveLoaded += SaveLoaded_CacheModel;

            // ---------
            // DAY START
            // ---------
            // When day begins, set tomorrow's weather
            this.Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;
        }


        // ---------
        // GRAB APIS
        // ---------
        // Grab APIs
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.iWAPI = this.Helper.ModRegistry.GetApi<IWAPI>("MsBontle.ImmersiveWeathers");
            this.Monitor.Log("ClimateControl enabled!", LogLevel.Trace);
        }


        // -------------------
        // CACHE WEATHER MODEL
        // -------------------
        // Cache relevant weather model into model's field
        private void SaveLoaded_CacheModel(object sender, SaveLoadedEventArgs e)
        {
            // Check if model needs to be reloaded
            this.Monitor.Log("Checking if weather model needs (re-)caching...", LogLevel.Trace);
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
            // If so, which model?
            if (shouldUpdateModel.Response.GoAheadToLoad)
            {
                if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                {
                    weatherChances = Config.WeatherModel;
                    modelChoice = IWAPI.WeatherModel.custom;
                    this.Monitor.Log("Loading custom model...", LogLevel.Trace);
                }
                else
                {
                    weatherChances = standardModel.Model;
                    modelChoice = IWAPI.WeatherModel.standard;
                    this.Monitor.Log("Loading standard model...", LogLevel.Trace);
                }
            }
            else
            {
                this.Monitor.Log("No changes made.", LogLevel.Trace);
            }
        }


        // --------------
        // CHANGE WEATHER
        // --------------
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Grab relevant info for calculation
            this.Monitor.Log("Attempting to change weather...", LogLevel.Trace);
            WorldDate currentDate = Game1.Date;
            ImmersiveWeathers.MessageContainer weatherWasChanged = new();
            weatherWasChanged.Message.MessageType = ImmersiveWeathers.IWAPI.MessageTypes.dayStarted;
            weatherWasChanged.Message.SisterMod = ImmersiveWeathers.IWAPI.SisterMods.ClimateControl;

            // Check if weather is allowed to be changed
            WeatherSlotMachine.CheckCanChange(currentDate, out bool canChange, out string reason);
            weatherWasChanged.Message.CouldChange = canChange;
            if (canChange)
            {
                // If so, attempt to change tomorrow's weather
                WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, iWAPI, out IWAPI.WeatherType weatherJackpot, out double diceRoll, out double odds);
                if ( weatherJackpot == IWAPI.WeatherType.sunny )
                    this.Monitor.Log($"No weather types passed the dice roll. Weather changed to {weatherJackpot}. Updating framework...", LogLevel.Trace);
                else
                    this.Monitor.Log($"Weather changed to {weatherJackpot}. Successfull dice roll was {diceRoll} against a probability of {0.01 * odds}. Updating framework...", LogLevel.Trace);
                Game1.weatherForTomorrow = (int)weatherJackpot;
                weatherWasChanged.Message.WeatherType = (ImmersiveWeathers.IWAPI.WeatherType)(int)weatherJackpot;
            }
            else
            {
                this.Monitor.Log($"Weather could not be changed because {reason} Updating framework...", LogLevel.Trace);
            }
            // Tell the framework about the change
            this.iWAPI.ProcessMessage(weatherWasChanged);
            if (weatherWasChanged.Response.Acknowledged)
                this.Monitor.Log("Acknowledgement received.", LogLevel.Trace);
            else
            {
                this.Monitor.Log("Error: No acknowledgement received from framework. WHAT DO I DO??", LogLevel.Error);
            }
        }
    }
}
