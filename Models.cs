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
    public class StandardModel
    {
        /// <summary>
        /// The weather probabilities throughout the year.
        /// </summary>
        public ModelDefinition Model { get; set; }

        public StandardModel()
        {
            Model = new ModelDefinition();

            // Spring:
            // Very wet but light intensity, vanishing chance of snow
            Model.Spring.Rain.Mid = 40;
            Model.Spring.Storm.Mid = 15;
            Model.Spring.Wind.Mid = 20;
            Model.Spring.Snow.Mid = 1;

            // Summer:
            // Mostly sunny, but can be intense rain
            Model.Summer.Rain.Mid = 10;
            Model.Summer.Storm.Mid = 20;
            Model.Summer.Wind.Mid = 5;
            Model.Summer.Snow.Mid = 0;

            // Fall:
            // Very dry and windy Fall, small chance of snow
            Model.Fall.Rain.Mid = 8;
            Model.Fall.Storm.Mid = 4;
            Model.Fall.Wind.Mid = 40;
            Model.Fall.Snow.Mid = 2;

            // Winter:
            // Lots of snow in Winter, seldom rain
            Model.Winter.Rain.Mid = 5;
            Model.Winter.Storm.Mid = 0;
            Model.Winter.Wind.Mid = 20;
            Model.Winter.Snow.Mid = 40;
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
        /// Probability on day 10.
        /// </summary>
        public double Mid { get; set; }
    }
}
