using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    public class ForegroundDetector
    {
        private Image<Gray, byte> _baseImage;

        private static readonly Gray Black = new Gray(0.0);
        private static readonly Gray White = new Gray(256.0);
        
        public ForegroundDetector(Image<Gray,byte> image)
        {
            _baseImage = image.Copy();
            _baseImage.SmoothGaussian(3);
        }

        public Image<Gray, byte> buildMask(Image<Gray, byte> image)
        {
            image = image.Copy();
            image.SmoothGaussian(3);
            //return image.AbsDiff(_baseImage);

            Image<Gray,byte> img = new Image<Gray, byte>(image.Width, image.Height);
            for(int i=0; i<image.Height; i++)
            {
                for(int j=0; j<image.Width; j++)
                {
                    Gray baseGray = _baseImage[i, j];
                    Gray topGray = image[i, j];
                    double topIntensity = topGray.Intensity;
                    if(topGray.Intensity == 0.0)
                    {
                        img[i, j] = Black;
                    }
                    else
                    {
                        double baseIntensity = baseGray.Intensity;
                        if(CompareDoubles(baseIntensity, topIntensity))
                        {
                            img[i, j] = Black;
                        }
                        else
                        {
                            img[i, j] = White;
                        }
                    }
                }
            }
            img._Erode(6);
            img._Dilate(10);
            img._SmoothGaussian(3);
            return img;

        }

        private const double TOL = 3.0;

        public static Boolean CompareDoubles(double first, double second)
        {
            return Math.Abs(first - second) <= TOL;
        }
    }
}
