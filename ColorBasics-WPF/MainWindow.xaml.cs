//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Drawing;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KineticControl;
using Microsoft.Kinect;
using RevKitt.ks.KinectCV;
using Point = System.Windows.Point;

namespace RevKitt.LightBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        private DepthImagePixel[] depthPixels;
        private DepthImagePoint[] depthPoints;


        private ImageProcessor _processor;


        private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution640x480Fps30;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            string pathEnv = Environment.GetEnvironmentVariable("Path");
            Environment.SetEnvironmentVariable("Path", "lib\\OpenCV;" + pathEnv);
            InitializeComponent();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorFormat);
                this.sensor.DepthStream.Enable(DepthFormat);

                var cameraSettings = this.sensor.ColorStream.CameraSettings;

                cameraSettings.AutoExposure = false;
                cameraSettings.AutoWhiteBalance = false;
                cameraSettings.FrameInterval = cameraSettings.MinExposureTime;
                cameraSettings.BacklightCompensationMode = BacklightCompensationMode.LowlightsPriority;

                this.ExposureSlider.Minimum = cameraSettings.MinExposureTime;
                this.ExposureSlider.Maximum = cameraSettings.MaxExposureTime;
                this.ExposureSlider.Value =  cameraSettings.FrameInterval;

                this.WBSlider.Minimum = cameraSettings.MinWhiteBalance;
                this.WBSlider.Maximum = cameraSettings.MaxWhiteBalance;
                this.WBSlider.Value = cameraSettings.WhiteBalance;

                this.ExposureSlider.ValueChanged += ExposureAdjusted;
                this.WBSlider.ValueChanged += WBAdjusted;
                this.SearchButton.Click += (o, args) => _processor.BeginSearch();
                this.SaveButton.Click += (o, args) => _processor.ExportObj(BuildFileName(".obj"));
                this.ViewButton.Click += (o, args) => _processor.SaveView();
                this.ResetButton.Click += (o, args) => _processor.Reset();
                this.ColorImage.MouseLeftButtonUp += ColorImageOnMouseLeftButtonUp;
                

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.depthPoints = new DepthImagePoint[this.sensor.DepthStream.FramePixelDataLength];

                LightSystem lightSystem = InitCK();
                _processor = new ImageProcessor(lightSystem, this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight);
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.AllFramesReady += this.AllFramesReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                    //this.sensor.ElevationAngle = this.sensor.MinElevationAngle;
                    this.StatusBar.Content = "Started Kinect with ID: " + this.sensor.UniqueKinectId;
                }
                catch (IOException)
                {
                    this.StatusBar.Content = "Failed to start Kinect: " + this.sensor.UniqueKinectId;
                    this.sensor = null;
                }
            }
            else
            {
                this.StatusBar.Content = Properties.Resources.NoKinectReady;
            }
        }

        private void UpdateDistance(object sender, MouseEventArgs e)
        {
                Point matPos = e.GetPosition(this.MaskImage);

                matPos.X *= 640.0 / this.MaskImage.ActualWidth;
                matPos.Y *= 480.0 / this.MaskImage.ActualHeight;
                float value = this.depthPoints[(int)matPos.Y*640+(int)matPos.X].Depth;
                if (Content.ToString() != value.ToString())
                {
                    this.StatusBar.Content = value;
                }
            
        }

       

        
        private string BuildFileName(string endsWith)
        {
            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            return Path.Combine(myPhotos, "KineticObj-" + time + endsWith);
        }
        

        private LightSystem InitCK()
        {
            LightingManager lightingManager = new LightingManager(Properties.Resources.HostInterface);
            if (lightingManager.NeedsPrompt)
                throw new Exception("BOOOO! Network adapter not set properly. Options Include: \n" + String.Join("\n", lightingManager.LightSystem.NetworkInterfaces));
            Console.WriteLine("Querying for Lighting....");
            LightSystem system = lightingManager.LightSystem;
            system.AutoUpdate = true;
            system.RefreshLightList().Wait();
            system.SetAllToColor(Colors.White);
            return lightingManager.LightSystem;
        }

        private void ColorImageOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var point = mouseButtonEventArgs.GetPosition(this.ColorImage);
            _processor.Click((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void ExposureAdjusted(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            this.sensor.ColorStream.CameraSettings.FrameInterval = Math.Round(args.NewValue);
        }

        private void WBAdjusted(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            this.sensor.ColorStream.CameraSettings.WhiteBalance = (int) Math.Round(args.NewValue);
        }


        private volatile bool _processingFrame = false;

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SearchButton.IsEnabled = !_processor.IsSearching;
            ResetButton.IsEnabled = _processor.IsSearching || _processor.DoneSearching;
            ViewButton.IsEnabled = _processor.DoneSearching;
            SaveButton.IsEnabled = _processor.CanExport;

            if (!_processingFrame)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (colorFrame != null && depthFrame != null)
                    {
                        // Copy the pixel data from the image to a temporary array
                        colorFrame.CopyPixelDataTo(this.colorPixels);
                        depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                        this.sensor.CoordinateMapper.MapColorFrameToDepthFrame(
                            ColorFormat, DepthFormat,
                            this.depthPixels, depthPoints);

                        Bitmap processed = _processor.ProcessImage(this.colorPixels, depthPoints);
                        UpdateColorImage(processed);
                        UpdateMaskImage(_processor.Mask.Bitmap);
                    }
                }
                _processingFrame = false;
            }
            else
            {
                Console.WriteLine("Not Processed");
            }
        }

        static MemPointer GetBitmap(Bitmap bitmap)
        {
            MemPointer memP = new MemPointer();
            memP.Ptr = bitmap.GetHbitmap();
            memP.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                memP.Ptr,
                IntPtr.Zero,
                new Int32Rect(0,0, bitmap.Width, bitmap.Height), 
                BitmapSizeOptions.FromEmptyOptions());
            return memP;
        }

        private MemPointer _oldMaskPtr;
        private readonly object maskImgLock = new object();

        private void UpdateMaskImage(Bitmap image)
        {
            lock (maskImgLock)
            {
                MemPointer memP = GetBitmap(image);
                this.MaskImage.Source = memP.Source;
                if (_oldMaskPtr != null)
                {
                    DeleteObject(_oldMaskPtr.Ptr);
                }
                _oldMaskPtr = memP;
            }
            
        }


        private MemPointer _oldImgPtr;
        private readonly object colorImgLock = new object();

       
        private void UpdateColorImage(Bitmap image)
        {
            lock (colorImgLock)
            {
                MemPointer memP = GetBitmap(image);
                this.ColorImage.Source = memP.Source;
                if (_oldImgPtr != null)
                {
                    DeleteObject(_oldImgPtr.Ptr);
                }
                _oldImgPtr = memP;
            }
        }

        internal class MemPointer
        {
            internal BitmapSource Source;
            internal IntPtr Ptr;
        }


 

        private void SaveImage(BitmapSource bitmap, String saveLocation)
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            

            // write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(saveLocation, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                this.StatusBar.Content = string.Format("{0} {1}", Properties.Resources.ScreenshotWriteSuccess,
                                                       saveLocation);
            }
            catch (IOException)
            {
                this.StatusBar.Content = string.Format("{0} {1}", Properties.Resources.ScreenshotWriteFailed, saveLocation);
            }
            
        }
        /// <summary>
        /// Handles the user clicking on the screenshot button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ButtonScreenshotClick(object sender, RoutedEventArgs e)
        {
            if (null == this.sensor)
            {
//                this.statusBarText.Text = Properties.Resources.ConnectDeviceFirst;
                return;
            }

            string time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            string colorPath = Path.Combine(myPhotos, "KinectSnapshot-" + time + "-color.png");
            SaveImage(this._oldImgPtr.Source, colorPath);
            string maskPath = Path.Combine(myPhotos, "KinectSnapshot-" + time + "-mask.png");
            SaveImage(this._oldMaskPtr.Source, maskPath);
        }
    }
}