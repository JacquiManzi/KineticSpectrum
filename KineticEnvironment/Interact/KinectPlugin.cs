using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Kinect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Interact
{
    public class KinectPlugin
    {
        private readonly Dispatcher _dispatcher;
        private KinectSensor _sensor;
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        private bool _enabled;
        private DepthImagePixel[] _depthPixels;
        private byte[] _imagePixels;

        private int _width, _height;

        public KinectPlugin()
        {
            FallOff = .95;
        }

        
        private void Setup()
        {
            lock (this)
            {
                if(_sensor != null)
                    return;

                foreach (var potentialSensor in KinectSensor.KinectSensors)
                {
                    if (potentialSensor.Status == KinectStatus.Connected)
                    {
                        _sensor = potentialSensor;
                        System.Diagnostics.Debug.WriteLine("Sensor Detected");
                        break;
                    }
                }
                if (_sensor != null)
                {
                    _sensor.DepthStream.Enable(DepthFormat);
                    _depthPixels = new DepthImagePixel[_sensor.DepthStream.FramePixelDataLength];
                    _imagePixels = new byte[_sensor.DepthStream.FramePixelDataLength];
                    _width = _sensor.DepthStream.FrameWidth;
                    _height = _sensor.DepthStream.FrameHeight;
                    _sensor.DepthFrameReady += SensorOnDepthFrameReady;
                    try
                    {
                        _sensor.Start();
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Error Starting Kinect");
                    }
                    System.Diagnostics.Debug.WriteLine("Sensor successfully started");
                }
            }
        }

        private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(_depthPixels);
 
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    int colorIndex = 0;
                    for (int i = 0; i < _depthPixels.Count(); i++)
                    {
                        _imagePixels[i] = (byte)(FallOff*_imagePixels[i]);
                        if (_imagePixels[i] < 10)
                            _imagePixels[i] = 0;
                        short depth = _depthPixels[i].Depth;
                        if (depth > minDepth && depth < maxDepth)
                        {
                            _imagePixels[i] = 255;
//                            _colorPixels[colorIndex++] = _imagePixels[i];
//                            _colorPixels[colorIndex++] = _imagePixels[i];
//                            _colorPixels[colorIndex++] = _imagePixels[i];
//                            colorIndex++;
                        }
                    }
                    // Write the pixel data into our bitmap
//                    _colorBitmap.WritePixels(
//                        new Int32Rect(0, 0, _colorBitmap.PixelWidth, _colorBitmap.PixelHeight),
//                        _colorPixels,
//                        _colorBitmap.PixelWidth * sizeof(int),
//                        0);
                }
            }
        }

        public bool HasKinect
        {
            get
            {
                if (_sensor != null)
                    return true;
                Setup();
                return _sensor != null;
            }
        }


        public bool Enabled { get { return _enabled && HasKinect; } set { _enabled = value; } }

        public double XMax { get; set; }
        public double XMin { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }

        public double FallOff { get; set; }

        public byte Applies(double x, double y)
        {
            x = (x - XMin)/(XMax - XMin);
            y = (y - YMin)/(YMax - YMin);
            if (!Enabled)
                return 0;

            int xpos = (int)(x*(_width - 1));
            int ypos = (int) ((1 - y)*(_height - 1));

            return _imagePixels[ypos*_width + xpos];
        }
    }
}
