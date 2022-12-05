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
        public static IWAPI.WeatherType GenerateWeather(WorldDate currentDate, ModelDefinition weatherChances, IWAPI api)
        {
            // Initialize
            IWAPI.WeatherType weatherJackpot = new();
            Season modelSeason = new();
            IWAPI.SeasonType currentSeason = Enum.Parse<IWAPI.SeasonType>(currentDate.Season);
            switch (currentSeason)
            {
                case IWAPI.SeasonType.spring:
                    modelSeason = weatherChances.Spring;
                    break;
                case IWAPI.SeasonType.summer:
                    modelSeason = weatherChances.Summer;
                    break;
                case IWAPI.SeasonType.fall:
                    modelSeason = weatherChances.Fall;
                    break;
                case IWAPI.SeasonType.winter:
                    modelSeason = weatherChances.Winter;
                    break;
            }
            // Flip a coin for each state. All at once so custom priorities can be implemented later
            bool stormBool = WeatherCoin.FlipStormCoin(modelSeason.Storm.Mid, api);
            bool rainBool = WeatherCoin.FlipStormCoin(modelSeason.Rain.Mid, api);
            bool windBool = WeatherCoin.FlipStormCoin(modelSeason.Wind.Mid, api);
            bool snowBool = WeatherCoin.FlipStormCoin(modelSeason.Snow.Mid, api);
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

        // Check against list of allowed dates // TODO Check Winter 14, 15, 16
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
