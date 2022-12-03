using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    public sealed class CCConfig
    {
        // Config values for weather chances
        
        // Spring // very wet but light intensity, vanishing chance of snow
        public double SpringRainChance { get; set; } = 40;
        public double SpringStormChance { get; set; } = 15;
        public double SpringWindChance { get; set; } = 20;
        public double SpringSnowChance { get; set; } = 0.25;

        // Summer // mostly sunny, but can be intense rain
        public double SummerRainChance { get; set; } = 10; // Watch out, TV overrides into storms 85% of the time!
        public double SummerStormChance { get; set; } = 20;
        public double SummerWindChance { get; set; } = 5; // TV says snow, but creates fall-colored leaves
        public double SummerSnowChance { get; set; } = 0;

        // Fall // very dry and windy, small chance of snow
        public double FallRainChance { get; set; } = 4;
        public double FallStormChance { get; set; } = 2;
        public double FallWindChance { get; set; } = 60;
        public double FallSnowChance { get; set; } = 1;

        // Winter // lots of snow, seldom rain
        public double WinterRainChance { get; set; } = 5;
        public double WinterStormChance { get; set; } = 0;
        public double WinterWindChance { get; set; } = 20; // TV says snow, but creates gentle snowflakes (really pretty)
        public double WinterSnowChance { get; set; } = 60; // This is actually less than the game's base chance!
    }
}
