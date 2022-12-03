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
    // Interface for IWAPI
    public interface IIWAPI
    {
        Tuple<string, string> GetWeatherInfo();
        string TranslateTomorrowStates(int integerState);
        int TranslateTomorrowStates(string stringState);
    }

    internal sealed class ClimateControl : Mod
    {
        // Where to grab config values
        private CCConfig Config;
        // Where they will be stored internally as fields for the class to access
        public static Dictionary<string, Dictionary<string, double>> weatherChances = new();

        // Main method
        public override void Entry(IModHelper helper)
        {
            // Tell SMAPI to grab config values and/or make config.json if absent
            this.Config = this.Helper.ReadConfig<CCConfig>();
            // Populate class config fields
            weatherChances = GrabWeatherChances();

            // When day begins, set tomorrow's weather
            this.Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;

        }
        
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Grab weather chances for calculations
            this.Monitor.Log($"The Spring Rain chance is {weatherChances["Spring"]["Rain"]}", LogLevel.Info);

            // Grab from IWAPI
            IIWAPI api = this.Helper.ModRegistry.GetApi<IIWAPI>("MsBontle.ImmersiveWeathers");
            if (api != null)
            {
                //Tuple<string, string> weatherInfo = api.GetWeatherInfo();
                //int tomorrowInteger = api.TranslateTomorrowStates(weatherInfo.Item2);
                //string tomorrowString = api.TranslateTomorrowStates(tomorrowInteger);
                //this.Monitor.Log($"Tomorrow's weather is {weatherInfo.Item2}, which corresponds to an integer of {tomorrowInteger} and a string of {tomorrowString}", LogLevel.Info);
                //this.Monitor.Log($"Today's day is {Game1.Date.DayOfWeek}", LogLevel.Info);

                //string currentDay = Game1.Date.DayOfWeek.ToString();
                
                /*
                if (currentDay == "Monday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Sunny");
                }
                else if (currentDay == "Tuesday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Raining");
                }
                else if (currentDay == "Wednesday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Snowing");
                }
                else if (currentDay == "Thursday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Windy");
                }
                else if (currentDay == "Friday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Wedding");
                }
                else if (currentDay == "Saturday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Festival");
                }
                else if (currentDay == "Sunday")
                {
                    Game1.weatherForTomorrow = api.TranslateTomorrowStates("Storming");
                }
                */
            }
            //Game1.weatherForTomorrow = 1;
            
        }
        
        // Grab weather chances from config
        private Dictionary<string, Dictionary<string, double>> GrabWeatherChances()
        {
            return new Dictionary<string, Dictionary<string, double>>
            {
                {
                    "Spring",
                    new Dictionary<string, double>
                    {
                        { "Rain", this.Config.SpringRainChance },
                        { "Storm", this.Config.SpringStormChance },
                        { "Wind", this.Config.SpringWindChance },
                        { "Snow", this.Config.SpringSnowChance }
                    }
                },
                {
                    "Summer",
                    new Dictionary<string, double>
                    {
                        { "Rain", this.Config.SummerRainChance },
                        { "Storm", this.Config.SummerStormChance },
                        { "Wind", this.Config.SummerWindChance },
                        { "Snow", this.Config.SummerSnowChance }
                    }
                },
                {
                    "Fall",
                    new Dictionary<string, double>
                    {
                        { "Rain", this.Config.FallRainChance },
                        { "Storm", this.Config.FallStormChance },
                        { "Wind", this.Config.FallWindChance },
                        { "Snow", this.Config.FallSnowChance }
                    }
                },
                {
                    "Winter",
                    new Dictionary<string, double>
                    {
                        { "Rain", this.Config.WinterRainChance },
                        { "Storm", this.Config.WinterStormChance },
                        { "Wind", this.Config.WinterWindChance },
                        { "Snow", this.Config.WinterSnowChance }
                    }
                }
            };
        }
    }
}
