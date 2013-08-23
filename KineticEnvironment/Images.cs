using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        public ImageRef(string fileName)
        {
            FileName = fileName;
        }

        public ImageRef Create()
        {
            if(null==_image)
                _image = new Bitmap(FileName);
            return this;
        }

        public string FileName { get; private set; }
        public int Width { get { return _image.Width; } }
        public int Height { get { return _image.Height; } }

        public Color GetValueAt(double x, double y)
        {
            double xPos = x*_image.Width;
            double yPos = y*_image.Height;

            double xPortion = xPos%1;
            double yPortion = yPos%1;

            int xFl = (int) Math.Floor(x);
            int yFl = (int) Math.Floor(y);
            int xCe = (int) Math.Ceiling(x);
            int yCe = (int) Math.Ceiling(y);

            //First interpolate across the x co-ordinates
            Color x1 = ColorUtil.Interpolate(_image.GetPixel(xFl, yFl), _image.GetPixel(xCe, yFl), xPortion);
            Color x2 = ColorUtil.Interpolate(_image.GetPixel(xFl, yCe), _image.GetPixel(xCe, yCe), xPortion);

            //then interpolate across y
            return ColorUtil.Interpolate(x1, x2, yPortion);
        }
    }
}