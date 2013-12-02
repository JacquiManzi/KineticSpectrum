using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Coloring;
using Color = System.Windows.Media.Color;

namespace RevKitt.KS.KineticEnvironment
{
    public class Images
    {
        private static readonly IDictionary<string, ImageRef> _nameMap = new Dictionary<string, ImageRef>();

        public static IList<String> GetImages(string directory)
        {
            string[] png = Directory.GetFiles(directory, "*.png");
            string[] jpg = Directory.GetFiles(directory, "*.jpg");
            IList<string> names = new List<string>();
            IEnumerable<string> files = png.Union(jpg);
            foreach (var file in files)
            {
                string shortName = new string(file.Reverse().TakeWhile(c => c != '\\').Reverse().ToArray());
                if (!_nameMap.ContainsKey(file))
                {
                    _nameMap[shortName]=new ImageRef(file);
                }
                names.Add(shortName);
            }

            return names;
        }

        public static ImageRef GetImage(string imageName)
        {
            if (_nameMap.ContainsKey(imageName))
                return _nameMap[imageName].Create();
            throw new ArgumentException("Specified Image does not exist");
        }
    }

    public class ImageRef
    {
        private Bitmap _image;
        private Color[,] _colorData;

        public ImageRef(string fileName)
        {
            FileName = fileName;
        }

        public ImageRef Create()
        {
            if (null == _image)
            {
                _image = new Bitmap(FileName);
                Width = _image.Width - 1;
                Height = _image.Height - 1;
                _colorData = new Color[Width,Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        System.Drawing.Color color = _image.GetPixel(x + 1, y + 1);
                        _colorData[x, y] = Color.FromArgb(color.A, color.R, color.G, color.B);
                    }
                }
            }
            return this;
        }

        public string FileName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Color GetValueAt(double x, double y)
        {
            double xPos = x*(Width-1);
            double yPos = y*(Height-1);

            double xPortion = xPos%1;
            double yPortion = yPos%1;

            int xFl = (int) Math.Floor(xPos);
            int yFl = (int) Math.Floor(yPos);
            int xCe = (int) Math.Ceiling(xPos);
            int yCe = (int) Math.Ceiling(yPos);

            //First interpolate across the x co-ordinates
            Color x1 = ColorUtil.Interpolate(_colorData[xFl, yFl], _colorData[xCe, yFl], xPortion);
            Color x2 = ColorUtil.Interpolate(_colorData[xFl, yCe], _colorData[xCe, yCe], xPortion);


            //then interpolate across y
            return ColorUtil.Interpolate(x1, x2, yPortion);
        }
    }
}