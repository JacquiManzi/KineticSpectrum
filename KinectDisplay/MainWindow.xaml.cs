
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using KinectAPI;
using KineticControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
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
        private readonly ByteDepthMatrix _matrix;
        //private readonly IntDepthMatrix _imgMatrix;
        private static BitmapSource _source;
        private MouseEventArgs _lastArgs;
        private static MemPointer _oldDestPointer;
        private static MemPointer _oldSrcPointer;
        private static Network _network;

        private static IBGFGDetector<Gray> _fg;
        private static BucketUpdater _buckets;

        private static IList<PDS> _PDSs;

        private static MCvFont _font = new MCvFont(FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
        private static BlobTrackerAuto<Bgr> _tracker = new BlobTrackerAuto<Bgr>();
        private static Emgu.CV.VideoSurveillance.BlobDetector _detector = new Emgu.CV.VideoSurveillance.BlobDetector(BLOB_DETECTOR_TYPE.Simple);
        private static BlobSeq _blobSeq = new BlobSeq();

        private static BlobDetector _blobDetect = new BlobDetector(480, 640, 400);
        private static ObjectTracker _blobTrack = new ObjectTracker();


        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Setting up Kinect...");
            if(DeviceLoader.Instance.Devices.Count == 0)
            {
                Console.WriteLine("No Kinect available");
                Environment.Exit(0);
            }
            _device = DeviceLoader.Instance.Devices[0];
            _device.Motor.Position = 0;//short.MaxValue/4;//*/ short.MinValue / 4;
            Console.WriteLine("Reading Camera Data...");
            //SetCamera(CameraType.DepthRgb32);
            //_source = _device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
            //((BitmapSource) camImg.Source).Changed += new EventHandler((object obj, EventArgs args) => ProcessFrame());

            Console.WriteLine("Getting Basic Depth Matrix");
            _matrix = _device.GetDepthMatrix();
            //Console.WriteLine("Loaded Color Depth Matrix");
            //_imgMatrix = _device.GetColorDepthMatrix();
           
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
            //_network.BroadCast();
            _PDSs = _network.PDSs;
            Console.WriteLine("Found {0} PDSs.", _PDSs.Count);

            Console.WriteLine("Calibrating Camera...");
            List<ColorData> colorDatas = new List<ColorData>();
            foreach(PDS pds in _PDSs)
            {
                colorDatas.AddRange(pds.AllColorData);
            }
            _buckets = new BucketUpdater(colorDatas);
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
            
            UpdateDistance();
            //var frame = new Image<Bgr, byte>(GetBitmap32(_32bitSource));
            //frame._SmoothGaussian(9); //filter out noises

            //_detector.Update(frame);
            Image<Gray, byte> depthImage = _matrix.AsImage();
            
            
            _fg.Update(depthImage);

            Image<Gray, byte> fgMask;
            fgMask = _fg.ForgroundMask;
            //fgMask = depthImage.Canny(new Gray(100), new Gray(100));
            //_buckets.UpdateBuckets(fgMask, depthImage);

            //Image<Bgr, byte> colorImg = _imgMatrix.AsImage();
            //colorImg._Dilate(5);
            //colorImg._SmoothGaussian(3);
            
            IList<Blob> blobs = _blobDetect.FindBlobs(fgMask);
            _blobTrack.Track(blobs);
           // fgMask = fgMask.CopyBlank();

            Blob.AddOutline(fgMask, blobs, _font);
            

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(CheckKeys));

            foreach(PDS pds in _PDSs)
            {
                pds.UpdateSystem();
            }

            //_network.SendUpdate(_buckets.ColorData);
            //BlobSeq newSeq = new BlobSeq();

            //_detector.DetectNewBlob(depthImage, fgMask, newSeq, _blobSeq);
            //Console.WriteLine("Blob Count: " + newSeq.Count);
            //_blobSeq = newSeq;
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

        private void CheckKeys()
        {
           Motor m = _device.Motor;
          if((Keyboard.GetKeyStates(Key.Divide) & KeyStates.Down) > 0 )
          {
              m.Position += short.MaxValue/ 50;
          }
          else if((Keyboard.GetKeyStates(Key.Multiply) & KeyStates.Down) > 0)
          {
              m.Position -= short.MaxValue / 50;
          }
          else if ((Keyboard.GetKeyStates(Key.Back) & KeyStates.Down) > 0)
          {
              return;
          }
        }
    }
}
