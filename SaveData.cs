using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IWAPI.WeatherType WeatherTomorrow { get; set; } = IWAPI.WeatherType.sunny;
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
        public IWAPI.WeatherType WeatherDayAfter { get; set; } = IWAPI.WeatherType.sunny;
    }
}
