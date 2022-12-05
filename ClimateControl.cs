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
    public interface IWAPI
    {
        public enum WeatherType
        {
            sunny = 0,
            raining = 1,
            windy = 2,
            storming = 3,
            festival = 4,
            snowing = 5,
            wedding = 6,
            unknown = 7
        }
        public enum SeasonType
        {
            spring = 0,
            summer = 1,
            fall = 2,
            winter = 3
        }
        Tuple<string, string> GetWeatherInfo();
        string TranslateTomorrowStates(int integerState);
        int TranslateTomorrowStates(string stringState);
        double RollTheDice();
        int RollTheDiceInt();
        public enum FollowTheWhiteRabbit
        {
            ClimateControl = 0
        }
        public void WakeUpNeo_TheyreWatchingYou(string messageForNeo, int thisIsMyName);
    }

    // Main class
    internal sealed class ClimateControl : Mod
    {
        // Where to grab config values
        private CCConfig Config;
        // Where they will be stored internally as fields for the class to access
        public static Dictionary<IWAPI.SeasonType, Dictionary<IWAPI.WeatherType, double>> weatherChances = new();

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
            IWAPI api = this.Helper.ModRegistry.GetApi<IWAPI>("MsBontle.ImmersiveWeathers");

            // Check if weather is allowed to be changed
            bool canChange = WeatherSlotMachine.CheckCanChange(currentDate);
            if (canChange)
            {
                // Attempt to change tomorrow's weather
                IWAPI.WeatherType weatherJackpot = WeatherSlotMachine.GenerateWeather(currentDate, weatherChances, api);
                Game1.weatherForTomorrow = (int)weatherJackpot;

                // Call the framework
                api.WakeUpNeo_TheyreWatchingYou($"Weather for tomorrow changed to {weatherJackpot}.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
            }
            else
                api.WakeUpNeo_TheyreWatchingYou($"Weather tomorrow is unchanged.", (int)IWAPI.FollowTheWhiteRabbit.ClimateControl);
        }
        
        // Grab weather chances from config
        private Dictionary<IWAPI.SeasonType, Dictionary<IWAPI.WeatherType, double>> GrabWeatherChances()
        {
            return new Dictionary<IWAPI.SeasonType, Dictionary<IWAPI.WeatherType, double>>
            {
                {
                    IWAPI.SeasonType.spring,
                    new Dictionary<IWAPI.WeatherType, double>
                    {
                        { IWAPI.WeatherType.raining, this.Config.SpringRainChance },
                        { IWAPI.WeatherType.storming, this.Config.SpringStormChance },
                        { IWAPI.WeatherType.windy, this.Config.SpringWindChance },
                        { IWAPI.WeatherType.snowing, this.Config.SpringSnowChance }
                    }
                },
                {
                    IWAPI.SeasonType.summer,
                    new Dictionary<IWAPI.WeatherType, double>
                    {
                        { IWAPI.WeatherType.raining, this.Config.SummerRainChance },
                        { IWAPI.WeatherType.storming, this.Config.SummerStormChance },
                        { IWAPI.WeatherType.windy, this.Config.SummerWindChance },
                        { IWAPI.WeatherType.snowing, this.Config.SummerSnowChance }
                    }
                },
                {
                    IWAPI.SeasonType.fall,
                    new Dictionary<IWAPI.WeatherType, double>
                    {
                        { IWAPI.WeatherType.raining, this.Config.FallRainChance },
                        { IWAPI.WeatherType.storming, this.Config.FallStormChance },
                        { IWAPI.WeatherType.windy, this.Config.FallWindChance },
                        { IWAPI.WeatherType.snowing, this.Config.FallSnowChance }
                    }
                },
                {
                    IWAPI.SeasonType.winter,
                    new Dictionary<IWAPI.WeatherType, double>
                    {
                        { IWAPI.WeatherType.raining, this.Config.WinterRainChance },
                        { IWAPI.WeatherType.storming, this.Config.WinterStormChance },
                        { IWAPI.WeatherType.windy, this.Config.WinterWindChance },
                        { IWAPI.WeatherType.snowing, this.Config.WinterSnowChance }
                    }
                }
            };
        }
    }
}
