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
    }
}
