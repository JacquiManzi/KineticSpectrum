using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment.Coloring
{
    public class ColorPicker
    {
        private static readonly IDictionary<Color, IList<Color>> ColorMap;
        private static readonly List<Color> AllColors = new List<Color>();
        private static readonly Random Random = new Random();

        private static readonly IDictionary<byte, IDictionary<byte, IDictionary<byte, Color>>> _colorLookup;
        static ColorPicker()
        {
            ColorMap = new Dictionary<Color, IList<Color>>();
            LoadMap();

            _colorLookup = new Dictionary<byte, IDictionary<byte, IDictionary<byte, Color>>>();
            BuildColorLookup();
        }

        private static void BuildColorLookup()
        {
            foreach (Color color in AllColors)
            {
                if (!_colorLookup.ContainsKey(color.R))
                {
                    _colorLookup[color.R] = new Dictionary<byte, IDictionary<byte, Color>>();
                }
                var gDict = _colorLookup[color.R];

                if (!gDict.ContainsKey(color.G))
                {
                    gDict[color.G] = new Dictionary<byte, Color>();
                }
                var bDict = gDict[color.G];

                if (!bDict.ContainsKey(color.B))
                {
                    bDict[color.B] = color;
                }
            }
        }

        private static Color FindClosest(Color color)
        {
            Color closest=Colors.Black;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                IDictionary<byte, IDictionary<byte, Color>> inner;
                byte r = (byte) (color.R + i);
                if (color.R + i <= byte.MaxValue && 
                      _colorLookup.TryGetValue(r, out inner))
                {
                    if (FindClosestGreen(inner, color,i, out closest))
                        return closest;
                }

                r = (byte) (color.R + i);
                if (color.R - i >= byte.MinValue && 
                      _colorLookup.TryGetValue(r, out inner))
                {
                    if (FindClosestGreen(inner, color,i, out closest))
                        return closest;
                }
            }
            return closest;
        }

        private static bool FindClosestGreen(IDictionary<byte, IDictionary<byte, Color>> greenDict, Color color,int max, out Color closest)
        {
            closest = Colors.Black;
            for (int i = 0; i <= max; i++)
            {
                IDictionary<byte, Color> inner;
                byte g = (byte) (color.G + i);
                if (color.G + i <= byte.MaxValue && 
                      greenDict.TryGetValue(g, out inner))
                {
                    if (FindClosestBlue(inner, color, i, out closest))
                        return true;
                }

                g = (byte) (color.G + i);
                if (color.G - i >= byte.MinValue && 
                      greenDict.TryGetValue(g, out inner))
                {
                    if (FindClosestBlue(inner, color, i, out closest))
                        return true;
                }
            }
            return false;
        }

        private static bool FindClosestBlue(IDictionary<byte, Color> blueDict, Color color,int max, out Color closest)
        {
            closest = Colors.Black;
            for (int i = 0; i <= max; i++)
            {
                Color inner;
                byte b = (byte) (color.B + i);
                if (color.B + i <= byte.MaxValue && 
                      blueDict.TryGetValue(b, out inner))
                {
                    return true;
                }

                b = (byte) (color.B + i);
                if (color.B - i >= byte.MinValue && 
                      blueDict.TryGetValue(b, out inner))
                {
                    return true;
                }
            }
            return false;
        }

        private static void LoadMap()
        {
            StreamReader reader = File.OpenText("../colorData.txt");
            ISet<Color> colorSet = new HashSet<Color>();
            colorSet.Add(Colors.Black);
            string line;
            while (null != (line = reader.ReadLine()))
            {
                string[] cStrings = line.Split(',');
                if (cStrings.GetLength(0) == 2)
                {
                    Color c1 = ColorUtil.FromInt(int.Parse(cStrings[0]));
                    Color c2 = ColorUtil.FromInt(int.Parse(cStrings[1]));
                    AddColor(c1, c2);
                    AddColor(c2, c1);
                    colorSet.Add(c1);
                    colorSet.Add(c2);
                }
            }
            AllColors.AddRange(colorSet);
            ColorMap[Colors.Black] = AllColors;
        }

        private static void AddColor(Color c1, Color c2)
        {
            IList<Color> cList;
            if (!ColorMap.TryGetValue(c1, out cList))
            {
                cList = new List<Color>();
                ColorMap[c1] = cList;
            }
            cList.Add(c2);
        }

        public static IList<Color> PickColors(int numColors)
        {
            Color first = AllColors[Random.Next(AllColors.Count)];
            return PickColors(first, numColors);
        }

        public static IList<Color> PickColors(Color startColor, int numColors /*including start*/)
        {
            List<Color> colors = new List<Color>();
            colors.Add(startColor);
            startColor = FindClosest(startColor);
            //already used one, generate until no more
            for (numColors--; numColors > 0; numColors--)
            {
                IList<Color> matched = ColorMap[startColor];
                startColor = matched[Random.Next(matched.Count)];
                colors.Add(startColor);
            }
            return colors;
        }
    }
}
