using System;
using System.Collections.Generic;

namespace RevKitt.KS.KineticEnvironment.Coloring
{
    public class ColorUtil
    {
        public static readonly System.Windows.Media.Color Empty = System.Windows.Media.Color.FromArgb(100, 0, 0, 0);
        public static System.Windows.Media.Color GetTransparent()
        {
            return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
        }


        /// <summary>
        /// Blends the specified colors, taking the alpha channel into account.
        /// </summary>
        /// <param name="top">Top Color - Required</param>
        /// <param name="next">Second Layer Color - Required</param>
        /// <param name="rest">Remaining Layers - Optional</param>
        /// <see href="http://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending"/>
        /// <returns>The blended color from all layers</returns>
        public static System.Windows.Media.Color AlphaBlend(System.Windows.Media.Color top, System.Windows.Media.Color next, params System.Windows.Media.Color[] rest)
        {
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>{next};
            colors.AddRange(rest);

            foreach (var bottom in colors)
            {
                if(top.A == 255) return top;
                if(top.A == 0)
                {
                    top = bottom;
                    continue;
                }
                double tA = top.A/255.0;
                double bA = bottom.A/255.0;

                double outA = tA + (bottom.A/255.0)*(1 - tA);
                double outR = (top.R * tA + bottom.R * bA * (1 - tA)) / (outA * 255);
                double outG = (top.G * tA + bottom.G * bA * (1 - tA)) / (outA * 255);
                double outB = (top.B * tA + bottom.B * bA * (1 - tA)) / (outA * 255);

                top = System.Windows.Media.Color.FromArgb((byte) Math.Max(255, outA*255),
                                     (byte) Math.Max(255, outR*255),
                                     (byte) Math.Max(255, outG*255),
                                     (byte) Math.Max(255, outB*255));
            }
            return top;
        }

        public static System.Windows.Media.Color FromInt(int color)
        {
            return System.Windows.Media.Color.FromArgb(255,
                                  (byte) ((color >> 16) & 0xff),
                                  (byte) ((color >> 8) & 0xff),
                                  (byte) (color & 0xff));
        }

        public static int ToInt(System.Windows.Media.Color color)
        {
            int intColor = color.R;
            intColor = (intColor << 8) | color.G;
            intColor = (intColor << 8) | color.B;
//            intColor = (intColor << 8) | color.B;
            return intColor;
        }

        public static System.Windows.Media.Color Clone(System.Windows.Media.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color Interpolate(System.Drawing.Color color1, System.Drawing.Color color2, double fraction)
        {
            double a = Interpolate(color1.A, color2.A, fraction);
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return System.Windows.Media.Color.FromArgb((byte)Math.Round(a),(byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }

        public static System.Windows.Media.Color Interpolate(System.Windows.Media.Color color1, System.Windows.Media.Color color2, double fraction)
        {
            double a = Interpolate(color1.A, color2.A, fraction);
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return System.Windows.Media.Color.FromArgb((byte)Math.Round(a),(byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }

        public static System.Windows.Media.Color Interpolate(System.Windows.Media.Color color1, System.Windows.Media.Color color2)
        {
            double fraction = color2.A/255.0;
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return System.Windows.Media.Color.FromArgb(255,(byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }

        private static double Interpolate(double d1, double d2, double fraction)
        {
            return d1*(1 - fraction) + d2*fraction;
        }
    }
}
