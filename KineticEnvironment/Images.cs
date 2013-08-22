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
        private static IDictionary<string, ImageRef> _nameMap = new Dictionary<string, ImageRef>();

        public static IList<String> GetImages()
        {
            string[] files = Directory.GetFiles(".", "*.png");
            return files.Select(file => file.Reverse().TakeWhile(c => c != '\\').Reverse().ToString()).ToList();
        }

        public static ImageRef GetImage(string imageName)
        {
            if (_nameMap.ContainsKey(imageName))
                return _nameMap[imageName];

            string[] files = Directory.GetFiles(".", "*.png");
            string fileName = files.First(s => s.EndsWith(@"\" + imageName));
            ImageRef imgRef = new ImageRef(fileName);
            _nameMap[imageName] = imgRef;
            return imgRef;
        }

    }

    public class ImageRef
    {
        private readonly string _fileName;
        private readonly Bitmap _image;

        public ImageRef(string fileName)
        {
            _fileName = fileName;
            _image = new Bitmap(fileName);
        }


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