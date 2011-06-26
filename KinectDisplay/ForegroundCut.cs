using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace KinectDisplay
{
    public class ForegroundCut<TColor> : IBGFGDetector<TColor> where TColor: struct, IColor
    {
        //private Image<TColor, byte> _baseImage;
        private readonly Image<Gray, byte> _foregroundMask;
        private readonly byte _threash;

        public ForegroundCut(byte threashold)
        {
            _threash = threashold;
            byte[, ,] diff = new byte[480, 640, 1];
            _foregroundMask = new Image<Gray, byte>(diff);
        }

        public void Update(Image<TColor, byte> image)
        {

            byte[, ,] data = image.Data;
            
            byte[, ,] diffData = _foregroundMask.Data;

            int height = image.Height;
            int width = image.Width;
            
            for(int i=0; i<height; i++)
            {
                for(int j=0; j<width; j++)
                {
                    byte val = data[i, j, 0];

                   if(val < _threash && val > 40)
                   {
                       diffData[i, j, 0] = 255;
                   }
                   else
                   {
                       diffData[i, j, 0] = 0;
                   }

                }
                
            }
               
            
//            _foregroundMask._Erode(6);
//            _foregroundMask._Dilate(8);
        }

        private const double TOL = 4.0;

        public static Boolean CompareDoubles(double baseI, double topI, double tol)
        {
            if (topI == 0.0) return true;
            return Math.Abs(baseI - topI) <= tol;
        }

        public Image<Gray, byte> BackgroundMask
        {
            get { return _foregroundMask.Not(); }
        }

        public Image<Gray, byte> ForgroundMask
        {
            get { return _foregroundMask.Copy(); }
        }

        public Image<TColor, byte> BaseImage
        {
            get { return null; }
        }
    }
}
