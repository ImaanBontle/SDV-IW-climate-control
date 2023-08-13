using IWClimateControl;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;

namespace IW_ClimateControl
{
    /// <summary>
    /// Calculates weather for tomorrow.
    /// </summary>
    internal class WeatherSlotMachine
    {
        /// <summary>
        /// Generates weather data for first time on current save.
        /// </summary>
        /// <param name="currentDate">The current game date.</param>
        internal static void GenerateSaveData(WorldDate currentDate)
        {
            // Check that tomorrow's weather can be changed.
            ClimateControl.s_eventLogger.SendToSMAPI("Weather not yet calculated for this save. Calculating tomorrow's weather for the first time...");
            PerformCheck(currentDate, out bool canChangeTomorrow, out string reasonTomorrow, out IIWAPI.WeatherType defaultTomorrow);
            ClimateControl.s_weatherChanges.SetChangeTomorrow(canChangeTomorrow).SetTomorrowReason(reasonTomorrow).SetWeatherTomorrow(defaultTomorrow);
        }

        /// <summary>
        /// Attempt weather changes for saved game.
        /// </summary>
        /// <param name="currentDate">The current game date.</param>
        internal static void AttemptChange(WorldDate currentDate)
        {
            // Check if tomorrow has already been checked.
            if (ClimateControl.s_weatherChanges.TomorrowReason == null)
            {
                // If not, can it change?
                ClimateControl.s_eventLogger.SendToSMAPI("Weather not yet calculated for this save. Calculating tomorrow's weather for the first time...");
                PerformCheck(currentDate, out bool canChangeTomorrow, out string reasonTomorrow, out IIWAPI.WeatherType defaultTomorrow);
                ClimateControl.s_weatherChanges.ChangeTomorrow = canChangeTomorrow;
                ClimateControl.s_weatherChanges.TomorrowReason = reasonTomorrow;
                if (canChangeTomorrow)
                {
                    // If can change, grab changes.
                    if (ClimateControl.s_config.EnableInterpolation)
                    {
                        ClimateControl.s_weatherChanges.WeatherTomorrow = GenerateWeather(currentDate);
                    }
                    else
                    {
                        ClimateControl.s_weatherChanges.WeatherTomorrow = GenerateWeather(currentDate, ClimateControl.s_weatherChances);
                    }
                }
                else
                {
                    // If can't change, save default weather.
                    ClimateControl.s_weatherChanges.WeatherTomorrow = defaultTomorrow;
                }
            }
            else
            {
                // Otherwise, grab predicted changes from day after tomorrow (shift day forward).
                ClimateControl.s_weatherChanges.WeatherTomorrow = ClimateControl.s_weatherChanges.WeatherDayAfter;
                ClimateControl.s_weatherChanges.ChangeTomorrow = ClimateControl.s_weatherChanges.ChangeDayAfter;
                ClimateControl.s_weatherChanges.TomorrowReason = ClimateControl.s_weatherChanges.DayAfterReason;
                ClimateControl.s_eventLogger.SendToSMAPI($"Weather already calculated for tomorrow: {ClimateControl.s_weatherChanges.WeatherTomorrow}.");
            }

            // Now perform checks for day after tomorrow.
            ClimateControl.s_eventLogger.SendToSMAPI("Calculating weather for the day after tomorrow...");
            WorldDate tomorrowDate = SDate.From(currentDate).AddDays(1).ToWorldDate();
            PerformCheck(tomorrowDate, out bool canChangeDayAfter, out string reasonDayAfter, out IIWAPI.WeatherType defaultDayAfter);
            ClimateControl.s_weatherChanges.ChangeDayAfter = canChangeDayAfter;
            ClimateControl.s_weatherChanges.DayAfterReason = reasonDayAfter;

            // Can day after be changed?
            if (ClimateControl.s_weatherChanges.ChangeDayAfter)
            {
                // If yes, grab changes.
                if (ClimateControl.s_config.EnableInterpolation)
                {
                    ClimateControl.s_weatherChanges.WeatherDayAfter = GenerateWeather(tomorrowDate);
                }
                else
                {
                    ClimateControl.s_weatherChanges.WeatherDayAfter = GenerateWeather(tomorrowDate, ClimateControl.s_weatherChances);
                }
            }
            else
            {
                // If not, grab defaults.
                ClimateControl.s_weatherChanges.WeatherDayAfter = defaultDayAfter;
            }
            ClimateControl.s_eventLogger.SendToSMAPI("Done.");
        }

        /// <summary>
        /// Performs check if weather can be changed tomorrow.
        /// </summary>
        /// <param name="thisDate">Today's date.</param>
        /// <param name="canChange">Can tomorrow's weather be changed?</param>
        /// <param name="reason">If not, why not?</param>
        /// <param name="weatherType">If not, what is the default weather?</param>
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
                    // Unique case: Spring 3 is tomorrow
                    if (thisDate.TotalDays == 1)
                        weatherType = IIWAPI.WeatherType.raining;
                    break;
                default:
                    if (ClimateControl.s_festivalDates[thisDate.Season].Contains(thisDate.DayOfMonth + 1))
                    {
                        // Festival tomorrow.
                        canChange = false;
                        reason = "tomorrow is a festival.";
                    }
                    else if (Game1.weatherForTomorrow == (int)IIWAPI.WeatherType.wedding)
                    {
                        // Wedding tomorrow.
                        canChange = false;
                        reason = "tomorrow is your wedding. Congratulations!";
                    }
                    else
                    {
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
        /// <param name="weatherChances">This model's probability parameters.</param>
        /// <param name="api">Framework API.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        public static IIWAPI.WeatherType GenerateWeather(WorldDate currentDate, ModelDefinition weatherChances)
        {
            // Initialize
            Season modelSeason = new();
            IIWAPI.SeasonType currentSeason = Enum.Parse<IIWAPI.SeasonType>(currentDate.Season);

            // Consider only the relevant season.
            switch (currentSeason)
            {
                case IIWAPI.SeasonType.spring:
                    modelSeason = weatherChances.Spring;
                    break;
                case IIWAPI.SeasonType.summer:
                    modelSeason = weatherChances.Summer;
                    break;
                case IIWAPI.SeasonType.fall:
                    modelSeason = weatherChances.Fall;
                    break;
                case IIWAPI.SeasonType.winter:
                    modelSeason = weatherChances.Winter;
                    break;
            }
            return ChooseWeather(modelSeason, currentDate);
        }

        /// <summary>
        /// Generates weather based on interpolated values.
        /// </summary>
        /// <param name="currentDate">Generated by Stardew Valley.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        public static IIWAPI.WeatherType GenerateWeather(WorldDate currentDate)
        {
            // Initialize
            int dayToCheck = 0;
            IIWAPI.SeasonType currentSeason = Enum.Parse<IIWAPI.SeasonType>(currentDate.Season);

            // Consider the relevant season.
            switch (currentSeason)
            {
                case IIWAPI.SeasonType.spring:
                    dayToCheck = currentDate.DayOfMonth;
                    break;
                case IIWAPI.SeasonType.summer:
                    dayToCheck = currentDate.DayOfMonth + 28;
                    break;
                case IIWAPI.SeasonType.fall:
                    dayToCheck = currentDate.DayOfMonth + 2 * 28;
                    break;
                case IIWAPI.SeasonType.winter:
                    dayToCheck = currentDate.DayOfMonth + 3 * 28;
                    break;
            }
            return ChooseWeather(dayToCheck);
        }

        /// <summary>
        /// Chooses weather for tomorrow using config values.
        /// </summary>
        /// <param name="modelSeason">The likelihood of each weather type in this season based on model parameters.</param>
        /// <param name="currentDate">Generated by Stardew Valley</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        private static IIWAPI.WeatherType ChooseWeather(Season modelSeason, WorldDate currentDate)
        {
            // List of <success,dicerolls,odds> for each weather type.
            List<Tuple<bool, double, double>> weatherRolls = new();

            // Grab the appropriate values based on time of season
            // and flip a coin for each weather type.
            if (currentDate.DayOfMonth % 28 is >= 0 and <= 8)
            {
                // Tomorrow is day 1-9.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Early),
                    FlipCoin(modelSeason.Storm.Early),
                    FlipCoin(modelSeason.Wind.Early),
                    FlipCoin(modelSeason.Snow.Early)
                };
            }
            else if (currentDate.DayOfMonth % 28 is >= 9 and <= 18)
            {
                // Tomorrow is day 10-19.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Mid),
                    FlipCoin(modelSeason.Storm.Mid),
                    FlipCoin(modelSeason.Wind.Mid),
                    FlipCoin(modelSeason.Snow.Mid)
                };
            }
            else if (currentDate.DayOfMonth % 28 is >= 19 and <= 27)
            {
                // Tomorrow is day 20-28.
                weatherRolls = new()
                {
                    FlipCoin(modelSeason.Rain.Late),
                    FlipCoin(modelSeason.Storm.Late),
                    FlipCoin(modelSeason.Wind.Late),
                    FlipCoin(modelSeason.Snow.Late)
                };
            }
            return InterpretChoice(weatherRolls);
        }

        /// <summary>
        /// Chooses weather for tomorrow using interpolated values.
        /// </summary>
        /// <param name="currentDate">Date of the year, from 1-112.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        private static IIWAPI.WeatherType ChooseWeather(int currentDate)
        {
            // List of <success,dicerolls,odds> for each weather type.
            List<Tuple<bool, double, double>> weatherRolls;

            // Grab the appropriate values for that day
            // and flip a coin for each weather type.
            weatherRolls = new()
            {
                FlipCoin(ClimateControl.s_weatherArrays.rainArray[currentDate % (28 * 4)]),
                FlipCoin(ClimateControl.s_weatherArrays.stormArray[currentDate % (28 * 4)]),
                FlipCoin(ClimateControl.s_weatherArrays.windArray[currentDate % (28 * 4)]),
                FlipCoin(ClimateControl.s_weatherArrays.snowArray[currentDate % (28 * 4)])
            };
            return InterpretChoice(weatherRolls);
        }

        /// <summary>
        /// Roll a random number to determine if the weather changes.
        /// </summary>
        /// <param name="chance">Likelihood of a change.</param>
        /// <param name="api">Framework API.</param>
        /// <returns>Tuple: The successful weather type and the value of the dice roll.</returns>
        private static Tuple<bool, double, double> FlipCoin(double chance)
        {
            // If dice roll lands within the percentage, permit the change.
            // Otherwise, deny it. Smaller percentages have narrower
            // windows, corresponding to a lower likelihood of change.
            bool weatherBool = false;
            double diceRoll = ClimateControl.s_iWAPI.RollTheDice();
            if ((0.01 * chance) >= diceRoll)
                weatherBool = true;
            return new Tuple<bool, double, double>(weatherBool, diceRoll, chance);
        }

        /// <summary>
        /// Interprets the dice rolls to determine winning weather type.
        /// </summary>
        /// <param name="weatherRolls">List of Tuples containing the outcome, dice roll and odds respectively.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        private static IIWAPI.WeatherType InterpretChoice(List<Tuple<bool, double, double>> weatherRolls)
        {
            // Default values
            double diceRoll = 1.0;
            double odds;
            IIWAPI.WeatherType weatherJackpot = IIWAPI.WeatherType.sunny;

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
                            weatherJackpot = IIWAPI.WeatherType.raining;
                            ClimateControl.s_eventLogger.SendToSMAPI($"Rain was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}");
                            break;
                        case 1:
                            weatherJackpot = IIWAPI.WeatherType.storming;
                            ClimateControl.s_eventLogger.SendToSMAPI($"Thunderstorm was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}");
                            break;
                        case 2:
                            weatherJackpot = IIWAPI.WeatherType.windy;
                            ClimateControl.s_eventLogger.SendToSMAPI($"Wind was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}");
                            break;
                        case 3:
                            weatherJackpot = IIWAPI.WeatherType.snowing;
                            ClimateControl.s_eventLogger.SendToSMAPI($"Snow was successful with a diceroll of {diceRoll} against odds of {0.01 * odds}");
                            break;
                    }
                }
            }
            return weatherJackpot;
        }
    }
}
