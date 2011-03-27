
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAPI;
using Point = System.Windows.Point;

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
        public MainWindow()
        {
            InitializeComponent();
            _device = DeviceLoader.Instance.Devices[0];
            _device.Motor.Position = 0;
            camImg.Source = GetSrcFromBmp();
            //_matrix = _device.GetDepthMatrix();
        }

        private BitmapSource GetSrcFromBmp()
        {
            Bitmap bmp = _device.GetBitmap(CameraType.ColorRgb32, Dispatcher);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        private void CamImgSwitch(object sender, MouseButtonEventArgs e)
        {
            if (_depthMode)
            {
                camImg.Source = GetSrcFromBmp();
                _depthMode = false;
            }
            else
            {
                camImg.Source = GetSrcFromBmp();
                _depthMode = true;
            }
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
                if (_matrix == null)
                {
                    _matrix = _device.GetDepthMatrix();
                }
                else
                {
                    _matrix = null;
                }
            }
        }
    }
}
