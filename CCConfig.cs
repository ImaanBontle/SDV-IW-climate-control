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
        
        // Spring
        public double SpringRainChance { get; set; } = 40;
        public double SpringStormChance { get; set; } = 15;
        public double SpringWindChance { get; set; } = 10;
        public double SpringSnowChance { get; set; } = 0.5;

        // Summer
        public double SummerRainChance { get; set; } = 10;
        public double SummerStormChance { get; set; } = 20;
        public double SummerWindChance { get; set; } = 5; // TV says snow, but actually creates fall-colored spring leaves
        public double SummerSnowChance { get; set; } = 0;

        // Fall
        public double FallRainChance { get; set; } = 8;
        public double FallStormChance { get; set; } = 4;
        public double FallWindChance { get; set; } = 30;
        public double FallSnowChance { get; set; } = 2;

        // Winter
        public double WinterRainChance { get; set; } = 5;
        public double WinterStormChance { get; set; } = 0;
        public double WinterWindChance { get; set; } = 20; // TV says snow, creates gentle snowflakes (really pretty)
        public double WinterSnowChance { get; set; } = 30;
    }
}
