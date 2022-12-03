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

namespace IWClimateControl
{
    // Internal interface for IWAPI
    public interface IIWAPI
    {
        Tuple<string, string> GetWeatherInfo();
        string TranslateTomorrowStates(int integerState);
        int TranslateTomorrowStates(string stringState);
        double RollTheDice();
        int RollTheDiceInt();
    }

    // Main class
    internal sealed class ClimateControl : Mod
    {
        // Where to grab config values
        private CCConfig Config;
        // Where they will be stored internally as fields for the class to access
        public static Dictionary<string, Dictionary<string, double>> weatherChances = new();

        // Main method
        public override void Entry(IModHelper helper)
        {
            // -------------
            // LAUNCH CONFIG
            // -------------
            // At launch, tell SMAPI where to grab config values and to make config.json if absent
            this.Config = this.Helper.ReadConfig<CCConfig>();
            // Populate field with config's weather chances for calculations
            weatherChances = GrabWeatherChances();

            // ---------
            // DAY START
            // ---------
            // When day begins, set tomorrow's weather
            this.Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;

        }

        // --------------
        // CHANGE WEATHER
        // --------------
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Grab relevant info for calculation
            var currentDate = Game1.Date;
            IIWAPI api = this.Helper.ModRegistry.GetApi<IIWAPI>("MsBontle.ImmersiveWeathers");

            // Check if tomorrow is OK to change
            if ((currentDate.DayOfMonth != 28) &&
                (Game1.weatherForTomorrow != api.TranslateTomorrowStates("Festival")) &&
                (Game1.weatherForTomorrow != api.TranslateTomorrowStates("Wedding")))
            {
                // If so, attempt to change it
                string weatherJackpot = WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, api);
                this.Monitor.Log($"Weather for tomorrow changed to {weatherJackpot}", LogLevel.Info);
                Game1.weatherForTomorrow = api.TranslateTomorrowStates(weatherJackpot);
            }
            else
            {
                // If not, don't touch it
                this.Monitor.Log("Tomorrow will always be sunny, so no changes will be made.", LogLevel.Info);
            }  
        }
        
        // Grab weather chances from config
        private Dictionary<string, Dictionary<string, double>> GrabWeatherChances()
        {
            return new Dictionary<string, Dictionary<string, double>>
            {
                {
                    "spring",
                    new Dictionary<string, double>
                    {
                        { "rain", this.Config.SpringRainChance },
                        { "storm", this.Config.SpringStormChance },
                        { "wind", this.Config.SpringWindChance },
                        { "snow", this.Config.SpringSnowChance }
                    }
                },
                {
                    "summer",
                    new Dictionary<string, double>
                    {
                        { "rain", this.Config.SummerRainChance },
                        { "storm", this.Config.SummerStormChance },
                        { "wind", this.Config.SummerWindChance },
                        { "snow", this.Config.SummerSnowChance }
                    }
                },
                {
                    "fall",
                    new Dictionary<string, double>
                    {
                        { "rain", this.Config.FallRainChance },
                        { "storm", this.Config.FallStormChance },
                        { "wind", this.Config.FallWindChance },
                        { "snow", this.Config.FallSnowChance }
                    }
                },
                {
                    "winter",
                    new Dictionary<string, double>
                    {
                        { "rain", this.Config.WinterRainChance },
                        { "storm", this.Config.WinterStormChance },
                        { "wind", this.Config.WinterWindChance },
                        { "snow", this.Config.WinterSnowChance }
                    }
                }
            };
        }
    }
}
