using IWClimateControl;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// Handles model interpolation onto a daily grid by using cubic spline interpolation.
    /// </summary>
    /// <remarks>See <see href="https://swharden.com/blog/2022-06-23-resample-interpolation/">this article</see> for source code. Unfortunately, no easy way to include plotting libraries in SMAPI, so have to use source code and store as numbers.</remarks>
    internal class Interpolator
    {
        public static WeatherArrays InterpolateWeather()
        {
            // Define arrays for each weather type.
            double[] rainChances =
            {
                ClimateControl.s_weatherChances.Winter.Rain.Mid,
                ClimateControl.s_weatherChances.Winter.Rain.Late,
                ClimateControl.s_weatherChances.Spring.Rain.Early,
                ClimateControl.s_weatherChances.Spring.Rain.Mid,
                ClimateControl.s_weatherChances.Spring.Rain.Late,
                ClimateControl.s_weatherChances.Summer.Rain.Early,
                ClimateControl.s_weatherChances.Summer.Rain.Mid,
                ClimateControl.s_weatherChances.Summer.Rain.Late,
                ClimateControl.s_weatherChances.Fall.Rain.Early,
                ClimateControl.s_weatherChances.Fall.Rain.Mid,
                ClimateControl.s_weatherChances.Fall.Rain.Late,
                ClimateControl.s_weatherChances.Winter.Rain.Early,
                ClimateControl.s_weatherChances.Winter.Rain.Mid,
                ClimateControl.s_weatherChances.Winter.Rain.Late,
                ClimateControl.s_weatherChances.Spring.Rain.Early,
                ClimateControl.s_weatherChances.Spring.Rain.Mid,
            };

            double[] stormChances =
            {
                ClimateControl.s_weatherChances.Winter.Storm.Mid,
                ClimateControl.s_weatherChances.Winter.Storm.Late,
                ClimateControl.s_weatherChances.Spring.Storm.Early,
                ClimateControl.s_weatherChances.Spring.Storm.Mid,
                ClimateControl.s_weatherChances.Spring.Storm.Late,
                ClimateControl.s_weatherChances.Summer.Storm.Early,
                ClimateControl.s_weatherChances.Summer.Storm.Mid,
                ClimateControl.s_weatherChances.Summer.Storm.Late,
                ClimateControl.s_weatherChances.Fall.Storm.Early,
                ClimateControl.s_weatherChances.Fall.Storm.Mid,
                ClimateControl.s_weatherChances.Fall.Storm.Late,
                ClimateControl.s_weatherChances.Winter.Storm.Early,
                ClimateControl.s_weatherChances.Winter.Storm.Mid,
                ClimateControl.s_weatherChances.Winter.Storm.Late,
                ClimateControl.s_weatherChances.Spring.Storm.Early,
                ClimateControl.s_weatherChances.Spring.Storm.Mid,
            };

            double[] windChances =
            {
                ClimateControl.s_weatherChances.Winter.Wind.Mid,
                ClimateControl.s_weatherChances.Winter.Wind.Late,
                ClimateControl.s_weatherChances.Spring.Wind.Early,
                ClimateControl.s_weatherChances.Spring.Wind.Mid,
                ClimateControl.s_weatherChances.Spring.Wind.Late,
                ClimateControl.s_weatherChances.Summer.Wind.Early,
                ClimateControl.s_weatherChances.Summer.Wind.Mid,
                ClimateControl.s_weatherChances.Summer.Wind.Late,
                ClimateControl.s_weatherChances.Fall.Wind.Early,
                ClimateControl.s_weatherChances.Fall.Wind.Mid,
                ClimateControl.s_weatherChances.Fall.Wind.Late,
                ClimateControl.s_weatherChances.Winter.Wind.Early,
                ClimateControl.s_weatherChances.Winter.Wind.Mid,
                ClimateControl.s_weatherChances.Winter.Wind.Late,
                ClimateControl.s_weatherChances.Spring.Wind.Early,
                ClimateControl.s_weatherChances.Spring.Wind.Mid,
            };

            double[] snowChances =
            {
                ClimateControl.s_weatherChances.Winter.Snow.Mid,
                ClimateControl.s_weatherChances.Winter.Snow.Late,
                ClimateControl.s_weatherChances.Spring.Snow.Early,
                ClimateControl.s_weatherChances.Spring.Snow.Mid,
                ClimateControl.s_weatherChances.Spring.Snow.Late,
                ClimateControl.s_weatherChances.Summer.Snow.Early,
                ClimateControl.s_weatherChances.Summer.Snow.Mid,
                ClimateControl.s_weatherChances.Summer.Snow.Late,
                ClimateControl.s_weatherChances.Fall.Snow.Early,
                ClimateControl.s_weatherChances.Fall.Snow.Mid,
                ClimateControl.s_weatherChances.Fall.Snow.Late,
                ClimateControl.s_weatherChances.Winter.Snow.Early,
                ClimateControl.s_weatherChances.Winter.Snow.Mid,
                ClimateControl.s_weatherChances.Winter.Snow.Late,
                ClimateControl.s_weatherChances.Spring.Snow.Early,
                ClimateControl.s_weatherChances.Spring.Snow.Mid,
            };

            // Define day grid which overlaps with previous and following year.
            double[] daysToInterpolate = new double[16];
            // Early in season.
            int earlyDay = 5;
            // Middle of season.
            int midDay = 15;
            // End of season.
            int lateDay = 24;
            // Mid-winter previous year should start on Day 0.
            int startDay = midDay - 28;
            // Day to include at end after interpolation (to get full year).
            int extraDays = 28 - lateDay;
            // Mid-winter previous year.
            daysToInterpolate[0] = midDay - 28 - startDay;
            // Late-winter previous year.
            daysToInterpolate[1] = lateDay - 28 - startDay;
            // Early-spring this year.
            daysToInterpolate[2] = earlyDay - startDay;
            // Generate the rest of the days.
            for (int i = 3; i <= 15; i++)
            {
                daysToInterpolate[i] = daysToInterpolate[i - 3] + 28;
            }

            // Grab the interpolated arrays.
            (double[] daysInterpolated, double[] rainInterpolated) = Interpolate1D(daysToInterpolate, rainChances, 28 * 5 + 1);
            (_, double[] stormInterpolated) = Interpolate1D(daysToInterpolate, stormChances, 28 * 5 + 1);
            (_, double[] windInterpolated) = Interpolate1D(daysToInterpolate, windChances, 28 * 5 + 1);
            (_, double[] snowInterpolated) = Interpolate1D(daysToInterpolate, snowChances, 28 * 5 + 1);

            // Reduce to only relevant ranges.
            double[] usefulDays = new double[28 * 4];
            double[] usefulRain = new double[28 * 4];
            double[] usefulStorm = new double[28 * 4];
            double[] usefulWind = new double[28 * 4];
            double[] usefulSnow = new double[28 * 4];
            for (int i = 0; i < daysInterpolated.Length; i++)
            {
                if ((i + startDay > 0) && (i + startDay <= 28 * 4))
                {
                    // Spring 1 is on Day 1, so offset by 1.
                    usefulDays[i + startDay - 1] = i + startDay;
                    usefulRain[i + startDay - 1] = rainInterpolated[i];
                    usefulStorm[i + startDay - 1] = stormInterpolated[i];
                    usefulWind[i + startDay - 1] = windInterpolated[i];
                    usefulSnow[i + startDay - 1] = snowInterpolated[i];
                }
            }

            // Shift original day array, so can compare with original chances arrays.
            for (int i = 0; i < daysToInterpolate.Length; i++)
            {
                daysToInterpolate[i] += startDay;
            }

            // Set any negative values to 0.
            for (int i = 0; i < usefulDays.Length; i++)
            {
                if (usefulRain[i] < 0)
                    usefulRain[i] = 0.0;
                if (usefulStorm[i] < 0)
                    usefulStorm[i] = 0.0;
                if (usefulWind[i] < 0)
                    usefulWind[i] = 0.0;
                if (usefulSnow[i] < 0)
                    usefulSnow[i] = 0.0;
            }

            // Wherever two consecutive inputs are 0, all intervening values should be 0.
            for (int i = 3; i < daysToInterpolate.Length - 2; i++)
            {
                if ((rainChances[i] == 0) && (rainChances[i-1] == 0))
                {
                    for (int j = (int)daysToInterpolate[i - 1]; j <= (int)daysToInterpolate[i]; j++)
                    {
                        usefulRain[j-1] = 0.0;
                    }
                }
                if ((stormChances[i] == 0) && (stormChances[i - 1] == 0))
                {
                    for (int j = (int)daysToInterpolate[i - 1]; j <= (int)daysToInterpolate[i]; j++)
                    {
                        usefulStorm[j - 1] = 0.0;
                    }
                }
                if ((windChances[i] == 0) && (windChances[i - 1] == 0))
                {
                    for (int j = (int)daysToInterpolate[i - 1]; j <= (int)daysToInterpolate[i]; j++)
                    {
                        usefulWind[j - 1] = 0.0;
                    }
                }
                if ((snowChances[i] == 0) && (snowChances[i - 1] == 0))
                {
                    for (int j = (int)daysToInterpolate[i - 1]; j <= (int)daysToInterpolate[i]; j++)
                    {
                        usefulSnow[j - 1] = 0.0;
                    }
                }
            }

            // Store these arrays
            return new WeatherArrays()
            {
                rainArray = usefulRain,
                stormArray = usefulStorm,
                windArray = usefulWind,
                snowArray = usefulSnow,
            };
        }

        public static (double[] xs, double[] ys) Interpolate1D(double[] xs, double[] ys, int count)
        {
            if (xs is null || ys is null || xs.Length != ys.Length)
                throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have same length");

            int inputPointCount = xs.Length;
            double[] inputDistances = new double[inputPointCount];
            for (int i = 1; i < inputPointCount; i++)
                inputDistances[i] = inputDistances[i - 1] + xs[i] - xs[i - 1];

            double meanDistance = inputDistances.Last() / (count - 1);
            double[] evenDistances = Enumerable.Range(0, count).Select(x => x * meanDistance).ToArray();
            double[] xsOut = Interpolate(inputDistances, xs, evenDistances);
            double[] ysOut = Interpolate(inputDistances, ys, evenDistances);
            return (xsOut, ysOut);
        }

        private static double[] Interpolate(double[] xOrig, double[] yOrig, double[] xInterp)
        {
            (double[] a, double[] b) = FitMatrix(xOrig, yOrig);

            double[] yInterp = new double[xInterp.Length];
            for (int i = 0; i < yInterp.Length; i++)
            {
                int j;
                for (j = 0; j < xOrig.Length - 2; j++)
                    if (xInterp[i] <= xOrig[j + 1])
                        break;

                double dx = xOrig[j + 1] - xOrig[j];
                double t = (xInterp[i] - xOrig[j]) / dx;
                double y = (1 - t) * yOrig[j] + t * yOrig[j + 1] +
                    t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
                yInterp[i] = y;
            }

            return yInterp;
        }

        private static (double[] a, double[] b) FitMatrix(double[] x, double[] y)
        {
            int n = x.Length;
            double[] a = new double[n - 1];
            double[] b = new double[n - 1];
            double[] r = new double[n];
            double[] A = new double[n];
            double[] B = new double[n];
            double[] C = new double[n];

            double dx1, dx2, dy1, dy2;

            dx1 = x[1] - x[0];
            C[0] = 1.0f / dx1;
            B[0] = 2.0f * C[0];
            r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);

            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];
                A[i] = 1.0f / dx1;
                C[i] = 1.0f / dx2;
                B[i] = 2.0f * (A[i] + C[i]);
                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            dx1 = x[n - 1] - x[n - 2];
            dy1 = y[n - 1] - y[n - 2];
            A[n - 1] = 1.0f / dx1;
            B[n - 1] = 2.0f * A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));

            double[] cPrime = new double[n];
            cPrime[0] = C[0] / B[0];
            for (int i = 1; i < n; i++)
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);

            double[] dPrime = new double[n];
            dPrime[0] = r[0] / B[0];
            for (int i = 1; i < n; i++)
                dPrime[i] = (r[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);

            double[] k = new double[n];
            k[n - 1] = dPrime[n - 1];
            for (int i = n - 2; i >= 0; i--)
                k[i] = dPrime[i] - cPrime[i] * k[i + 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1] * dx1 - dy1;
                b[i - 1] = -k[i] * dx1 + dy1;
            }

            return (a, b);
        }
    }
}
