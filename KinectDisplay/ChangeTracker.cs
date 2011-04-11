using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace KinectDisplay
{
    class ChangeTracker<TColor> : IBGFGDetector<TColor> where TColor : struct, IColor
    {
        private Image<TColor, byte> _lastImage;
        
        private readonly Image<Gray, byte> _diffImg;

        private static readonly Gray Black = new Gray(0.0);
        private static readonly Gray White = new Gray(255.0);

        public ChangeTracker()
        {
            _lastImage = null;
            _diffImg = new Image<Gray, byte>(640,480);
            byte[,,] diff = new byte[480,640,1];
            _diffImg.Data = diff;
        }

        public void Update(Image<TColor, byte> image)
        {
            if (_lastImage == null)
            {
                _lastImage = image.Copy();
            }

            byte[,,] data = image.Data;
            byte[,,] lastdata = _lastImage.Data;
            byte[,,] diffData = _diffImg.Data;
            int height = image.Height;
            int width = image.Width;
            

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte baseGray = lastdata[i, j, 0];
                    byte topGray = data[i, j, 0];

                    if (topGray == 0 || baseGray == 0)
                    {
                        diffData[i, j, 0] = 0;
                    }
                    else
                    {
                        if (ForegroundDetector<TColor>.CompareDoubles(baseGray, topGray, 2.0))
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
            _diffImg._Erode(3);
            _lastImage = image.Copy();
        }

        public Image<Gray, byte> ForgroundMask
        {
            get { return _diffImg.Copy(); }
        }

        public Image<Gray, byte> BackgroundMask
        {
            get { return _diffImg.Not(); }
        }
    }
}
