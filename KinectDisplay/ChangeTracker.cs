using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using KineticControl;

namespace KinectDisplay
{
    class ChangeTracker
    {
        private readonly ColorData _colorState;
        //public int[] _lastUpdated;

        private readonly Image<Gray, byte> _lastImage;

        private readonly DateTime _lastUpdate;

        private readonly Image<Gray, byte> _diffImg;

        private static readonly Gray Black = new Gray(0.0);
        private static readonly Gray White = new Gray(256.0);

        public ChangeTracker(ColorData colorData)
        {
            _colorState = colorData;
            for (int i = 0; i < colorData.Count; i++ )
            {
                colorData[i] = Colors.Black;
            }
            //_lastUpdated = new int[colorData.Count];
            _lastUpdate = DateTime.Now;
            _lastImage = null;
            _diffImg = new Image<Gray, byte>(640,480);
        }

        public Image<Gray, byte> UpdateImage(Image<Gray, byte> image)
        {
            UpdateColors();
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Gray baseGray = _lastImage[i, j];
                    Gray topGray = image[i, j];
                    double topIntensity = topGray.Intensity;
                    if (topGray.Intensity == 0.0 || baseGray.Intensity == 0)
                    {
                        _diffImg[i, j] = Black;
                    }
                    else
                    {
                        double baseIntensity = baseGray.Intensity;
                        if (ForegroundDetector.CompareDoubles(baseIntensity, topIntensity))
                        {
                            _diffImg[i, j] = Black;
                        }
                        else
                        {
                            _diffImg[i, j] = White;
                        }
                    }
                }
            }
            _diffImg.Erode(3);

            for (int i = 0; i < _diffImg.Height; i++ )
            {
                for(int j =0; j < _diffImg.Width; j++)
                {
                    if(_diffImg[i,j].Intensity == 256.0)
                    {
                        UpdateBucket(image[i, j].Intensity);
                    }
                }
            }

            return _diffImg;
        }

        private void UpdateColors()
        {
            TimeSpan delta = DateTime.Now - _lastUpdate;
            double adjfactor = Math.Pow(2, delta.Seconds);

            for (int i = 0; i < _colorState.Count; i++)
            {
                Color color = _colorState[i];
                color.R = (byte) (color.R/adjfactor);
                color.G = (byte) (color.G/adjfactor);
                color.B = (byte) (color.B/adjfactor);
                _colorState[i] = color;
            }
        }

        private void UpdateBucket(double intensity)
        {
            int pos = (int)Math.Floor(intensity/250.0*49);
            _colorState[pos] = Colors.Red;
        }

        public ColorData ColorData { get { return _colorState; } }
    }
}
