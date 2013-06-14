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
using System.Windows.Media.Imaging;
using KineticControl;
using Microsoft.Kinect;

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


        private ImageProcessor _processor;


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
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

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
                this.ColorImage.MouseLeftButtonUp += ColorImageOnMouseLeftButtonUp;
                

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                LightSystem lightSystem = InitCK();
                _processor = new ImageProcessor(lightSystem, this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight);
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

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

        private LightSystem InitCK()
        {
            LightingManager lightingManager = new LightingManager();
            if (lightingManager.NeedsPrompt)
                throw new Exception("BOOOO! Network adapter not set properly. Options Include: \n" + String.Join("\n", lightingManager.LightSystem.NetworkInterfaces));
            Console.WriteLine("Querying for Lighting....");
            lightingManager.LightSystem.RefreshLightList().Wait();
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
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            SearchButton.IsEnabled = !_processor.IsSearching;
            if (!_processingFrame)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        // Copy the pixel data from the image to a temporary array
                        colorFrame.CopyPixelDataTo(this.colorPixels);
                        Bitmap processed = _processor.ProcessImage(this.colorPixels);
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