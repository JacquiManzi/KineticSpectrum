using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public partial class KinectPanel : UserControl
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        static KinectPanel()
        {
            string pathEnv = Environment.GetEnvironmentVariable("Path");
            Environment.SetEnvironmentVariable("Path", "lib\\OpenCV;" + pathEnv);
        }

        public Device Device;
        public MCvFont Font;
        public int Position = 0;

        private ByteDepthMatrix _matrix;
        //private readonly IntDepthMatrix _imgMatrix;
        private BitmapSource _source;
        private MouseEventArgs _lastArgs;
        private MemPointer _oldDestPointer;
        private MemPointer _oldSrcPointer;

        private IBGFGDetector<Gray> _fg;

        private BlobDetector _blobDetect = new BlobDetector(480, 640, 400);
        private ObjectTracker _blobTrack = new ObjectTracker();


        public KinectPanel()
        {
            InitializeComponent();
        }

        internal void CalibrateAndDispatch()
        {
            Device.Motor.Position = 0;//short.MaxValue/4;//*/ short.MinValue / 4;
            Console.WriteLine("Reading Camera Data...");
            //SetCamera(CameraType.DepthRgb32);
            //_source = Device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
            //((BitmapSource) camImg.Source).Changed += new EventHandler((object obj, EventArgs args) => ProcessFrame());

            Console.WriteLine("Getting Basic Depth Matrix");
            processing = true;
            _matrix = Device.GetDepthMatrix(Update);
            Thread.Sleep(2000);

            //_fg = new ForegroundDetector<Gray>(_matrix.AsImage());
            _fg = new ForegroundCut<Gray>(210);
            processing = false;
//            Task frameProcessor = new Task(()=>{
//                                                   Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
//                                                   while(true){ ProcessFrame(); }
//                                               });
//            frameProcessor.Start();
        }

        void Update()
        {
            if (!processing)
                new Task(ProcessFrame).Start();
        }

        private volatile Boolean processing = false;
        void ProcessFrame()
        {
            processing = true;
            //UpdateDistance();
            //var frame = new Image<Bgr, byte>(GetBitmap32(_32bitSource));
            //frame._SmoothGaussian(9); //filter out noises

            //_detector.Update(frame);
            Image<Gray, byte> depthImage = _matrix.AsImage();
            
            
            _fg.Update(depthImage);

            Image<Gray, byte> fgMask;
            //fgMask = depthImage;
            fgMask = _fg.ForgroundMask;
            //fgMask = depthImage.Canny(new Gray(100), new Gray(100));
            //_buckets.UpdateBuckets(fgMask, depthImage);

            //Image<Bgr, byte> colorImg = _imgMatrix.AsImage();
            //colorImg._Dilate(5);
            //colorImg._SmoothGaussian(3);
            
            IList<Blob> blobs = _blobDetect.FindBlobs(fgMask);
            _blobTrack.Track(blobs);
           // fgMask = fgMask.CopyBlank();

            Blob.AddOutline(fgMask, blobs, Font);
            

            //Dispatcher.Invoke(DispatcherPriority.Normal, new Action(CheckKeys));

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

            Dispatcher.Invoke(new Action(() => UpdateFirstImage(depthImage)));
            Dispatcher.Invoke(new Action(() => UpdateSecondImage(fgMask)));
            processing = false;
        }

        private void SetCamera(CameraType type)
        {
            _source = Device.GetCamera(type, Dispatcher);
            if (type != CameraType.DepthRgb32 && type != CameraType.ColorRgb32)
            {
                if (type == CameraType.DepthCorrected12 || type == CameraType.DepthCorrected8 ||
                    type == CameraType.DepthRaw)
                {
                    camImg.Source = Device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
                }
                else
                {
                    camImg.Source = Device.GetCamera(CameraType.ColorRgb32, Dispatcher);
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
           Motor m = Device.Motor;
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

    internal class MemPointer
    {
        internal BitmapSource Source;
        internal IntPtr Ptr;
    }
}
