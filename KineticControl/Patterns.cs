using System;
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


        public Patterns()
        {
            AllColors.Add(Colors.Aqua);
            AllColors.Add(Colors.AliceBlue);
            AllColors.Add(Colors.Aquamarine);
            AllColors.Add(Colors.Blue);
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

            Blues.Add(Colors.Aqua);
            Blues.Add(Colors.Aquamarine);
            Blues.Add(Colors.Blue);
            Blues.Add(Colors.CadetBlue);
            Blues.Add(Colors.CornflowerBlue);
            Blues.Add(Colors.Cyan);
            Blues.Add(Colors.DarkBlue);
            Blues.Add(Colors.DarkCyan);

            Reds.Add(Colors.Brown);
            Reds.Add(Colors.Crimson);

            Oranges.Add(Colors.Chocolate);
            Oranges.Add(Colors.Coral);
            Oranges.Add(Colors.DarkGoldenrod);
            Oranges.Add(Colors.DarkOrange);

            Greens.Add(Colors.Chartreuse);
            Greens.Add(Colors.DarkGreen);

            Pinks.Add(Colors.DarkMagenta);
            Pinks.Add(Colors.DarkOrchid);
            Pinks.Add(Colors.DarkSalmon);
            Pinks.Add(Colors.Salmon);
            Pinks.Add(Colors.BlueViolet); 
        }

        public List<Color> GetColors()
        {
            return AllColors;
        }

        public List<Color> GetBlues()
        {
            return Blues;
        }

        

    }
}
