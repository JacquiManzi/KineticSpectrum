using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace KinectDisplay
{
    public class ForegroundDetector<TColor> : IBGFGDetector<TColor> where TColor: struct, IColor
    {
        //private Image<TColor, byte> _baseImage;
        private readonly Background _backGround;
        private readonly Image<TColor, byte> _lastImage;
        private readonly Image<Gray, byte> _foregroundMask;
        private DateTime _resetTime;

        public ForegroundDetector(Image<TColor,byte> image)
        {
            //_baseImage = image.Copy();
            _backGround = new Background(image.Width, image.Height);
            _backGround.Reset(image);
            _lastImage = image.Copy();
            _foregroundMask = new Image<Gray, byte>(640, 480);
            byte[, ,] diff = new byte[480, 640, 1];
            _foregroundMask.Data = diff;
        }

        public void Update(Image<TColor, byte> image)
        {
            byte[, ,] data = image.Data;
            byte[, ,] diffData = _foregroundMask.Data;

            int height = image.Height;
            int width = image.Width;

            double numMatchingBase = 1, numMatchingLast = 1;
            double numDiffBase = 0, numDiffLast = 0;
            
            for(int i=0; i<height; i++)
            {
                for(int j=0; j<width; j++)
                {
                    byte topIntensity = data[i, j, 0];
                    diffData[i, j, 0] = (byte) _backGround.Update(topIntensity, j, i);
                }
            }
            //Console.WriteLine("Last image differences: {0}%", numDiffLast/(numDiffLast + numMatchingLast));
            //Console.WriteLine("Base image differences: {0}%", numDiffBase/(numDiffBase + numMatchingBase));
            if(numDiffBase/(numDiffBase + numMatchingBase) > .70 &&
                numDiffLast/(numDiffLast + numMatchingLast) > .60)
            {
                _resetTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, 250);
                //_baseImage = image;
                Console.WriteLine("Resetting base");
            }
               
            
            _foregroundMask._Erode(6);
            _foregroundMask._Dilate(7);
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
