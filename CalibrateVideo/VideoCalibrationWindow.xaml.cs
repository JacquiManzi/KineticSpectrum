using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KinectAPI;

namespace CalibrateVideo
{
    /// <summary>
    /// Interaction logic for VideoCalibrationWindow.xaml
    /// </summary>
    public partial class VideoCalibrationWindow : Window
    {

        private Point origin;
        private Point start;

        private Dictionary<Image, Device> _imgToDev = new Dictionary<Image, Device>();
        private Image _selectedImage;
        public VideoCalibrationWindow()
        {
            InitializeComponent();
            

            Grid grid = (Grid)Content;

            int row = 1;
            foreach (Device device in DeviceLoader.Instance.Devices)
            {
                //lets create a transform group which will handle all transformations
                TransformGroup TFG = new TransformGroup();
                ScaleTransform STF = new ScaleTransform();
                TFG.Children.Add(STF);
                RotateTransform RTF = new RotateTransform();
                TFG.Children.Add(RTF);
                TranslateTransform TTF = new TranslateTransform();
                TFG.Children.Add(TTF);
                
                //Name="img" Opacity=".5" RenderTransformOrigin="0.5,0.5" Width="406" Margin="28,32,69,62" Grid.Row="1" />
                Image img = new Image() {Opacity = .5, RenderTransformOrigin = new Point(.5, .5), Width = 300};
                grid.Children.Add(img);
                //grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(img, 1);
                img.Source = device.GetCamera(CameraType.DepthRgb32, Dispatcher);
                _selectedImage = img;
                _imgToDev.Add(img, device);


                //image.Source = device.GetCamera(CameraType.ColorRgb32, Dispatcher);
                //assign all transformations to this group
                img.RenderTransform = TFG;

                //lets add the events
                img.MouseWheel += image_MouseWheel;
                img.MouseLeftButtonDown += image_MouseLeftButtonDown;
                img.MouseLeftButtonUp += image_MouseLeftButtonUp;
                img.MouseMove += image_MouseMove;
                rslider.ValueChanged += rslider_ValueChanged;
            }

        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image) sender;
            _selectedImage = image;
            image.CaptureMouse();
            var TT = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(TT.X, TT.Y);
        }
        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            image.ReleaseMouseCapture();
            Console.WriteLine(image.RenderTransform.Value);
        }
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            if (!image.IsMouseCaptured) return;

            var TT = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector vMM = start - e.GetPosition(border);
            TT.X = origin.X - vMM.X;
            TT.Y = origin.Y - vMM.Y;
        }
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image image = (Image)sender;
            TransformGroup TFG2 = (TransformGroup)image.RenderTransform;
            ScaleTransform STF2 = (ScaleTransform)TFG2.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            STF2.ScaleX += zoom;
            STF2.ScaleY += zoom;
        }

        private void rslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            var RT = (RotateTransform)((TransformGroup)_selectedImage.RenderTransform).Children.First(tr => tr is RotateTransform);
            RT.Angle = rslider.Value;
            RT.CenterX = 0;
            RT.CenterY = 0;
        }

        private void SaveTransform(object sender, RoutedEventArgs e)
        {
            foreach(Image img in _imgToDev.Keys)
            {
                Device dev = _imgToDev[img];
                Console.WriteLine(dev.Serial);
                Matrix transform = img.RenderTransform.Value;
                Matrix scaled = new Matrix(transform.M11, transform.M12, transform.M21, transform.M22, transform.OffsetX, transform.OffsetY);
                scaled.OffsetX *= 640.0/img.ActualWidth;
                scaled.OffsetY *= 480.0/img.ActualHeight;
                Console.WriteLine(scaled);
                Console.WriteLine(img.RenderTransformOrigin);
            }

            Console.WriteLine();
        }
    }
}
