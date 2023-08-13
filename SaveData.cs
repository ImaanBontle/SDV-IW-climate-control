namespace IW_ClimateControl
{
    /// <summary>
    /// Stores relevant weather predictions to save file.
    /// </summary>
    internal class SaveData
    {
        /// <summary>
        /// Could the weather be changed tomorrow?
        /// </summary>
        public bool ChangeTomorrow { get; set; } = false;
        /// <summary>
        /// The reason the weather could not be changed tomorrow.
        /// </summary>
        public string TomorrowReason { get; set; } = null;
        /// <summary>
        /// What should the weather be tomorrow? Defaults to sunny.
        /// </summary>
        public IIWAPI.WeatherType WeatherTomorrow { get; set; } = IIWAPI.WeatherType.sunny;
        /// <summary>
        /// Could the weather be changed the day after tomorrow?
        /// </summary>
        public bool ChangeDayAfter { get; set; } = false;
        /// <summary>
        /// The reason the weather could not be changed the day after tomorrow.
        /// </summary>
        public string DayAfterReason { get; set; } = null;
        /// <summary>
        /// What should the weather be the day after tomorrow? Defaults to sunny.
        /// </summary>
        public IIWAPI.WeatherType WeatherDayAfter { get; set; } = IIWAPI.WeatherType.sunny;

        /// <summary>
        /// Sets <see cref="ChangeTomorrow"/> to <paramref name="changeTomorrow"/>.
        /// </summary>
        /// <param name="changeTomorrow">See <see cref="ChangeTomorrow"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetChangeTomorrow(bool changeTomorrow)
        {
            ChangeTomorrow = changeTomorrow;
            return this;
        }
        /// <summary>
        /// Sets <see cref="TomorrowReason"/> to <paramref name="tomorrowReason"/>.
        /// </summary>
        /// <param name="tomorrowReason">See <see cref="TomorrowReason"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetTomorrowReason(string tomorrowReason)
        {
            TomorrowReason = tomorrowReason;
            return this;
        }
        /// <summary>
        /// Sets <see cref="WeatherTomorrow"/> to <paramref name="weatherTomorrow"/>.
        /// </summary>
        /// <param name="weatherTomorrow">See <see cref="WeatherTomorrow"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetWeatherTomorrow(IIWAPI.WeatherType weatherTomorrow)
        {
            WeatherTomorrow = weatherTomorrow;
            return this;
        }
        /// <summary>
        /// Sets <see cref="ChangeDayAfter"/> to <paramref name="changeDayAfter"/>.
        /// </summary>
        /// <param name="changeDayAfter">See <see cref="ChangeDayAfter"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetChangeDayAfter(bool changeDayAfter)
        {
            ChangeDayAfter = changeDayAfter;
            return this;
        }
        /// <summary>
        /// Sets <see cref="DayAfterReason"/> to <paramref name="dayAfterReason"/>.
        /// </summary>
        /// <param name="dayAfterReason">See <see cref="DayAfterReason"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetDayAfterReason(string dayAfterReason)
        {
            DayAfterReason = dayAfterReason;
            return this;
        }
        /// <summary>
        /// Sets <see cref="WeatherDayAfter"/> to <paramref name="weatherDayAfter"/>.
        /// </summary>
        /// <param name="weatherDayAfter">See <see cref="WeatherDayAfter"/>.</param>
        /// <returns><see cref="SaveData"/></returns>
        public SaveData SetWeatherDayAfter(IIWAPI.WeatherType weatherDayAfter)
        {
            WeatherDayAfter = weatherDayAfter;
            return this;
        }
    }
}
