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

// TODO: Broadcast model choice, model updated and weather change to Framework <----- v0.4.0
// TODO: Standardise trace vs info messages. Add traces for every time a mod does an action, framework receives a broadcast, and messages sent to player <----- v0.4.0
// TODO: Change priority of probabilities calculation <----- v0.5.0
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
        private Config Config;
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
            this.Config = this.Helper.ReadConfig<Config>();

            // -----------
            // DATA MODELS
            // -----------
            // At launch, SMAPI repeats the Config process for each of the weather models
            // Read files
            standardModel = this.Helper.Data.ReadJsonFile<StandardModel>("models/standard.json") ?? new StandardModel();
            // Save files (if needed)
            this.Helper.Data.WriteJsonFile("models/standard.json", standardModel);

            // ----------
            // API IMPORT
            // ----------
            // At game ready, SMAPI grabs API from Framework
            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            // -----------
            // SAVE LOADED
            // -----------
            // At save load, cache relevant weather model
            this.Helper.Events.GameLoop.SaveLoaded += SaveLoaded_CacheWeatherModel;

            // -----------------
            // RETURNED TO TITLE
            // -----------------
            // At return to title, uncache the weather model
            this.Helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle_UncacheWeatherModel;

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


        // -------------------
        // CACHE WEATHER MODEL
        // -------------------
        // Cache relevant weather model into model's field
        private void SaveLoaded_CacheWeatherModel(object sender, SaveLoadedEventArgs e)
        {
            // Check if model needs to be reloaded
            ImmersiveWeathers.RedPillOrBluePill shouldUpdateModel = new();
            shouldUpdateModel.MessageFromTrinity.MessageType = ImmersiveWeathers.IWAPI.TypeOfMessage.saveLoaded;
            shouldUpdateModel.MessageFromTrinity.SisterMod = ImmersiveWeathers.IWAPI.FollowTheWhiteRabbit.ClimateControl;
            if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                shouldUpdateModel.MessageFromTrinity.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.custom;
            else if (Config.ModelChoice == IWAPI.WeatherModel.standard.ToString())
                shouldUpdateModel.MessageFromTrinity.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.standard;
            else
                shouldUpdateModel.MessageFromTrinity.ModelType = ImmersiveWeathers.IWAPI.WeatherModel.none;
            this.iWAPI.WakeUpNeo_TheyreWatchingYou(shouldUpdateModel);
            // If so, which model?
            if (shouldUpdateModel.MessageFromNeo.GoAheadToLoad)
            {
                if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
                {
                    weatherChances = Config.WeatherModel;
                    modelChoice = IWAPI.WeatherModel.custom;
                }
                else
                {
                    weatherChances = standardModel.Model;
                    modelChoice = IWAPI.WeatherModel.standard;
                }
            }
        }


        // ---------------------
        // UNCACHE WEATHER MODEL
        // ---------------------
        // Uncache the weather model, in case the player changes it later when GMCM is integrated
        private void ReturnedToTitle_UncacheWeatherModel(object sender, ReturnedToTitleEventArgs e)
        {
            /*
            weatherChances = null;
            modelChoice = IWAPI.WeatherModel.none;
            modelChosen = false;
            */
        }


        // --------------
        // CHANGE WEATHER
        // --------------
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            /*
            // Grab relevant info for calculation
            WorldDate currentDate = Game1.Date;

            // Check if weather is allowed to be changed
            bool canChange = WeatherSlotMachine.CheckCanChange(currentDate);
            if (canChange)
            {
                // If so, attempt to change tomorrow's weather
                IWAPI.WeatherType weatherJackpot;
                weatherJackpot = WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, iWAPI);
                Game1.weatherForTomorrow = (int)weatherJackpot;

                // Tell the framework about the change
                iWAPI.WakeUpNeo_TheyreWatchingYou($"Weather for tomorrow changed to {weatherJackpot}.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
            }
            else
                iWAPI.WakeUpNeo_TheyreWatchingYou($"Weather tomorrow is unchanged.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
            */
        }
    }
}
