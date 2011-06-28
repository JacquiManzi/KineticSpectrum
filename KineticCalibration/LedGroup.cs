using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KineticControl;

namespace KineticCalibration
{
    internal class LedGroup
    {
        internal static readonly BitmapImage BlackSqare =
            new BitmapImage(  new Uri(Environment.CurrentDirectory+@"\blackSquare.png"));
        internal static readonly BitmapImage RedSquare = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\redSquare.png"));
        //private readonly List<Led> _leds;
        private readonly IList<LedMarker> _markers;
        private IEnumerable<Image> _images;


        public LedGroup(IList<Led> leds, Grid grid, Border border, int no, IEnumerable<Image> images)
        {
            _markers = new List<LedMarker>(leds.Count);
            double starty = 8*25;
            double startx = 380 + no;
            _images = images;
            foreach (Led led in leds)
            {
                LedMarker marker = new LedMarker(led, border);
                
                Image img = marker.Img;
                grid.Children.Add(img);
                Grid.SetRow( img, 1);
                Grid.SetColumn(img, 3);
                _markers.Add(marker);
                if(led.LedPosition == null)
                {
                    marker.Transform(startx, starty);
                    starty -= 8;
                }
                else
                {
                    Image fst = _images.GetEnumerator().Current;
                    double x = led.LedPosition.X*fst.ActualWidth/640.0;
                    double y = led.LedPosition.Y*fst.ActualHeight/480.0;
                    marker.Transform(x,y);
                }

            }

            foreach (LedMarker marker in _markers)
            {
                marker.Selected += Selected;
                marker.Moved += Moved;
            }
        }
        
        private void Moved(LedMarker sender)
        {
            Led led = sender.Led;
            if(led.LedPosition == null)
            {
                led.LedPosition = new LedPosition();
            }
            Image fst = _images.GetEnumerator().Current;
            led.LedPosition.X = sender.X*640.0/fst.ActualWidth;
            led.LedPosition.Y = sender.Y*480.0/fst.ActualHeight;
            led.LedPosition.External = true;
            foreach (Image image in _images)
            {
                if (image.IsMouseOver)
                {
                    led.LedPosition.External = false;
                    break;
                }
            }
        }

        private void Selected(LedMarker marker)
        {
            foreach (LedMarker ledMarker in _markers)
            {
                if (marker != ledMarker)
                    ledMarker.IsSelected = false;
            } 
        }
    }


    internal delegate void SelectedEventHandler(LedMarker sender);

    internal delegate void MovedEventHandler(LedMarker sender);

    internal class LedMarker
    {
        private static Point _origin;
        private static Point _start;

        internal readonly Led Led;
        private readonly Border _border;
        internal readonly Image Img = new Image {Source = LedGroup.RedSquare};
        private bool _isSelected;
        //private readonly Image _red = new Image {Source = LedGroup.RedSquare, Opacity = 0};
       


        internal LedMarker(Led led, Border borderBase)
        {
            _isSelected = true;
            _border = borderBase;
            Led = led;
            Img.MaxHeight = 4;
            Img.MaxWidth = 4;
            Img.MouseLeftButtonDown += ImageMouseLeftButtonDown;
            Img.MouseLeftButtonUp += ImageMouseLeftButtonUp;
            Img.MouseMove += ImageMouseMove;
            Img.RenderTransform = new TranslateTransform();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (value)
                {
                    Img.Source = LedGroup.RedSquare;
                    Led.Color = Colors.Red;
                    if(Selected != null)
                        Selected(this);
                }
                else
                {
                    Img.Source = LedGroup.BlackSqare;
                    Led.Color = Colors.Black;
                }
            }
        }

        internal SelectedEventHandler Selected;
        internal MovedEventHandler Moved;

        internal void Transform(double x, double y)
        {
            TranslateTransform TT = (TranslateTransform) Img.RenderTransform;
            TT.X += x;
            TT.Y += y;
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image) sender;
            IsSelected = true;
            image.CaptureMouse();
            var TT = (TranslateTransform) image.RenderTransform;
            _start = e.GetPosition(_border);
            _origin = new Point(TT.X, TT.Y);
        }

        private void ImageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            image.ReleaseMouseCapture();
            if (Moved != null)
                Moved(this);
            //Console.WriteLine(image.RenderTransform.Value);
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            if (!image.IsMouseCaptured) return;

            var TT = (TranslateTransform) image.RenderTransform;
            Vector vMM = _start - e.GetPosition(_border);
            TT.X = _origin.X - vMM.X;
            TT.Y = _origin.Y - vMM.Y;
        }

        internal double X { get { return Img.RenderTransform.Value.OffsetX; } }
        internal double Y { get { return Img.RenderTransform.Value.OffsetY; } }
    }
}
