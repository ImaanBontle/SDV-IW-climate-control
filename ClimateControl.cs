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

// TODO: Update TODO from file and move following items to appropriate code

// TODO: Move model check to save load (unset model choice parameters)
// TODO: Broadcast model choice, model updated and weather change to Framework
// TODO: Update API
// TODO: Implement broadcast in Framework and cache values
// TODO: Write custom log messages for weather changes
// TODO: Standardise trace vs info messages. Add traces for every time a mod does an action, framework receives a broadcast, and messages sent to player
// TODO: Add config for SMAPI logging
// TODO: Add mod config menu (make sure to reload any changes from player)
// TODO: Add multiple season values
// TODO: Add interpolation between values and cache

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
        private Config Config;
        // Where to store API
        IWAPI iWAPI;
        // Where to grab models if necessary
        StandardModel standardModel = new();
        // Bool for checking model choice only once
        bool modelChosen = false;
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
            this.Config = this.Helper.ReadConfig<Config>();

            // -----------
            // DATA MODELS
            // -----------
            // At launch, SMAPI repeats the Config process for the weather models
            // Read files
            standardModel = this.Helper.Data.ReadJsonFile<StandardModel>("models/standard.json") ?? new StandardModel();
            // Save files (if needed)
            this.Helper.Data.WriteJsonFile("models/standard.json", standardModel);

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework
            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

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
        }

        // --------------
        // CHANGE WEATHER
        // --------------
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Grab relevant info for calculation
            WorldDate currentDate = Game1.Date;

            // Check if weather is allowed to be changed
            bool canChange = WeatherSlotMachine.CheckCanChange(currentDate);
            if (canChange)
            {
                // If so, attempt to change tomorrow's weather
                IWAPI.WeatherType weatherJackpot;
                // Only load model if necessary
                if (modelChosen == false)
                {
                    // Make sure to use correct model for calculations
                    if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                    {
                        this.Monitor.Log("Model loaded from config", LogLevel.Info);
                        weatherChances = Config.WeatherModel;
                        modelChoice = IWAPI.WeatherModel.custom;
                    }
                    else
                    {
                        this.Monitor.Log("Model loaded from standard", LogLevel.Info);
                        weatherChances = standardModel.Model;
                        modelChoice = IWAPI.WeatherModel.standard;
                    }
                    modelChosen = true;
                }
                weatherJackpot = WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, iWAPI);
                Game1.weatherForTomorrow = (int)weatherJackpot;

                // Tell the framework about the change
                iWAPI.WakeUpNeo_TheyreWatchingYou($"Weather for tomorrow changed to {weatherJackpot}.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
            }
            else
                iWAPI.WakeUpNeo_TheyreWatchingYou($"Weather tomorrow is unchanged.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
        }
    }
}
