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
        public static void GenerateWeather(WorldDate currentDate, ModelDefinition weatherChances, IWAPI api, out IWAPI.WeatherType weatherJackpot, out double diceRoll, out double odds)
        {
            // Initialize
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
            WeatherCoin.FlipCoin(modelSeason.Storm.Mid, api, out bool stormBool, out double stormRoll);
            WeatherCoin.FlipCoin(modelSeason.Rain.Mid, api, out bool rainBool, out double rainRoll);
            WeatherCoin.FlipCoin(modelSeason.Wind.Mid, api, out bool windBool, out double windRoll);
            WeatherCoin.FlipCoin(modelSeason.Snow.Mid, api, out bool snowBool, out double snowRoll);
            // For now, prioritise storms over rain over wind over snow
            if (stormBool)
            {
                weatherJackpot = IWAPI.WeatherType.storming;
                diceRoll = stormRoll;
                odds = modelSeason.Storm.Mid;
            }
            else if (rainBool)
            {
                weatherJackpot = IWAPI.WeatherType.raining;
                diceRoll = rainRoll;
                odds = modelSeason.Rain.Mid;
            }
            else if (windBool)
            {
                weatherJackpot = IWAPI.WeatherType.windy;
                diceRoll = windRoll;
                odds = modelSeason.Wind.Mid;
            }
            else if (snowBool)
            {
                weatherJackpot = IWAPI.WeatherType.snowing;
                diceRoll = snowRoll;
                odds = modelSeason.Snow.Mid;
            }
            else
            {
                weatherJackpot = IWAPI.WeatherType.sunny;
                diceRoll = 0.0;
                odds = 0.0;
            }
        }

        // Check against list of allowed dates
        public static void CheckCanChange(WorldDate currentDate, out bool canChange, out string reason)
        {
            canChange = true;
            reason = "";
            switch (currentDate.TotalDays)
            {
                case 1:
                case 2:
                case 3:
                    canChange = false;
                    reason = "the player has played too few days on this save.";
                    break;
                default:
                    switch (Game1.weatherForTomorrow)
                    {
                        case (int)IWAPI.WeatherType.festival:
                            canChange = false;
                            reason = "tomorrow is a festival.";
                            break;
                        case (int)IWAPI.WeatherType.wedding:
                            canChange = false;
                            reason = "tomorrow is your wedding. Congratulations!";
                            break;
                        default:
                            switch (currentDate.DayOfMonth)
                            {
                                case 28:
                                    canChange = false;
                                    reason = "tomorrow is the first day of the season and it is always sunny.";
                                    break;
                                default:
                                    switch (Enum.Parse<IWAPI.SeasonType>(currentDate.Season))
                                    {
                                        case IWAPI.SeasonType.summer:
                                            if ((currentDate.DayOfMonth + 1) % 13 == 0)
                                            {
                                                canChange = false;
                                                reason = "tomorrow is a Summer day and is hardcoded to storm.";
                                            }
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
        }


    }

    // Flips coins for weather states
    internal class WeatherCoin
    {
        public static void FlipCoin(double chance, IWAPI api, out bool weatherBool, out double diceRoll)
        {
            // If dice roll lands within the percentage, permit the change.
            // Otherwise, deny it. Smaller percentages have narrower
            // windows, corresponding to a lower likelihood of change.
            weatherBool = false;
            diceRoll = api.RollTheDice();
            if ((0.01 * chance) >= diceRoll)
                weatherBool = true;
        }
    }
}
