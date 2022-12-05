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
    public class StandardModel
    {
        public ModelDefinition Model { get; set; }

        public StandardModel()
        {
            Model = new ModelDefinition();

            // very wet but light intensity, vanishing chance of snow
            Model.Spring.Rain.Mid = 40;
            Model.Spring.Storm.Mid = 15;
            Model.Spring.Wind.Mid = 20;
            Model.Spring.Snow.Mid = 1;

            // mostly sunny, but can be intense rain
            Model.Summer.Rain.Mid = 10;
            Model.Summer.Storm.Mid = 20;
            Model.Summer.Wind.Mid = 5;
            Model.Summer.Snow.Mid = 0;

            // very dry and windy, small chance of snow
            Model.Fall.Rain.Mid = 8;
            Model.Fall.Storm.Mid = 4;
            Model.Fall.Wind.Mid = 40;
            Model.Fall.Snow.Mid = 2;

            // lots of snow, seldom rain
            Model.Winter.Rain.Mid = 5;
            Model.Winter.Storm.Mid = 0;
            Model.Winter.Wind.Mid = 20;
            Model.Winter.Snow.Mid = 40;
        }
    }

    public class ModelDefinition
    {
        public Season Spring { get; set; }
        public Season Summer { get; set; }
        public Season Fall { get; set; }
        public Season Winter { get; set; }

        public ModelDefinition()
        {
            Spring = new Season();
            Summer = new Season();
            Fall = new Season();
            Winter = new Season();
        }
    }

    public class Season
    {
        public Weather Rain { get; set; }
        public Weather Storm { get; set; }
        public Weather Wind { get; set; }
        public Weather Snow { get; set; }

        public Season()
        {
            Rain = new Weather();
            Storm = new Weather();
            Wind = new Weather();
            Snow = new Weather();
        }
    }

    public class Weather
    {
        public double Mid { get; set; }
    }
}
