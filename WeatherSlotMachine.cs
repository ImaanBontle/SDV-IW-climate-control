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
        public static IWAPI.WeatherType GenerateWeather(WorldDate currentDate, Dictionary<IWAPI.SeasonType, Dictionary<IWAPI.WeatherType, double>> weatherChances, IWAPI api)
        {
            // Initialize
            string currentSeason = currentDate.Season;
            IWAPI.WeatherType weatherJackpot = new();
            // Flip a coin for each state. All at once so custom priorities can be implemented later
            bool stormBool = WeatherCoin.FlipStormCoin(weatherChances[Enum.Parse<IWAPI.SeasonType>(currentSeason)][IWAPI.WeatherType.storming], api);
            bool rainBool = WeatherCoin.FlipRainCoin(weatherChances[Enum.Parse<IWAPI.SeasonType>(currentSeason)][IWAPI.WeatherType.raining], api);
            bool snowBool = WeatherCoin.FlipSnowCoin(weatherChances[Enum.Parse<IWAPI.SeasonType>(currentSeason)][IWAPI.WeatherType.snowing], api);
            bool windBool = WeatherCoin.FlipWindCoin(weatherChances[Enum.Parse<IWAPI.SeasonType>(currentSeason)][IWAPI.WeatherType.windy], api);
            // For now, prioritise storms over rain over wind over snow
            if (stormBool)
                weatherJackpot = IWAPI.WeatherType.storming;
            else if (rainBool)
                weatherJackpot = IWAPI.WeatherType.raining;
            else if (windBool)
                weatherJackpot = IWAPI.WeatherType.windy;
            else if (snowBool)
                weatherJackpot = IWAPI.WeatherType.snowing;
            else
                weatherJackpot = IWAPI.WeatherType.sunny;
            return weatherJackpot;
        }

        // Check against list of allowed dates // TODO: Check Winter 14, 15, 16
        public static bool CheckCanChange(WorldDate currentDate)
        {
            bool canChange = true;
            switch (currentDate.TotalDays)
            {
                case 1:
                case 2:
                case 3:
                    canChange = false;
                    break;
                default:
                    switch (Game1.weatherForTomorrow)
                    {
                        case (int)IWAPI.WeatherType.festival:
                        case (int)IWAPI.WeatherType.wedding:
                            canChange = false;
                            break;
                        default:
                            switch (currentDate.DayOfMonth)
                            {
                                case 28:
                                    canChange = false;
                                    break;
                                default:
                                    switch (Enum.Parse<IWAPI.SeasonType>(currentDate.Season))
                                    {
                                        case IWAPI.SeasonType.summer:
                                            if ((currentDate.DayOfMonth + 1) % 13 == 0)
                                                canChange = false;
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
            return canChange;
        }


    }

    // Flips coins for weather states
    internal class WeatherCoin
    {
        // Thunderstorm
        public static bool FlipStormCoin(double stormChance, IWAPI api)
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
        public static bool FlipRainCoin(double rainChance, IWAPI api)
        {
            bool rainBool = false;
            if ((0.01 * rainChance) >= api.RollTheDice())
                rainBool = true;
            return rainBool;
        }

        // Snow
        public static bool FlipSnowCoin(double snowChance, IWAPI api)
        {
            bool snowBool = false;
            if ((0.01 * snowChance) >= api.RollTheDice())
                snowBool = true;
            return snowBool;
        }

        // Wind
        public static bool FlipWindCoin(double windChance, IWAPI api)
        {
            bool windBool = false;
            if ((0.01 * windChance) >= api.RollTheDice())
                windBool = true;
            return windBool;
        }
    }
}
