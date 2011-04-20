﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using KinectAPI;
using KineticControl;
using Point = System.Windows.Point;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace KinectDisplay
{
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        static MainWindow()
        {
            string pathEnv = Environment.GetEnvironmentVariable("Path");
            Environment.SetEnvironmentVariable("Path", "lib\\OpenCV;" + pathEnv);
        }

        private readonly Device _device;
        private bool _depthMode;
        private readonly ByteDepthMatrix _matrix;
        private readonly IntDepthMatrix _imgMatrix;
        private static BitmapSource _source;
        private MouseEventArgs _lastArgs;
        private static MemPointer _oldDestPointer;
        private static MemPointer _oldSrcPointer;
        private static Network _network;

        private static IBGFGDetector<Gray> _fg;
        private static BucketUpdater _buckets;

        private static IList<PDS60ca> _PDSs;

        private static MCvFont _font = new MCvFont(FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
        private static BlobTrackerAuto<Bgr> _tracker = new BlobTrackerAuto<Bgr>();
        private static BlobDetector _detector = new BlobDetector(BLOB_DETECTOR_TYPE.Simple);
        private static BlobSeq _blobSeq = new BlobSeq();


        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Setting up Kinect...");
            _device = DeviceLoader.Instance.Devices[0];
            _device.Motor.Position = short.MaxValue/8;//*/ short.MinValue / 4;
            Console.WriteLine("Reading Camera Data...");
            SetCamera(CameraType.DepthRgb32);
            //_source = _device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
            //((BitmapSource) camImg.Source).Changed += new EventHandler((object obj, EventArgs args) => ProcessFrame());

            Console.WriteLine("Getting Basic Depth Matrix");
            _matrix = _device.GetDepthMatrix();
            Console.WriteLine("Loaded Color Depth Matrix");
            _imgMatrix = _device.GetColorDepthMatrix();
           
            CalibrateAndDispatch();
        }

        private void CalibrateAndDispatch()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Setting Up Network...");
            _network = new Network();
            Console.WriteLine("Setting Network Inerface...");
            _network.SetInterface("Local Area Connection");
            Console.WriteLine("Searching for Power Supplies...");
//            _network.FindPowerSupplies();
            _network.BroadCast();
            _PDSs = _network.PDSs;
            Console.WriteLine("Found {0} PDSs.", _PDSs.Count);

            Console.WriteLine("Calibrating Camera...");
            _buckets = new BucketUpdater(_PDSs[0].FixtureOne, _PDSs[0].FixtureTwo);
            //_fg = new ChangeTracker<Gray>();
            _fg = new ForegroundDetector<Gray>(_matrix.AsImage());
            Task frameProcessor = new Task(()=>{
                                                   Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                                                   while(true){ ProcessFrame(); }
                                               });
            frameProcessor.Start();
        }


        void ProcessFrame()
        {
            
            //UpdateDistance();
            //var frame = new Image<Bgr, byte>(GetBitmap32(_32bitSource));
            //frame._SmoothGaussian(9); //filter out noises

            //_detector.Update(frame);
            Image<Gray, byte> depthImage = _matrix.AsImage();
            
            _fg.Update(depthImage);

            Image<Gray, byte> fgMask = _fg.ForgroundMask;
            _buckets.UpdateBuckets(fgMask, depthImage);

            Image<Bgr, byte> colorImg = _imgMatrix.AsImage();
            colorImg._Dilate(5);
            //colorImg._SmoothGaussian(3);
            foreach(PDS60ca pds in _PDSs)
            {
                pds.UpdateSystem();
            }

            //_network.SendUpdate(_buckets.ColorData);
            BlobSeq newSeq = new BlobSeq();

            _detector.DetectNewBlob(depthImage, fgMask, newSeq, _blobSeq);
            //Console.WriteLine("Blob Count: " + newSeq.Count);
            _blobSeq = newSeq;
//            _tracker.Process(colorImg, fgMask);
//
//            foreach (MCvBlob blob in _tracker)
//            {
//                colorImg.Draw(Rectangle.Round(blob), new Bgr(255.0, 255.0, 255.0), 2);
//                colorImg.Draw(blob.ID.ToString(), ref _font, System.Drawing.Point.Round(blob.Center), new Bgr(255.0,255.0,255.0));
//            }    
                

            //            _lastImage = SmoothImage(_lastImage, forgroundMask);
            //            var foreground = _lastImage.Copy();
            //            foreground.SmoothGaussian(5);
                        
            //            camImg.Source = _source;//GetBitmap(frame.Bitmap);

            Dispatcher.BeginInvoke(new Action(() => UpdateFirstImage(depthImage)));
            Dispatcher.BeginInvoke(new Action(() => UpdateSecondImage(fgMask)));
            


            //            memP = GetBitmap(frame.Bitmap);
            //            camImg.Source = memP.Source;
            //            if(_oldSrcPointer !=null)
            //            {
            //                DeleteObject(_oldSrcPointer.Ptr);
            //            }
            //            _oldSrcPointer = memP;
            //Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(ProcessFrame));

        }

        private void SetCamera(CameraType type)
        {
            _source = _device.GetCamera(type, Dispatcher);
            if (type != CameraType.DepthRgb32 && type != CameraType.ColorRgb32)
            {
                if (type == CameraType.DepthCorrected12 || type == CameraType.DepthCorrected8 ||
                    type == CameraType.DepthRaw)
                {
                    camImg.Source = _device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
                }
                else
                {
                    camImg.Source = _device.GetCamera(CameraType.ColorRgb32, Dispatcher);
                }
            }
            else
            {
                camImg.Source = _source;
            }
        }

        static MemPointer GetBitmap(Bitmap bitmap)
        {
            MemPointer memP = new MemPointer();
            memP.Ptr = bitmap.GetHbitmap();
            memP.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                memP.Ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return memP;
        }

        static Bitmap GetBitmap32(BitmapSource source)
        {
            var bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        private void CamImgSwitch(object sender, MouseButtonEventArgs e)
        {
            if (_depthMode)
            {
                SetCamera(CameraType.ColorRgb32);
                camImg.Source = _source; // _device.GetCamera(CameraType.ColorRgb32, Dispatcher);
                _depthMode = false;
            }
            else
            {
                SetCamera(CameraType.DepthCorrected8);
                camImg.Source = _source;// _device.GetCamera(CameraType.DepthRgb32, Dispatcher);
                _depthMode = true;
            }
        }


        private void UpdateSecondImage(Image<Gray, byte> fg)
        {
            MemPointer memP = GetBitmap(fg.Bitmap);
            jaqinetik.Source = memP.Source;
            if (_oldDestPointer != null)
            {
                DeleteObject(_oldDestPointer.Ptr);
            }
            _oldDestPointer = memP;
        }

        private void UpdateFirstImage(Image<Gray, byte> fg)
        {
            MemPointer memP = GetBitmap(fg.Bitmap);
            camImg.Source = memP.Source;
            if (_oldSrcPointer != null)
            {
               DeleteObject(_oldSrcPointer.Ptr);
            }
            _oldSrcPointer = memP;
        }


        private static Image<Gray, byte> SmoothImage(Image<Gray, byte> lastImage, Image<Gray, byte> nextImage)
        {
            if(lastImage == null) return nextImage;
            if(lastImage.Height != nextImage.Height || lastImage.Width != nextImage.Width )
                throw new ArgumentException("Image sized don't match");

            //byte[,,] lastBytes = lastImage.GetObjectData()
            //byte[,,] newBytes  = new byte[lastImage.Height,lastImage.Width,0];
            Image<Gray, byte> newImage = new Image<Gray, byte>(lastImage.Width,lastImage.Height);

            for(int i=0; i<lastImage.Height; i++)
            {
                for(int j = 0; j<lastImage.Width; j++)
                {
                    Gray val = nextImage[i, j];
                    newImage[i, j] = (val.Intensity==0.0 ? lastImage[i, j] : val);
                }
            }

            return newImage;
        }


        private void UpdateDistance()
        {
           if(_lastArgs != null) UpdateDistance(null, _lastArgs); 
        }

        private void UpdateDistance(object sender, MouseEventArgs e)
        {
            if (_matrix != null)
            {
                Point drawPosition = e.GetPosition(this);
                Point matPos = e.GetPosition(camImg);

                matPos.X *= 640.0/camImg.ActualWidth;
                matPos.Y *= 480.0/camImg.ActualHeight;
                float value = _matrix[(int) matPos.Y][(int) matPos.X];
                if (Content.ToString() != value.ToString())
                {
                    ellipse1.Content = value;
                    ellipse1.Margin = new Thickness(drawPosition.X + 5, drawPosition.Y + 5, 0, 0);
                }
            }
            _lastArgs = e;
        }

        private void OnEnter(object sender, MouseEventArgs e)
        {
            ellipse1.Visibility = Visibility.Visible;
        }

        private void OnExit(object sender, MouseEventArgs e)
        {
            ellipse1.Visibility = Visibility.Hidden;
        }
    }

    internal class MemPointer
    {
        internal BitmapSource Source;
        internal IntPtr Ptr;
    }

    
}
