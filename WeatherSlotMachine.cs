using IWClimateControl;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// Calculates weather for tomorrow.
    /// </summary>
    internal class WeatherSlotMachine
    {
        /// <summary>
        /// Attempt weather changes for saved game.
        /// </summary>
        /// <param name="currentDate">The current game date.</param>
        /// <param name="weatherChanges">The relevant object to store results for this save.</param>
        /// <param name="weatherChances">Relevant model data for this save.</param>
        /// <param name="_iWAPI">ImmersiveWeathers API.</param>
        internal static void AttemptChange(WorldDate currentDate, SaveData weatherChanges, ModelDefinition weatherChances, IIWAPI _iWAPI)
        {
            // Check if tomorrow has already been checked.
            if (weatherChanges.TomorrowReason == null)
            {
                // If not, can it change?
                ClimateControl.s_eventLogger.SendToSMAPI("Weather not yet calculated for this save. Calculating tomorrow's weather for the first time...", EventType.trace);
                PerformCheck(currentDate, out bool canChangeTomorrow, out string reasonTomorrow, out IIWAPI.WeatherType defaultTomorrow);
                weatherChanges.ChangeTomorrow = canChangeTomorrow;
                weatherChanges.TomorrowReason = reasonTomorrow;
                if (canChangeTomorrow)
                {
                    // If can change, grab changes.
                    weatherChanges.WeatherTomorrow = GenerateWeather(currentDate, weatherChances, _iWAPI);
                }
                else
                {
                    // If can't change, save default weather.
                    weatherChanges.WeatherTomorrow = defaultTomorrow;
                }
            }
            else
            {
                // Otherwise, grab predicted changes from day after tomorrow (shift day forward).
                weatherChanges.WeatherTomorrow = weatherChanges.WeatherDayAfter;
                weatherChanges.ChangeTomorrow = weatherChanges.ChangeDayAfter;
                weatherChanges.TomorrowReason = weatherChanges.DayAfterReason;
                ClimateControl.s_eventLogger.SendToSMAPI($"Weather already calculated for tomorrow: {weatherChanges.WeatherTomorrow}.", EventType.trace);
            }

            // Now perform checks for day after tomorrow.
            ClimateControl.s_eventLogger.SendToSMAPI("Calculating weather for the day after tomorrow...", EventType.trace);
            WorldDate tomorrowDate = SDate.From(currentDate).AddDays(1).ToWorldDate();
            PerformCheck(tomorrowDate, out bool canChangeDayAfter, out string reasonDayAfter, out IIWAPI.WeatherType defaultDayAfter);
            weatherChanges.ChangeDayAfter = canChangeDayAfter;
            weatherChanges.DayAfterReason = reasonDayAfter;

            // Can day after be changed?
            if (weatherChanges.ChangeDayAfter)
            {
                // If yes, grab changes.
                weatherChanges.WeatherDayAfter = GenerateWeather(tomorrowDate, weatherChances, _iWAPI);
            }
            else
            {
                // If not, grab defaults.
                weatherChanges.WeatherDayAfter = defaultDayAfter;
            }
            ClimateControl.s_eventLogger.SendToSMAPI("Done.", EventType.trace);
        }

        /// <summary>
        /// Performs check if weather can be changed.
        /// </summary>
        /// <param name="thisDate">The date to check.</param>
        /// <param name="canChange">Can the weather be changed?</param>
        /// <param name="reason">If not, why not?</param>
        private static void PerformCheck(WorldDate thisDate, out bool canChange, out string reason, out IIWAPI.WeatherType weatherType)
        {
            // Initialize.
            canChange = true;
            reason = "";
            weatherType = IIWAPI.WeatherType.sunny;

            // Check possibilities
            switch (thisDate.TotalDays)
            {
                case 0:
                case 1:
                case 2:
                    // Too early in game.
                    canChange = false;
                    reason = "the player has played too few days on this save.";
                    // Spring 3
                    if (thisDate.TotalDays == 1)
                        weatherType = IIWAPI.WeatherType.raining;
                    break;
                default:
                    switch (Game1.weatherForTomorrow)
                    {
                        case (int)IIWAPI.WeatherType.festival:
                            // Festival tomorrow.
                            canChange = false;
                            reason = "tomorrow is a festival.";
                            break;
                        case (int)IIWAPI.WeatherType.wedding:
                            // Wedding tomorrow.
                            canChange = false;
                            reason = "tomorrow is your wedding. Congratulations!";
                            break;
                        default:
                            switch (thisDate.DayOfMonth)
                            {
                                case 28:
                                    // First day of a season is always Sunny.
                                    canChange = false;
                                    reason = "tomorrow is the first day of the season and it is always sunny.";
                                    break;
                                default:
                                    switch (Enum.Parse<IIWAPI.SeasonType>(thisDate.Season))
                                    {
                                        case IIWAPI.SeasonType.summer:
                                            if ((thisDate.DayOfMonth + 1) % 13 == 0)
                                            {
                                                // Summer 13 and 26 always storm.
                                                canChange = false;
                                                reason = "tomorrow is a Summer day and is hardcoded to storm.";
                                                weatherType = IIWAPI.WeatherType.storming;
                                            }
                                            break;
                                        case IIWAPI.SeasonType.winter:
                                            if ((thisDate.DayOfMonth + 1) is >= 14 and <= 16)
                                            {
                                                // Winter 14, 15 and 16 are always sunny
                                                canChange = false;
                                                reason = "tomorrow is a Winter day and is hardcoded to be sunny.";
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Generates weather based on config values.
        /// </summary>
        /// <param name="currentDate">Generated by Stardew Valley.</param>
        /// <param name="weatherChances">This model's probability profile.</param>
        /// <param name="api">Framework API.</param>
        internal static IIWAPI.WeatherType GenerateWeather(WorldDate currentDate, ModelDefinition weatherChances, IIWAPI api)
        {
            // Initialize
            Season modelSeason = new();
            IWAPI.SeasonType currentSeason = Enum.Parse<IWAPI.SeasonType>(currentDate.Season);

            // Consider only the relevant season.
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
            return ChooseWeather(modelSeason, api, currentDate);
        }

        /// <summary>
        /// Chooses weather for tomorrow.
        /// </summary>
        /// <param name="modelSeason">The likelihood of each weather type.</param>
        /// <param name="api">Framework API</param>
        /// <param name="weatherJackpot">The successful weather for tomorrow.</param>
        /// <param name="currentDate">Generated by Stardew Valley</param>
        private static IWAPI.WeatherType ChooseWeather(Season modelSeason, IWAPI api, WorldDate currentDate)
        {
            // Default values
            double diceRoll = 1.0;
            double odds;
            IWAPI.WeatherType weatherJackpot = IWAPI.WeatherType.sunny;

            // List of <success,dicerolls,odds> for each weather type.
            List<Tuple<bool, double, double>> weatherRolls = new();

            // Grab the appropriate values based on time of season
            // and flip a coin for each weather type.
            if (currentDate.DayOfMonth%28 is >=0 and <= 8)
            {
                // Tomorrow is day 1-9.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Early, api),
                    FlipCoin(modelSeason.Storm.Early, api),
                    FlipCoin(modelSeason.Wind.Early, api),
                    FlipCoin(modelSeason.Snow.Early, api)
                };
            }
            else if (currentDate.DayOfMonth%28 is >= 9 and <= 18)
            {
                // Tomorrow is day 10-19.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Mid, api),
                    FlipCoin(modelSeason.Storm.Mid, api),
                    FlipCoin(modelSeason.Wind.Mid, api),
                    FlipCoin(modelSeason.Snow.Mid, api)
                };
            }
            else if (currentDate.DayOfMonth%28 is >= 19 and <= 27)
            {
                // Tomorrow is day 20-28.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Late, api),
                    FlipCoin(modelSeason.Storm.Late, api),
                    FlipCoin(modelSeason.Wind.Late, api),
                    FlipCoin(modelSeason.Snow.Late, api)
                };
            }

            // Find the weather with the lowest successful dice-roll.
            for (int i = 0; i < weatherRolls.Count; i++)
            {
                if ((weatherRolls[i].Item1 == true) && weatherRolls[i].Item2 <= diceRoll)
                {
                    diceRoll = weatherRolls[i].Item2;
                    odds = weatherRolls[i].Item3;
                    switch (i)
                    {
                        case 0:
                            weatherJackpot = IWAPI.WeatherType.raining;
                            ClimateControl.eventLogger.SendToSMAPI($"Rain was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}", EventType.info);
                            break;
                        case 1:
                            weatherJackpot = IWAPI.WeatherType.storming;
                            ClimateControl.eventLogger.SendToSMAPI($"Thunderstorm was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}", EventType.info);
                            break;
                        case 2:
                            weatherJackpot = IWAPI.WeatherType.windy;
                            ClimateControl.eventLogger.SendToSMAPI($"Wind was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}", EventType.info);
                            break;
                        case 3:
                            weatherJackpot = IWAPI.WeatherType.snowing;
                            ClimateControl.eventLogger.SendToSMAPI($"Snow was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}", EventType.info);
                            break;
                    }
                }
            }
            return weatherJackpot;
        }

        /// <summary>
        /// Roll a random number to determine if the weather changes.
        /// </summary>
        /// <param name="chance">Likelihood of a change.</param>
        /// <param name="api">Framework API.</param>
        /// <returns>Tuple: The successful weather type and the value of the dice roll.</returns>
        private static Tuple<bool, double, double> FlipCoin(double chance, IWAPI api)
        {
            // If dice roll lands within the percentage, permit the change.
            // Otherwise, deny it. Smaller percentages have narrower
            // windows, corresponding to a lower likelihood of change.
            bool weatherBool = false;
            double diceRoll = api.RollTheDice();
            if ((0.01 * chance) >= diceRoll)
                weatherBool = true;
            return new Tuple<bool, double, double>(weatherBool, diceRoll, chance);
        }
    }
}
