using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    public class ObjectTracker
    {
        private Image<Gray, byte> _lastImage;
        private int _life;


        public ObjectTracker(int width, int height)
        {
            _lastImage = new Image<Gray, byte>(width, height, new Gray(0.0));
        }

        public void Process(Image<Gray, byte> baseImg, Image<Gray, byte> maskImg)
        {
            
        }


    }

    public class Blob
    {
        private Image<Gray, byte> _blobImg;
    }
}
