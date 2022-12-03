using IWClimateControl;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    // Handles weather outcomes
    internal class WeatherSlotMachine
    {
        // Generate weather based on config values
        public static string GenerateWeather(WorldDate currentDate, Dictionary<string, Dictionary<string, double>> weatherChances, IIWAPI api)
        {
            string weatherJackpot = "";
            string currentSeason = currentDate.Season;
            // Flip a coin for each state. All at once so custom priorities can be implemented later
            bool stormBool = WeatherCoin.FlipStormCoin(weatherChances[currentSeason]["storm"], api);
            bool rainBool = WeatherCoin.FlipRainCoin(weatherChances[currentSeason]["rain"], api);
            bool snowBool = WeatherCoin.FlipSnowCoin(weatherChances[currentSeason]["snow"], api);
            bool windBool = WeatherCoin.FlipWindCoin(weatherChances[currentSeason]["wind"], api);
            // For now, prioritise storms over rain over wind over snow
            if (stormBool)
                weatherJackpot = "Storming";
            else if (rainBool)
                weatherJackpot = "Raining";
            else if (windBool)
                weatherJackpot = "Windy";
            else if (snowBool)
                weatherJackpot = "Snowing";
            else
                weatherJackpot = "Sunny";
            return weatherJackpot;
        }

    }

    // Flips coins for weather states
    internal class WeatherCoin
    {
        // Thunderstorm
        public static bool FlipStormCoin(double stormChance, IIWAPI api)
        {
            // If dice roll lands within the percentage, permit the change.
            // Otherwise, deny it. Smaller percentages have narrower
            // windows, corresponding to a lower likelihood of change.
            bool stormBool = false;
            if ((0.01 * stormChance) >= api.RollTheDice())
                stormBool = true;
            return stormBool;
        }

        // Rain
        public static bool FlipRainCoin(double rainChance, IIWAPI api)
        {
            bool rainBool = false;
            if ((0.01 * rainChance) >= api.RollTheDice())
                rainBool = true;
            return rainBool;
        }

        // Snow
        public static bool FlipSnowCoin(double snowChance, IIWAPI api)
        {
            bool snowBool = false;
            if ((0.01 * snowChance) >= api.RollTheDice())
                snowBool = true;
            return snowBool;
        }

        // Wind
        public static bool FlipWindCoin(double windChance, IIWAPI api)
        {
            bool windBool = false;
            if ((0.01 * windChance) >= api.RollTheDice())
                windBool = true;
            return windBool;
        }
    }
}
