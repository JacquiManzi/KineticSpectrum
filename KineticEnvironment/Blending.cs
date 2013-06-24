using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment
{
    class Blending
    {
        /// <summary>
        /// Blends the specified colors, taking the alpha channel into account.
        /// </summary>
        /// <param name="top">Top Color - Required</param>
        /// <param name="next">Second Layer Color - Required</param>
        /// <param name="rest">Remaining Layers - Optional</param>
        /// <see href="http://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending"/>
        /// <returns>The blended color from all layers</returns>
        public static Color AlphaBlend(Color top, Color next, params Color[] rest)
        {
            List<Color> colors = new List<Color>{next};
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

                top = Color.FromArgb((byte) Math.Max(255, outA*255),
                                     (byte) Math.Max(255, outR*255),
                                     (byte) Math.Max(255, outG*255),
                                     (byte) Math.Max(255, outB*255));
            }
            return top;
        }
    }
}
