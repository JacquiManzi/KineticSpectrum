using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace KinectDisplay
{
    public class ForegroundDetector<TColor> : IBGFGDetector<TColor> where TColor: struct, IColor
    {
        private Image<TColor, byte> _baseImage;
        private readonly Image<TColor, byte> _lastImage;
        private readonly Image<Gray, byte> _foregroundMask;
        private DateTime _resetTime;

        public ForegroundDetector(Image<TColor,byte> image)
        {
            _baseImage = image.Copy();
            _lastImage = image.Copy();
            _foregroundMask = new Image<Gray, byte>(640, 480);
            byte[, ,] diff = new byte[480, 640, 1];
            _foregroundMask.Data = diff;
            UpdateBase();

//
//            Color colorState;
//            colorState.R = (byte) (colorState.R/adjfactor);
//
//
//            for()
//            {
//                _colorData[i] = colorState;
//            }
        }

        private void UpdateBase()
        {
            byte[,,] data = _baseImage.Data;
            int height = _baseImage.Height;
            int width = _baseImage.Width;

            for(int i = 0; i < height; i++)
                for(int j=0; j<width; j++)
                {
                    if(data[i,j,0] < 50)
                    {
                        data[i, j, 0] = 0;
                    }
                }
        }

        public void Update(Image<TColor, byte> image)
        {
            if( _resetTime < DateTime.Now)
            {
                _baseImage = image;
                _resetTime = DateTime.MaxValue;
            }
            //image._SmoothGaussian(3);
            //return image.AbsDiff(_baseImage);
            byte[, ,] data = image.Data;
            byte[, ,] basedata = _baseImage.Data;
            byte[, ,] diffData = _foregroundMask.Data;
            byte[, ,] lastData = _lastImage.Data;

            int height = image.Height;
            int width = image.Width;

            double numMatchingBase = 0, numMatchingLast = 0;
            double numDiffBase = 0, numDiffLast = 0;
            
            for(int i=0; i<height; i++)
            {
                    for(int j=0; j<width; j++)
                    {
                        int topIntensity = data[i, j, 0];
                        if(topIntensity == 0)
                        {
                            diffData[i, j, 0] = 0;
                        }
                        else
                        {
                            int baseIntensity = basedata[i, j, 0]; 
                            if( CompareDoubles(baseIntensity, topIntensity, TOL))
                            {
                                diffData[i, j, 0] = byte.MinValue;
                            }
                            else
                            {
                                diffData[i, j, 0] = byte.MaxValue;
                            }
                        if( baseIntensity > 10 && topIntensity - baseIntensity > TOL)
                        {
                            basedata[i, j, 0] = (byte)topIntensity;
                        }
                        int lastIntensity = lastData[i, j, 0];
                        if(lastIntensity > 10 && lastIntensity == topIntensity )
                        {
                            numMatchingLast++;
                        }
                        else if(lastIntensity > 10)
                        {
                            numDiffLast++;
                        }
                        if(baseIntensity >10 && baseIntensity == topIntensity)
                        {
                            numMatchingBase++;
                        }
                        else if(baseIntensity > 10)
                        {
                            numDiffBase++;
                        }

                    }
                }
            }
            //Console.WriteLine("Last image differences: {0}%", numDiffLast/(numDiffLast + numMatchingLast));
            //Console.WriteLine("Base image differences: {0}%", numDiffBase/(numDiffBase + numMatchingBase));
            if(numDiffBase/(numDiffBase + numMatchingBase) > .70 &&
                numDiffLast/(numDiffLast + numMatchingLast) > .60)
            {
                _resetTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, 250);
                _baseImage = image;
                Console.WriteLine("Resetting base");
            }
               
            
            _foregroundMask._Erode(6);
//            _foregroundMask._Dilate(6);
//            img._Dilate(10);
//            img._SmoothGaussian(3);
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
            get { return _baseImage.Copy(); }
        }
    }
}
