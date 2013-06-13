using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace RevKitt.LightBuilder
{
    class ImageMatcher
    {
        private readonly byte[,,] _data;
        private readonly LightDetector.ColorDetector _detector;
        private readonly Image<Rgba, byte> _image; 

        public ImageMatcher(Image<Rgba,byte> image, LightDetector.ColorDetector detector )
        {
            _image = image;
            _data = image.Data;
            _detector = detector;
        }

        public Boolean IsMatch(int y, int x)
        {
            return _detector(_data[y, x, 0], _data[y, x, 0] , _data[y, x, 2]);
        }

        public int ImgHeight
        {
            get { return _image.Height; }
        }

        public int ImgWidth
        {
            get { return _image.Width; }
        }
    }
}