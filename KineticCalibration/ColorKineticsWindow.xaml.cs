using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using KinectAPI;
using KineticControl;

namespace KineticCalibration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ColorKineticsWindow : Window
    {
        private PDS60ca _selectedPds;
        private Image _selectedImage;

        private Point _origin;
        private Point _start;

        private LedGroup LedGroup1;
        private LedGroup LedGroup2;

        private Timer _timer;

        private readonly Dictionary<Image, Device> _imgToDev;

        public ColorKineticsWindow()
        {
            _imgToDev = new Dictionary<Image, Device>();
            InitializeComponent();

            foreach (LightType lType in LightType.Types)
            {
                Fixture1.Items.Add(lType);
                Fixture2.Items.Add(lType);
            }
            InitializeVideo();
            //Grid grid = (Grid)Content;
            //PDS60ca pds = new PDS60ca();
            //pds.SetColorData1(LightType.Short);
            //LedGroup gr = new LedGroup(pds.Data1.Leds,grid, Border,1,_imgToDev.Keys);
            _timer = new Timer(1000/30);
            _timer.Elapsed += (s,e)=>UpdateLights();
            _timer.Enabled = true;

        }

        private void UpdateLights()
        {
           foreach(PDS60ca pds in PowerSupplies.Items )
           {
               try
               {
                   if(pds.EndPoint.Address != System.Net.IPAddress.Any)
                        pds.UpdateSystem();
               }
               catch(SocketException se){}
           }
        }

        private void ResizeHandler()
        {
            
        }

        private void InitializeVideo()
        {
            Grid grid = (Grid)Content;

            foreach (Device device in DeviceLoader.Instance.Devices)
            {
                //lets create a transform group which will handle all transformations
                TransformGroup TFG = new TransformGroup();
//                ScaleTransform STF = new ScaleTransform();
//                TFG.Children.Add(STF);
                RotateTransform RTF = new RotateTransform();
                TFG.Children.Add(RTF);
                TranslateTransform TTF = new TranslateTransform();
                TFG.Children.Add(TTF);

                //Name="img" Opacity=".5" RenderTransformOrigin="0.5,0.5" Width="406" Margin="28,32,69,62" Grid.Row="1" />
                Image img = new Image { Opacity = .5, RenderTransformOrigin = new Point(.5, .5), Width = 300 };
                grid.Children.Add(img);
                //grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(img, 1);
                Grid.SetColumn(img, 3);
                img.Source = device.GetCamera(CameraType.DepthRgb32, Dispatcher);
                _selectedImage = img;
                _imgToDev.Add(img, device);


                //image.Source = device.GetCamera(CameraType.ColorRgb32, Dispatcher);
                //assign all transformations to this group
                img.RenderTransform = TFG;

                //lets add the events
                //img.MouseWheel += image_MouseWheel;
                img.MouseLeftButtonDown += ImageMouseLeftButtonDown;
                img.MouseLeftButtonUp += ImageMouseLeftButtonUp;
                img.MouseMove += ImageMouseMove;
                rslider.ValueChanged += RSliderValueChanged;
            }
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            _selectedImage = image;
            image.CaptureMouse();
            var TT = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            _start = e.GetPosition(Border);
            _origin = new Point(TT.X, TT.Y);
        }

        private void ImageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            image.ReleaseMouseCapture();
            Console.WriteLine(image.RenderTransform.Value);
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            if (!image.IsMouseCaptured) return;

            var TT = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector vMM = _start - e.GetPosition(Border);
            TT.X = _origin.X - vMM.X;
            TT.Y = _origin.Y - vMM.Y;
        }
//        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
//        {
//            Image image = (Image)sender;
//            TransformGroup TFG2 = (TransformGroup)image.RenderTransform;
//            ScaleTransform STF2 = (ScaleTransform)TFG2.Children[0];
//
//            double zoom = e.Delta > 0 ? .2 : -.2;
//            STF2.ScaleX += zoom;
//            STF2.ScaleY += zoom;
//        }

        private void RSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            var RT = (RotateTransform)((TransformGroup)_selectedImage.RenderTransform).Children.First(tr => tr is RotateTransform);
            RT.Angle = rslider.Value;
            RT.CenterX = 0;
            RT.CenterY = 0;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _selectedPds = (PDS60ca) e.AddedItems[0];
                IPAddress.IsEnabled = Fixture1.IsEnabled = Fixture2.IsEnabled = RemoveButton.IsEnabled = true;
                IPAddress.Text = _selectedPds.EndPoint==null?"":_selectedPds.EndPoint.Address.ToString();

                if (!Fixture1.Items.Contains(_selectedPds.Data1.LightType))
                    Fixture1.Items.Add(_selectedPds.Data1.LightType);
                if (!Fixture2.Items.Contains(_selectedPds.Data2.LightType))
                    Fixture2.Items.Add(_selectedPds.Data2.LightType);
                
                Fixture1.SelectedItem = _selectedPds.Data1.LightType;
                Fixture2.SelectedItem = _selectedPds.Data2.LightType;
            }
            else
            {
                IPAddress.IsEnabled = Fixture1.IsEnabled = Fixture2.IsEnabled = RemoveButton.IsEnabled = false;
                IPAddress.Text = "";
            }
        }

        private void PdsAddressChanged(object sender, RoutedEventArgs e)
        {
            IPAddress ipAddress;
            if (System.Net.IPAddress.TryParse(IPAddress.Text, out ipAddress))
            {
                _selectedPds.EndPoint = new IPEndPoint(ipAddress, _selectedPds.EndPoint.Port);
                PowerSupplies.Items.Refresh();
//                int index = PowerSupplies.Items.IndexOf(_selectedPds);
//                PowerSupplies.Items.Remove(_selectedPds);
//                PowerSupplies.Items.Insert(index, _selectedPds);
//                PowerSupplies.SelectedItem = _selectedPds;
            }
        }

        private void PdsFixtureChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == Fixture1)
            {
                if (_selectedPds.Data1.LightType != (LightType)Fixture1.SelectedItem)
                {
                    _selectedPds.SetColorData1((LightType) Fixture1.SelectedItem);
                }
                if (LedGroup1 != null)
                    LedGroup1.CleanUp((Grid)Content);
                LedGroup1 = new LedGroup(_selectedPds.Data1.Leds,(Grid)Content, Border, 0, _imgToDev.Keys);
            }

            if (sender == Fixture2)
            {
                if (_selectedPds.Data2.LightType != (LightType)Fixture2.SelectedItem)
                {
                    _selectedPds.SetColorData2((LightType) Fixture1.SelectedItem);
                }
                if (LedGroup2 != null)
                    LedGroup2.CleanUp((Grid) Content);
                LedGroup2 = new LedGroup(_selectedPds.Data1.Leds, (Grid)Content, Border, 20, _imgToDev.Keys);
            }
        }

        private void AddPds(object sender, RoutedEventArgs e)
        {
            _selectedPds = new PDS60ca();
            PowerSupplies.Items.Add(_selectedPds);
            PowerSupplies.SelectedItem = _selectedPds;
        }

        private void RemovePds(object sender, RoutedEventArgs e)
        {
            PowerSupplies.Items.Remove(_selectedPds);
            if (PowerSupplies.Items.Count > 0)
                PowerSupplies.SelectedIndex = 0;
        }

        private void SaveTransform(object sender, RoutedEventArgs e)
        {
            //Save Camera Settings:
            Dictionary<string, Matrix> serToTransform = new Dictionary<string, Matrix>();
            foreach (Image img in _imgToDev.Keys)
            {
                Device dev = _imgToDev[img];
                Console.WriteLine(dev.Serial);
                Matrix transform = img.RenderTransform.Value;
                Matrix scaled = new Matrix(transform.M11, transform.M12, transform.M21, transform.M22, transform.OffsetX, transform.OffsetY);
                scaled.OffsetX *= 640.0 / img.ActualWidth;
                scaled.OffsetY *= 480.0/img.ActualHeight;
                serToTransform.Add(dev.Serial, scaled);
            }
            
            //List<PDS> pdss = new List<PDS>(PowerSupplies.ItemsSource.GetEnumerator());
            PDS60ca[] pdss = new PDS60ca[PowerSupplies.Items.Count];
            PowerSupplies.Items.CopyTo(pdss, 0);
            XElement xmlDoc = new XElement("Configuration");
            xmlDoc.Add(XmlEncoder.encodeXml(serToTransform));
            xmlDoc.Add(XmlEncoder.encodeXml(pdss));
            xmlDoc.Save("LedAlignment.xml");
        }

    
    }
}
