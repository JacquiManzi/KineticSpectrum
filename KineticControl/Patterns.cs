using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace KineticControl
{
    public class Patterns
    {
        private List<Color> AllColors = new List<Color>();
        private List<Color> Blues = new List<Color>();
        private List<Color> Oranges = new List<Color>();
        private List<Color> Yellows = new List<Color>();
        private List<Color> Greens = new List<Color>();
        private List<Color> Reds = new List<Color>();
        private List<Color> Pinks = new List<Color>();
        private List<Color> Purples = new List<Color>();
        private Boolean change = true;



        public Patterns()
        {
            AllColors.Add(Colors.Blue);
            AllColors.Add(Colors.Red);
            AllColors.Add(Colors.Green);
            AllColors.Add(Colors.Aqua);
            AllColors.Add(Colors.AliceBlue);
            AllColors.Add(Colors.Aquamarine);
            AllColors.Add(Colors.BlueViolet);
            AllColors.Add(Colors.Brown);
            AllColors.Add(Colors.CadetBlue);
            AllColors.Add(Colors.Chocolate);
            AllColors.Add(Colors.Chartreuse);
            AllColors.Add(Colors.Coral);
            AllColors.Add(Colors.CornflowerBlue);
            AllColors.Add(Colors.Crimson);
            AllColors.Add(Colors.Cyan);
            AllColors.Add(Colors.DarkBlue);
            AllColors.Add(Colors.DarkCyan);
            AllColors.Add(Colors.DarkGreen);
            AllColors.Add(Colors.DarkMagenta);
            AllColors.Add(Colors.DarkOrange);
            AllColors.Add(Colors.DarkOrchid);
            AllColors.Add(Colors.DarkSalmon);
            AllColors.Add(Colors.DarkSlateBlue);
            AllColors.Add(Colors.DarkSlateGray);
            AllColors.Add(Colors.DarkTurquoise);
            AllColors.Add(Colors.DarkViolet);
            AllColors.Add(Colors.DeepPink);
            AllColors.Add(Colors.DeepSkyBlue);
            AllColors.Add(Colors.DodgerBlue);
            AllColors.Add(Colors.Firebrick);
            AllColors.Add(Colors.DarkRed);
            AllColors.Add(Colors.ForestGreen);
            AllColors.Add(Colors.Green);
            AllColors.Add(Colors.Fuchsia);
            AllColors.Add(Colors.Gold);
            AllColors.Add(Colors.Purple);
            AllColors.Add(Colors.Goldenrod);
            AllColors.Add(Colors.GreenYellow);
            AllColors.Add(Colors.HotPink);
            AllColors.Add(Colors.IndianRed);
            AllColors.Add(Colors.Indigo);
            AllColors.Add(Colors.LawnGreen);
            AllColors.Add(Colors.LightCoral);
            AllColors.Add(Colors.LightPink);
            AllColors.Add(Colors.LightSalmon);
            AllColors.Add(Colors.LightSeaGreen);
            AllColors.Add(Colors.Magenta);
            AllColors.Add(Colors.Maroon);
            AllColors.Add(Colors.MediumAquamarine);
            AllColors.Add(Colors.MediumBlue);
            AllColors.Add(Colors.MediumOrchid);
            AllColors.Add(Colors.MediumPurple);
            AllColors.Add(Colors.MediumSeaGreen);
            AllColors.Add(Colors.MediumSlateBlue);
            AllColors.Add(Colors.MediumSpringGreen);
            AllColors.Add(Colors.White);
            AllColors.Add(Colors.MediumTurquoise);
            AllColors.Add(Colors.MediumVioletRed);
            AllColors.Add(Colors.Navy);
            AllColors.Add(Colors.Olive);
            AllColors.Add(Colors.OliveDrab);
            AllColors.Add(Colors.OrangeRed);
            AllColors.Add(Colors.Orchid);
            AllColors.Add(Colors.PaleVioletRed);
            AllColors.Add(Colors.Peru);
            AllColors.Add(Colors.Pink);
            AllColors.Add(Colors.Plum);
            AllColors.Add(Colors.RoyalBlue);
            AllColors.Add(Colors.SaddleBrown);
            AllColors.Add(Colors.SeaGreen);
            AllColors.Add(Colors.Sienna);
            AllColors.Add(Colors.SkyBlue);
            AllColors.Add(Colors.SlateBlue);
            AllColors.Add(Colors.SpringGreen);
            AllColors.Add(Colors.SteelBlue);
            AllColors.Add(Colors.Teal);
            AllColors.Add(Colors.Tomato);
            AllColors.Add(Colors.Turquoise);
            AllColors.Add(Colors.Violet);
            AllColors.Add(Colors.YellowGreen);

            Blues.Add(Colors.Aqua);
            Blues.Add(Colors.Aquamarine);
            Blues.Add(Colors.Blue);
            Blues.Add(Colors.CadetBlue);
            Blues.Add(Colors.CornflowerBlue);
            Blues.Add(Colors.Cyan);
            Blues.Add(Colors.DarkBlue);
            Blues.Add(Colors.DarkCyan);
            Blues.Add(Colors.DarkSlateGray);
            Blues.Add(Colors.DarkTurquoise);
            Blues.Add(Colors.DeepSkyBlue);
            Blues.Add(Colors.DodgerBlue);
            Blues.Add(Colors.LightSeaGreen);
            Blues.Add(Colors.MediumAquamarine);
            Blues.Add(Colors.MediumBlue);
            Blues.Add(Colors.MediumTurquoise);
            Blues.Add(Colors.Navy);
            Blues.Add(Colors.RoyalBlue);
            Blues.Add(Colors.SeaGreen);
            Blues.Add(Colors.Teal);
            Blues.Add(Colors.SteelBlue);
            Blues.Add(Colors.SkyBlue);
            Blues.Add(Colors.Turquoise);
            
            Reds.Add(Colors.Brown);
            Reds.Add(Colors.Crimson);
            Reds.Add(Colors.Firebrick);
            Reds.Add(Colors.Red);
            Reds.Add(Colors.DarkRed);
            Reds.Add(Colors.Maroon);

            Oranges.Add(Colors.Chocolate);
            Oranges.Add(Colors.Coral);
            Oranges.Add(Colors.DarkGoldenrod);
            Oranges.Add(Colors.DarkOrange);
            Oranges.Add(Colors.Orange);
            Oranges.Add(Colors.OrangeRed);
            Oranges.Add(Colors.Peru);
            Oranges.Add(Colors.SaddleBrown);
            Oranges.Add(Colors.Sienna);

            Greens.Add(Colors.Chartreuse);
            Greens.Add(Colors.DarkGreen);
            Greens.Add(Colors.ForestGreen);
            Greens.Add(Colors.Green);
            Greens.Add(Colors.LawnGreen);
            Greens.Add(Colors.MediumSeaGreen);
            Greens.Add(Colors.MediumSpringGreen);
            Greens.Add(Colors.SpringGreen);

            Pinks.Add(Colors.DarkMagenta);
            Pinks.Add(Colors.DarkOrchid);
            Pinks.Add(Colors.DarkSalmon);
            Pinks.Add(Colors.Salmon);
            Pinks.Add(Colors.BlueViolet);
            Pinks.Add(Colors.DeepPink);
            Pinks.Add(Colors.Fuchsia);
            Pinks.Add(Colors.Purple);
            Pinks.Add(Colors.HotPink);
            Pinks.Add(Colors.IndianRed);
            Pinks.Add(Colors.LightCoral);
            Pinks.Add(Colors.LightPink);
            Pinks.Add(Colors.LightSalmon);
            Pinks.Add(Colors.Magenta);
            Pinks.Add(Colors.MediumOrchid);
            Pinks.Add(Colors.MediumVioletRed);
            Pinks.Add(Colors.Orchid);
            Pinks.Add(Colors.PaleVioletRed);
            Pinks.Add(Colors.Pink);
            Pinks.Add(Colors.Plum);
            Pinks.Add(Colors.Tomato);
            Pinks.Add(Colors.Violet);

            Purples.Add(Colors.DarkSlateBlue);
            Purples.Add(Colors.DarkViolet);
            Purples.Add(Colors.Indigo);
            Purples.Add(Colors.MediumPurple);
            Purples.Add(Colors.MediumSlateBlue);
            Purples.Add(Colors.SlateBlue);

            Yellows.Add(Colors.GreenYellow);
            Yellows.Add(Colors.Gold);
            Yellows.Add(Colors.Yellow);
            Yellows.Add(Colors.Goldenrod);
            Yellows.Add(Colors.Olive);
            Yellows.Add(Colors.OliveDrab);
            Yellows.Add(Colors.YellowGreen);
        }


        private Random _random = new Random();
        public Color RandomColor()
        {
            return AllColors[_random.Next(AllColors.Count)];
        }

        /*
         * This is an effect that displays a color to its max and the nto its mix
         */

        public void ParanoidZen(ref Color colorState, double adjfactor, ColorData colorData)
        {
          //  Console.WriteLine(colorState.B + " " +adjfactor);
            
            if (change)
            {
                adjfactor += .5;
                if (colorState.B - adjfactor < 0)
                {
                    colorState.B = 0;
                }
                else
                {

                    colorState.B = (byte)(colorState.B - adjfactor);
                }
                for (int i = 0; i < (colorData).Count; i++)
                  {
                    colorData[i] = colorState;
                     if (colorState.B == 0)
                     {
                       change = false;
                     }
                  }

            }
            else
            {
                adjfactor += .5;

                if (colorState.B + adjfactor > 255)
                {
                    colorState.B = 255;
                }
                else
                {
                    colorState.B = (byte) (colorState.B + adjfactor);
                }
                for (int i = 0; i < ( colorData).Count; i++)
                {
                    colorData[i] = colorState;
                    if (colorState.B == 255)
                    {
                        change = true;
                    }

                }

            }
            
        }

        /*
         * This is a color trailing effect
         * */

        public void HappyTrails(ColorData _colorData, double adjfactor)
        {
            for (int i = 0; i < _colorData.Count; i++)
            {
                Color color = _colorData[i];

                color.R = (byte) (color.R/adjfactor);
                color.G = (byte) (color.G/adjfactor);
                if(color.B < 250)
                    color.B = (byte) (color.B + adjfactor);
                _colorData[i] = color;
            }
        }

        public List<Color> GetColors()
        {
            return AllColors;
        }

        public List<Color> GetBlues()
        {
            return Blues;
        }

        public List<Color> GetReds()
        {
            return Reds;
        }

        public  List<Color> GetPurples()
        {
            return Purples;
        }

        public List<Color> GetGreens()
        {
            return Greens;
        }

        public List<Color> GetYellows()
        {
            return Yellows;
        }


        public List<Color> GetPinks()
        {
            return Pinks;
        }


        

    }
}
