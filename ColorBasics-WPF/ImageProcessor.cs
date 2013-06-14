﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using KinectDisplay;
using KineticControl;
using Color = System.Windows.Media.Color;

namespace RevKitt.LightBuilder
{
    class ImageProcessor
    {
        private readonly int           _width;
        private readonly int           _height;
        private readonly LightDetector _lightDetector;
        private LightMatcher  _lightMatcher;
        private readonly LightSystem   _lightSystem;
        private readonly MCvFont       _font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.3, 0.3);

        public ImageProcessor(LightSystem lightSystem, int width, int height)
        {
            _width = width;
            _height = height;
            _lightDetector = new LightDetector(height, width, 5);
            _lightSystem = lightSystem;
            Lights = new List<Light>();
        }

        public Bitmap ProcessImage(byte[] colorPixels)
        {
            var image = new Image<Rgba, byte>(_width, _height);
            image.Bytes = colorPixels;

            Mask = _lightDetector.Mask(image);
            IList<Blob> blobs = _lightDetector.FindBlobs(Mask, image);
            if (_lightMatcher != null)
            {
                Lights = _lightMatcher.UpdateLights(blobs, image).AsReadOnly();
                Blob.AddOutline(image, Lights.Select(light => light.LightBlob), _font);
            }
            else
            {
                NameBlobs(blobs);
                Blob.AddOutline(image, blobs, _font);
            }
            return image.Bitmap;
        }

        public Image<Gray, byte> Mask {  get; private set; }
        public IList<Light> Lights { get; private set; } 

        public void Click(int x, int y)
        {
            if (IsSearching) return;

            Light light = Lights.FirstOrDefault(l => l.LightBlob.ContainsPoint(x, y));
            if(light != null && !light.IsUnknown)
            {
                Color color = light.SensedOn ? Colors.Black : Colors.White;
                _lightSystem[light.Address] = color;
            }
        }

        public bool IsSearching
        {
            get { return _lightMatcher != null && _lightMatcher.IsSearching; }
        }

        public void BeginSearch()
        {
            _lightMatcher = new LightMatcher(_lightSystem);
            _lightMatcher.BeginSearch();
        }


        private void NameBlobs(IList<Blob> blobs )
        {
            int i = 1;
            foreach (var blob in blobs)
            {
                blob.Name = "" + i++;
            }
        }

        private Image<Rgba, byte> BuildImage(byte[] colorPixels)
        {
            byte[, ,] imgPix = new byte[_height, _width, 4];
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++ )
                {
                    int pos = y*_width*4 + x*4;
                    imgPix[y, x, 0] = colorPixels[pos];
                    imgPix[y, x, 1] = colorPixels[pos + 1];
                    imgPix[y, x, 2] = colorPixels[pos + 2];
                    imgPix[y, x, 3] = colorPixels[pos + 3];
                }

            return new Image<Rgba, byte>(imgPix);
        }
    }
}
