using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// The standard model for generic climates.
    /// </summary>
    /// <remarks>Spring has many gentle showers. Summer has intense but brief thunderstorms. Fall is dry and windy. Winter has lots of snowfall and some rain. Both Spring and Fall have a very small chance of snow.</remarks>
    public class StandardModel : ModelDefinition
    {
        public StandardModel()
        {
            // Spring:
            // Very wet but light intensity, vanishing chance of snow
            Spring.Rain.Early = 15;
            Spring.Storm.Early = 2;
            Spring.Wind.Early = 30;
            Spring.Snow.Early = 5;

            Spring.Rain.Mid = 40;
            Spring.Storm.Mid = 5;
            Spring.Wind.Mid = 20;
            Spring.Snow.Mid = 0;

            Spring.Rain.Late = 30;
            Spring.Storm.Late = 15;
            Spring.Wind.Late = 10;
            Spring.Snow.Late = 0;

            // Summer:
            // Mostly sunny, but can be intense rain
            Summer.Rain.Early = 15;
            Summer.Storm.Early = 30;
            Summer.Wind.Early = 5;
            Summer.Snow.Early = 0;

            Summer.Rain.Mid = 12;
            Summer.Storm.Mid = 20;
            Summer.Wind.Mid = 5;
            Summer.Snow.Mid = 0;

            Summer.Rain.Late = 10;
            Summer.Storm.Late = 10;
            Summer.Wind.Late = 10;
            Summer.Snow.Late = 0;

            // Fall:
            // Very dry and windy Fall, small chance of snow
            Fall.Rain.Early = 8;
            Fall.Storm.Early = 5;
            Fall.Wind.Early = 20;
            Fall.Snow.Early = 0;

            Fall.Rain.Mid = 6;
            Fall.Storm.Mid = 4;
            Fall.Wind.Mid = 40;
            Fall.Snow.Mid = 2;

            Fall.Rain.Late = 4;
            Fall.Storm.Late = 0;
            Fall.Wind.Late = 30;
            Fall.Snow.Late = 10;

            // Winter:
            // Lots of snow in Winter, seldom rain
            Winter.Rain.Early = 2;
            Winter.Storm.Early = 0;
            Winter.Wind.Early = 15;
            Winter.Snow.Early = 30;

            Winter.Rain.Mid = 0;
            Winter.Storm.Mid = 0;
            Winter.Wind.Mid = 20;
            Winter.Snow.Mid = 50;

            Winter.Rain.Late = 10;
            Winter.Storm.Late = 0;
            Winter.Wind.Late = 40;
            Winter.Snow.Late = 20;
        }
    }

    /// <summary>
    /// Generic container for weather probabilities throughout the year.
    /// </summary>
    public class ModelDefinition
    {
        /// <summary>
        /// Likelihood in Spring.
        /// </summary>
        public Season Spring { get; set; }

        /// <summary>
        /// Likelihood in Summer.
        /// </summary>
        public Season Summer { get; set; }

        /// <summary>
        /// Likelihood in Fall.
        /// </summary>
        public Season Fall { get; set; }

        /// <summary>
        /// Likelihood in Winter.
        /// </summary>
        public Season Winter { get; set; }

        public ModelDefinition()
        {
            Spring = new Season();
            Summer = new Season();
            Fall = new Season();
            Winter = new Season();
        }
    }

    /// <summary>
    /// Generic container for weather probabilities within the season.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Likelihood of rain.
        /// </summary>
        public Weather Rain { get; set; }

        /// <summary>
        /// Likelihood of thunderstoms.
        /// </summary>
        public Weather Storm { get; set; }

        /// <summary>
        /// Likelihood of windy weather.
        /// </summary>
        public Weather Wind { get; set; }

        /// <summary>
        /// Likelihood of snowfall.
        /// </summary>
        public Weather Snow { get; set; }

        public Season()
        {
            Rain = new Weather();
            Storm = new Weather();
            Wind = new Weather();
            Snow = new Weather();
        }
    }

    /// <summary>
    /// Generic container for a weather's probabilities within a season.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// Probability on days 1-9.
        /// </summary>
        public double Early { get; set; }
        /// <summary>
        /// Probability on days 10-19.
        /// </summary>
        public double Mid { get; set; }
        /// <summary>
        /// Probability on days 20-28.
        /// </summary>
        public double Late { get; set; }
    }

    /// <summary>
    /// Stores the most recent interpolated probability arrays.
    /// </summary>
    public class WeatherArrays
    {
        /// <summary>
        /// Probability of rain on each day of the year.
        /// </summary>
        public double[] rainArray { get; set; }
        /// <summary>
        /// Probability of thunderstorms on each day of the year.
        /// </summary>
        public double[] stormArray { get; set; }
        /// <summary>
        /// Probability of wind on each day of the year.
        /// </summary>
        public double[] windArray { get; set; }
        /// <summary>
        /// Probability of snow on each day of the year.
        /// </summary>
        public double[] snowArray { get; set; }
    }
}
