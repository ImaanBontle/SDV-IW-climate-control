using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
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
        // Main method
        public override void Entry(IModHelper helper)
        {
            // When day begins, set tomorrow's weather
            this.Helper.Events.GameLoop.DayStarted += DayStarted_ChangeWeather;

        }
        
        // Change tomorrow's weather
        private void DayStarted_ChangeWeather(object sender, DayStartedEventArgs e)
        {
            // Grab from IWAPI
            IIWAPI api = this.Helper.ModRegistry.GetApi<IIWAPI>("MsBontle.ImmersiveWeathers");
            if (api != null)
            {
                //Tuple<string, string> weatherInfo = api.GetWeatherInfo();
                //int tomorrowInteger = api.TranslateTomorrowStates(weatherInfo.Item2);
                //string tomorrowString = api.TranslateTomorrowStates(tomorrowInteger);
                //this.Monitor.Log($"Tomorrow's weather is {weatherInfo.Item2}, which corresponds to an integer of {tomorrowInteger} and a string of {tomorrowString}", LogLevel.Info);
                //this.Monitor.Log($"Today's day is {Game1.Date.DayOfWeek}", LogLevel.Info);

                string currentDay = Game1.Date.DayOfWeek.ToString();
                this.Monitor.Log($"Warning! Today is {currentDay}. Expect strange weather tomorrow...", LogLevel.Info);
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
            }
            //Game1.weatherForTomorrow = 1;
            
        }
    }
}
