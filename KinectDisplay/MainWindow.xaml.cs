
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using KinectAPI;
using Point = System.Windows.Point;
using System.Timers;

namespace KinectDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device _device;
        private bool _depthMode = false;
        private DepthMatrix _matrix;
        private static BlobTrackerAuto<Bgr> _tracker;
        private static IBGFGDetector<Bgr> _detector;
        private static MCvFont _font = new MCvFont(FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
        private Timer _timer;
        private static BitmapSource _source;

        public MainWindow()
        {
            InitializeComponent();
            _device = DeviceLoader.Instance.Devices[0];
            _device.Motor.Position = 0;
            _source = _device.GetCamera(CameraType.DepthRgb32, Dispatcher);
            //((BitmapSource) camImg.Source).Changed += new EventHandler((object obj, EventArgs args) => ProcessFrame());

            _timer = new Timer { Interval = 500 };
            _timer.Elapsed += (s, e) => Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(ProcessFrame));
            _timer.Enabled = true;

            _detector = new FGDetector<Bgr>(FORGROUND_DETECTOR_TYPE.FGD);
            _tracker = new BlobTrackerAuto<Bgr>();

            //Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(ProcessFrame));
        }

        static BitmapSource GetBitmap(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        static Bitmap GetBitmap(BitmapSource source)
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
                camImg.Source = _device.GetCamera(CameraType.ColorRgb32, Dispatcher);
                _depthMode = false;
            }
            else
            {
                camImg.Source = _device.GetCamera(CameraType.DepthRgb32, Dispatcher);
                _depthMode = true;
            }
        }
        
        void ProcessFrame()
        {
            var frame = new Image<Bgr, byte>(GetBitmap(_source));
            frame._SmoothGaussian(3); //filter out noises

            _detector.Update(frame);
            Image<Gray, Byte> forgroundMask = _detector.ForgroundMask;

            _tracker.Process(frame, forgroundMask);

            foreach (MCvBlob blob in _tracker)
            {
                frame.Draw(Rectangle.Round(blob), new Bgr(255.0, 255.0, 255.0), 2);
                frame.Draw(blob.ID.ToString(), ref _font, System.Drawing.Point.Round(blob.Center), new Bgr(255.0, 255.0, 255.0));
            }

            camImg.Source = GetBitmap(frame.Bitmap);
            jaqinetik.Source = GetBitmap(forgroundMask.Bitmap);

        }

        private void UpdateDistance(object sender, MouseEventArgs e)
        {
            if (_matrix != null)
            {
                Point drawPosition = e.GetPosition(this);
                Point matPos = e.GetPosition(camImg);
                matPos.X *= 640.0/camImg.ActualWidth;
                matPos.Y *= 480.0/camImg.ActualHeight;
                byte value = _matrix[(int) matPos.Y][(int) matPos.X];
                if (Content.ToString() != value.ToString())
                {
                    ellipse1.Content = value;
                    ellipse1.Margin = new Thickness(drawPosition.X + 5, drawPosition.Y + 5, 0, 0);
                }
            }
        }

        private void OnEnter(object sender, MouseEventArgs e)
        {
            ellipse1.Visibility = Visibility.Visible;
        }

        private void OnExit(object sender, MouseEventArgs e)
        {
            ellipse1.Visibility = Visibility.Hidden;
        }

        private void Grid_KeyDown(object sender, MouseEventArgs e)
        {
           
            lock (this)
            {
                _matrix = _matrix == null ? _device.GetDepthMatrix() : null;
            }
        }
    }
}
