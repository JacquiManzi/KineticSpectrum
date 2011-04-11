using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace KinectDisplay
{
    public class ForegroundDetector<TColor> : IBGFGDetector<TColor> where TColor: struct, IColor
    {
        private readonly Image<TColor, byte> _baseImage;
        private readonly Image<Gray, byte> _foregroundMask;

        public ForegroundDetector(Image<TColor,byte> image)
        {
            _baseImage = image.Copy();
            _baseImage.SmoothGaussian(3);
            _foregroundMask = new Image<Gray, byte>(640, 480);
            byte[, ,] diff = new byte[480, 640, 1];
            _foregroundMask.Data = diff;
        }

        public void Update(Image<TColor, byte> image)
        {
            image = image.Copy();
            image.SmoothGaussian(3);
            //return image.AbsDiff(_baseImage);
            byte[, ,] data = image.Data;
            byte[, ,] basedata = _baseImage.Data;
            byte[, ,] diffData = _foregroundMask.Data;
            int height = image.Height;
            int width = image.Width;

            
            for(int i=0; i<height; i++)
            {
                for(int j=0; j<width; j++)
                {
                    byte topIntensity = data[i, j, 0];
                    if(topIntensity == 0)
                    {
                        diffData[i, j, 0] = 0;
                    }
                    else
                    {
                        if(CompareDoubles(basedata[i,j,0], topIntensity))
                        {
                            diffData[i, j,0] = byte.MinValue;
                        }
                        else
                        {
                            diffData[i, j,0] = byte.MaxValue;
                        }
                    }
                }
            }
            _foregroundMask._Erode(7);
//            img._Dilate(10);
//            img._SmoothGaussian(3);
        }

        private const double TOL = 4.0;

        public static Boolean CompareDoubles(double first, double second)
        {
            if (second == 0.0) return true;
            return Math.Abs(first - second) <= TOL;
        }

        public Image<Gray, byte> BackgroundMask
        {
            get { return _foregroundMask.Not(); }
        }

        public Image<Gray, byte> ForgroundMask
        {
            get { return _foregroundMask.Copy(); }
        }
    }
}
