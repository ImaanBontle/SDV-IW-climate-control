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
        /// Generates weather data for the day after <paramref name="dateToConsider"/>.
        /// </summary>
        /// <param name="dateToConsider">The date to consider (NB: changes apply to the following day).</param>
        internal static void GenerateTomorrowChanges(SDate dateToConsider)
        {
            // Check that tomorrow's weather can be changed.
            PerformCheck(dateToConsider, out bool canChangeTomorrow, out string reasonTomorrow, out IIWAPI.WeatherType defaultTomorrow);
            ClimateControl.s_weatherChanges.SetChangeTomorrow(canChangeTomorrow).SetTomorrowReason(reasonTomorrow).SetWeatherTomorrow(defaultTomorrow);

            // Generate appropriate weather based on this information.
            if (canChangeTomorrow)
            {
                ClimateControl.s_weatherChanges.SetWeatherTomorrow(GenerateWeather(dateToConsider));
            }
        }

        /// <summary>
        /// Attempt weather changes for tomorrow.
        /// </summary>
        internal static void AttemptTomorrowChange()
        {
            // Debug logging.
            string message;
            // Can weather be changed?
            if (ClimateControl.s_weatherChanges.ChangeTomorrow)
            {
                // If yes, did any weather types pass the dice roll?
                if (ClimateControl.s_weatherChanges.WeatherTomorrow != IIWAPI.WeatherType.sunny)
                {
                    // Yes. Weather will change to reflect the winner.
                    message = $"Weather tomorrow changed to {ClimateControl.s_weatherChanges.WeatherTomorrow}. Updating framework...";
                }
                else
                {
                    // No. Weather will remain Sunny.
                    message = $"No weather types passed the dice roll for tomorrow. Weather changed to {ClimateControl.s_weatherChanges.WeatherTomorrow}. Updating framework...";
                }
                // Perform actual change in weather.
                Game1.weatherForTomorrow = (int)ClimateControl.s_weatherChanges.WeatherTomorrow;
            }
            else
            {
                // If not, make note of this.
                message = $"Weather could not be changed because {ClimateControl.s_weatherChanges.TomorrowReason} Updating framework...";
            }
            // Send debug log.
            ClimateControl.s_eventLogger.SendToSMAPI(message);
        }

        /// <summary>
        /// Checks if the weather can be changed for the day after <paramref name="dateToConsider"/>.
        /// </summary>
        /// <param name="dateToConsider">The date to consider.</param>
        /// <param name="canChange">Can tomorrow's weather be changed?</param>
        /// <param name="reason">If not, why not?</param>
        /// <param name="weatherType">If not, what is the default weather?</param>
        private static void PerformCheck(SDate dateToConsider, out bool canChange, out string reason, out IIWAPI.WeatherType weatherType)
        {
            // Initialize.
            canChange = true;
            reason = "";
            weatherType = IIWAPI.WeatherType.sunny;

            // Check possibilities
            switch (dateToConsider.DaysSinceStart)
            {
                case 1:
                case 2:
                case 3:
                    // Too early in game.
                    canChange = false;
                    reason = "the player has played too few days on this save.";
                    // Unique case: Spring 3 is tomorrow
                    if (dateToConsider.DaysSinceStart == 2)
                        weatherType = IIWAPI.WeatherType.raining;
                    break;
                default:
                    if (ClimateControl.s_festivalDates[dateToConsider.Season].Contains(dateToConsider.Day + 1))
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
                        switch (dateToConsider.Day)
                        {
                            case 28:
                                // First day of a season is always Sunny.
                                canChange = false;
                                reason = "tomorrow is the first day of the season and it is always sunny.";
                                break;
                            default:
                                switch (Enum.Parse<IIWAPI.SeasonType>(dateToConsider.Season))
                                {
                                    case IIWAPI.SeasonType.summer:
                                        if ((dateToConsider.Day + 1) % 13 == 0)
                                        {
                                            // Summer 13 and 26 always storm.
                                            canChange = false;
                                            reason = "tomorrow is a Summer day and is hardcoded to storm.";
                                            weatherType = IIWAPI.WeatherType.storming;
                                        }
                                        break;
                                    case IIWAPI.SeasonType.winter:
                                        if ((dateToConsider.Day + 1) is >= 14 and <= 16)
                                        {
                                            // Winter 14, 15 and 16 are always sunny (
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
        /// Generates weather for day after <paramref name="dateToConsider"/>, based on config values.
        /// </summary>
        /// <param name="dateToConsider">Date to consider.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        public static IIWAPI.WeatherType GenerateWeather(SDate dateToConsider)
        {
            // Print layout reminder to terminal.
            ClimateControl.s_eventLogger.SendToSMAPI("OUTCOME: ROLL vs. ODDS (WEATHER-TYPE)");
            // Determine whether to use interpolation or fixed values.
            if (ClimateControl.s_config.EnableInterpolation)
            {
                // Initialize
                int dayToCheck = 0;
                IIWAPI.SeasonType currentSeason = Enum.Parse<IIWAPI.SeasonType>(dateToConsider.Season);

                // Consider the relevant season.
                switch (currentSeason)
                {
                    case IIWAPI.SeasonType.spring:
                        dayToCheck = dateToConsider.Day;
                        break;
                    case IIWAPI.SeasonType.summer:
                        dayToCheck = dateToConsider.Day + 28;
                        break;
                    case IIWAPI.SeasonType.fall:
                        dayToCheck = dateToConsider.Day + 2 * 28;
                        break;
                    case IIWAPI.SeasonType.winter:
                        dayToCheck = dateToConsider.Day + 3 * 28;
                        break;
                }

                // List of <success,dicerolls,odds> for each weather type.
                List<Tuple<bool, double>> weatherRolls;

                // Grab the appropriate values for that day
                // and flip a coin for each weather type.
                weatherRolls = new()
                {
                    FlipCoin(ClimateControl.s_weatherArrays.rainArray[dayToCheck % (28 * 4)], "Rain"),
                    FlipCoin(ClimateControl.s_weatherArrays.stormArray[dayToCheck % (28 * 4)], "Thunderstorm"),
                    FlipCoin(ClimateControl.s_weatherArrays.windArray[dayToCheck % (28 * 4)], "Wind"),
                    FlipCoin(ClimateControl.s_weatherArrays.snowArray[dayToCheck % (28 * 4)], "Snow")
                };
                return InterpretChoice(weatherRolls);
            }
            else
            {
                // Initialize
                Season modelSeason = new();
                IIWAPI.SeasonType currentSeason = Enum.Parse<IIWAPI.SeasonType>(dateToConsider.Season);

                // Consider only the relevant season.
                switch (currentSeason)
                {
                    case IIWAPI.SeasonType.spring:
                        modelSeason = ClimateControl.s_weatherChances.Spring;
                        break;
                    case IIWAPI.SeasonType.summer:
                        modelSeason = ClimateControl.s_weatherChances.Summer;
                        break;
                    case IIWAPI.SeasonType.fall:
                        modelSeason = ClimateControl.s_weatherChances.Fall;
                        break;
                    case IIWAPI.SeasonType.winter:
                        modelSeason = ClimateControl.s_weatherChances.Winter;
                        break;
                }

                // List of <success,dicerolls,odds> for each weather type.
                List<Tuple<bool, double>> weatherRolls = new();

                // Grab the appropriate values based on time of season
                // and flip a coin for each weather type.
                if (dateToConsider.Day % 28 is >= 0 and <= 8)
                {
                    // Tomorrow is day 1-9.
                    weatherRolls = new()
                    {
                        FlipCoin(modelSeason.Rain.Early, "Rain"),
                        FlipCoin(modelSeason.Storm.Early, "Thunderstorm"),
                        FlipCoin(modelSeason.Wind.Early, "Wind"),
                        FlipCoin(modelSeason.Snow.Early, "Snow")
                    };
                }
                else if (dateToConsider.Day % 28 is >= 9 and <= 18)
                {
                    // Tomorrow is day 10-19.
                    weatherRolls = new()
                    {
                        FlipCoin(modelSeason.Rain.Mid, "Rain"),
                        FlipCoin(modelSeason.Storm.Mid, "Thunderstorm"),
                        FlipCoin(modelSeason.Wind.Mid, "Wind"),
                        FlipCoin(modelSeason.Snow.Mid, "Snow")
                    };
                }
                else if (dateToConsider.Day % 28 is >= 19 and <= 27)
                {
                    // Tomorrow is day 20-28.
                    weatherRolls = new()
                    {
                        FlipCoin(modelSeason.Rain.Late, "Rain"),
                        FlipCoin(modelSeason.Storm.Late, "Thunderstorm"),
                        FlipCoin(modelSeason.Wind.Late, "Wind"),
                        FlipCoin(modelSeason.Snow.Late, "Snow")
                    };
                }
                return InterpretChoice(weatherRolls);
            }
        }

        /// <summary>
        /// Roll a random number to determine if the weather changes.
        /// </summary>
        /// <param name="chance">Likelihood of a change.</param>
        /// <param name="api">Framework API.</param>
        /// <returns>Tuple: The successful weather type and the value of the dice roll.</returns>
        private static Tuple<bool, double> FlipCoin(double chance, string weatherForRoll)
        {
            // If dice roll lands within the percentage, permit the change.
            // Otherwise, deny it. Smaller percentages have narrower
            // windows, corresponding to a lower likelihood of change.
            bool weatherBool = false;
            double diceRoll = ClimateControl.s_iWAPI.RollTheDice();
            if ((0.01 * chance) >= diceRoll)
                weatherBool = true;

            // Print all dicerolls to terminal.
            ClimateControl.s_eventLogger.SendToSMAPI($"{(weatherBool ? "Success" : "Failure")}: {Math.Round(diceRoll * 100, 2)} vs. {Math.Round(chance, 2)} ({weatherForRoll}).");

            return new Tuple<bool, double>(weatherBool, diceRoll);
        }

        /// <summary>
        /// Interprets the dice rolls to determine winning weather type.
        /// </summary>
        /// <param name="weatherRolls">List of Tuples containing the outcome, dice roll and odds respectively.</param>
        /// <returns>
        /// <see cref="IIWAPI.WeatherType"/>: the generated weather.
        /// </returns>
        private static IIWAPI.WeatherType InterpretChoice(List<Tuple<bool, double>> weatherRolls)
        {
            // Default values
            double diceRoll = 1.0;
            IIWAPI.WeatherType weatherJackpot = IIWAPI.WeatherType.sunny;

            // Find the weather with the lowest successful dice-roll.
            for (int i = 0; i < weatherRolls.Count; i++)
            {
                if ((weatherRolls[i].Item1 == true) && weatherRolls[i].Item2 <= diceRoll)
                {
                    diceRoll = weatherRolls[i].Item2;
                    switch (i)
                    {
                        case 0:
                            weatherJackpot = IIWAPI.WeatherType.raining;
                            break;
                        case 1:
                            weatherJackpot = IIWAPI.WeatherType.storming;
                            break;
                        case 2:
                            weatherJackpot = IIWAPI.WeatherType.windy;
                            break;
                        case 3:
                            weatherJackpot = IIWAPI.WeatherType.snowing;
                            break;
                    }
                }
            }
            return weatherJackpot;
        }
    }
}
