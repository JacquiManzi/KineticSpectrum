
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
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        static MainWindow()
        {
            string pathEnv = Environment.GetEnvironmentVariable("Path");
            Environment.SetEnvironmentVariable("Path", "lib\\OpenCV;" + pathEnv);
        }

        private static Network _network;
        private static BucketUpdater _buckets;
        private static IList<PDS> _PDSs;
        private static MCvFont _font = new MCvFont(FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);

        private static IList<KinectPanel> _panels;


        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("Setting up Kinect...");
            var devices = DeviceLoader.Instance.Devices;
            if(devices.Count == 0)
            {
                Console.WriteLine("No Kinect available");
                Environment.Exit(0);
            }

            StackPanel grid = (StackPanel) Content;
            _panels = new List<KinectPanel>(devices.Count);

            foreach(Device device in devices)
            {
                Console.WriteLine("Setting up Device: " + device.Serial);
                KinectPanel kp = new KinectPanel {Device = device, Font = _font, Position = 0};
                kp.CalibrateAndDispatch();
                _panels.Add(kp);
                Height += 350;
                grid.Children.Add(kp);
            }
           
            SetupNetwork();
        }

        private void SetupNetwork()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Setting Up Network...");
            _network = new Network();
            Console.WriteLine("Setting Network Inerface...");
            _network.SetInterface("Local Area Connection");
            Console.WriteLine("Searching for Power Supplies...");
 //           _network.FindPowerSupplies();
            //_network.BroadCast();
            _PDSs = _network.PDSs;
            Console.WriteLine("Found {0} PDSs.", _PDSs.Count);

//            Task frameProcessor = new Task(()=>{
//                                                   Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
//                                                   while(true){ ProcessFrame(); }
//                                               });
//            frameProcessor.Start();
        }
    }
}
