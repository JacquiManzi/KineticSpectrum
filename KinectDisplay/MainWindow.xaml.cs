
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using KinectAPI;

namespace KinectDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device _device;
        private bool _depthMode = true;
        private DepthMatrix _matrix;
        public MainWindow()
        {
            InitializeComponent();
            _device = DeviceLoader.Instance.Devices[0];
            _device.Motor.Position = 0;
            camImg.Source = _device.GetCamera(CameraType.ColorRgb32, Dispatcher);
            _matrix = _device.GetDepthMatrix();
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
                camImg.Source = _device.GetCamera(CameraType.DepthCorrected8, Dispatcher);
                _depthMode = true;
            }
        }

        private void UpdateDistance(object sender, MouseEventArgs e)
        {
            Point drawPosition = e.GetPosition(this);
            Point matPos = e.GetPosition(camImg);
            matPos.X *= 640.0/camImg.ActualWidth;
            matPos.Y *= 480.0/camImg.ActualHeight;
            byte value = _matrix[(int)matPos.Y][(int)matPos.X];
            if (Content.ToString() != value.ToString())
            {
                ellipse1.Content = _matrix[(int) matPos.Y][(int) matPos.X];
                ellipse1.Margin = new Thickness(drawPosition.X + 5, drawPosition.Y+5, 0, 0);
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
    }
}
