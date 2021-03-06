﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace RevKitt.KS.KineticEnvironment.Interact
{
    public enum KinectMode
    {
        SHADOW, GESTURE
    }

    public class GesturePosition
    {
        public bool IsTracked;
        public ulong SkeletonId;
        public Vector3D RightHand;
        public Vector3D LeftHand;
        public Vector3D RightShoulder;
        public Vector3D LeftShoulder;
        public Vector3D RightWrist;
        public Vector3D LeftWrist;
        public Vector3D Head;

        public void PrintGesture()
        {
            System.Diagnostics.Debug.WriteLine("RH: {0} {1} {2}", RightHand.X, RightHand.Y, RightHand.Z );
            System.Diagnostics.Debug.WriteLine("RW: {0} {1} {2}", RightWrist.X, RightWrist.Y, RightWrist.Z);
        }
    }

    class GestureState
    {
        private volatile GesturePosition _initialGesture;
        private Int64 _lastUpdate = DateTime.Now.Ticks;
        private Timer _timer;

        private long _currentRatio = 0;

        public double CurrentPortion
        {
            get
            {
                if (_initialGesture == null)
                    return double.NaN;
                return BitConverter.Int64BitsToDouble(Interlocked.Read(ref _currentRatio));
            }
            set { Interlocked.Exchange(ref _currentRatio, BitConverter.DoubleToInt64Bits(value)); }
        }

        public GestureState()
        {
            _timer = new Timer(ValidateTime,null, 1000, 1000);
        }

        private void ValidateTime(object value)
        {
            if ((DateTime.Now - LastUpdate()).TotalSeconds > 1)
            {
                _initialGesture = null;
            }
        }

        private void UpdateTime()
        {
            Interlocked.Exchange(ref _lastUpdate, DateTime.Now.Ticks);
        }
        private DateTime LastUpdate()
        {
            return new DateTime(Interlocked.Read(ref _lastUpdate));
        }

        private bool IsValid(GesturePosition gesturePosition)
        {
            bool isValid = gesturePosition.RightHand.Y > gesturePosition.RightShoulder.Y &&
                   gesturePosition.LeftHand.Y > gesturePosition.LeftShoulder.Y;
            return isValid;
        }

        private bool IsStart(GesturePosition gesturePosition)
        {
            return .06 > Math.Abs((gesturePosition.RightHand - gesturePosition.LeftHand).Length);
        }

        private double GetDistance(GesturePosition gesturePosition)
        {
            return Math.Abs((gesturePosition.RightHand - gesturePosition.LeftHand).Length);
        }

        private double GetRatio(GesturePosition gesturePosition)
        {
            double handDist = GetDistance(gesturePosition) - GetDistance(_initialGesture);
            double shoulderDist = Math.Abs((gesturePosition.RightShoulder - gesturePosition.LeftShoulder).Length);
            double ratio = (handDist/3)/shoulderDist;
            return Math.Max(0, Math.Min(1, ratio));
        }

        private void Initialize(GesturePosition gesturePosition)
        {
            if (IsStart(gesturePosition))
            {
                System.Diagnostics.Debug.WriteLine("Got initial gesture!");
                if(_initialGesture == null)
                    _initialGesture = gesturePosition;
                else if (GetDistance(gesturePosition) < GetDistance(_initialGesture))
                    _initialGesture = gesturePosition;
            }
        }

        private DateTime _lastPrint;
        public void UpdatePosition(GesturePosition gesturePostition)
        {
            if((DateTime.Now - LastUpdate()).TotalSeconds > 1)
                UpdateTime();
                gesturePostition.PrintGesture();
//            if (!IsValid(gesturePostition))
//            {
//                _initialGesture = null;
//                return;
//            }
//            Initialize(gesturePostition);
//            if(_initialGesture != null)
//                CurrentPortion = GetRatio(gesturePostition);
//            if (DateTime.Now - new TimeSpan(0, 0, 1) > _lastPrint)
//            {
//                gesturePostition.PrintGesture();
//                double handDistance = Math.Abs((gesturePostition.RightHand - gesturePostition.LeftHand).Length);
//                double shoulderDistance =
//                    Math.Abs((gesturePostition.RightShoulder - gesturePostition.LeftShoulder).Length);
//                System.Diagnostics.Debug.WriteLine("Hand Distance: {0}", handDistance);
//                System.Diagnostics.Debug.WriteLine("Shoulder Distance: {0}", shoulderDistance);

//                _lastPrint = DateTime.Now;
//            }
        }
    }

    public delegate void GesturesUpdates(GesturePosition[] gesturePositions, int lastBodyId);

    public class KinectPlugin
    {
        public static KinectPlugin Instance = new KinectPlugin();
        private KinectSensor _sensor;
        private BodyFrameReader _frameReader;
//        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        private bool _enabled;
//        private DepthImagePixel[] _depthPixels;
        private byte[] _imagePixels;
        private readonly object _lock = new object();
        private Body[] _bodies = null;
        private Body _currentBody = null;
        public Vector3D CameraPosition = new Vector3D(0,0,0);
        public GesturesUpdates GestureUpdateHandler;

//        private readonly GestureState _gestureState = new GestureState();
        private GesturePosition[] _gesturePositions;

        private int _width, _height;

        public KinectMode Mode = KinectMode.GESTURE;

        public KinectPlugin()
        {
            FallOff = .95;
        }

        private void Setup()
        {
            lock (_lock)
            {
                if(_sensor != null)
                    return;

                _sensor = KinectSensor.GetDefault();
                if (_sensor == null)
                {
                    System.Diagnostics.Debug.WriteLine("No Kinect Found");
                    return;
                }
                if (Mode == KinectMode.SHADOW)
                    SetupShadow(_sensor);
                else if (Mode == KinectMode.GESTURE)
                    SetupGesture(_sensor);
                try
                {
                    _sensor.Open();
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error Starting Kinect");
                    return;
                }
                System.Diagnostics.Debug.WriteLine("Sensor successfully started");
            }
        }

        private void SetupGesture(KinectSensor sensor)
        {
            _frameReader = sensor.BodyFrameSource.OpenReader();
            _frameReader.FrameArrived += SkeletonFrameReady;
        }

//        public double GesturePortion { get { return _gestureState.CurrentPortion; } }

        private void SkeletonFrameReady(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame()) // Open the Skeleton frame
            {
                if (bodyFrame != null)
                {
                    if (_bodies == null)
                    {
                        _bodies = new Body[bodyFrame.BodyCount];
                        _gesturePositions = new GesturePosition[bodyFrame.BodyCount];
                        for (int i = 0; i < _gesturePositions.Length; i++)
                        {
                            _gesturePositions[i] = new GesturePosition() {IsTracked = false};
                        }
                    }
                    bodyFrame.GetAndRefreshBodyData(this._bodies);
                    HandleSkeletonUpdate(_bodies);
                }
            }
        }

        private void HandleSkeletonUpdate(Body[] bodies)
        {
            int lastTracked = -1;
            for(int i=0; i<bodies.Length; i++)
            {
                Body body = bodies[i];
                GesturePosition gesturePosition = _gesturePositions[i];
                bool isTracked = body.IsTracked && body.Joints[JointType.HandLeft].Position.Z != 0.0;
                gesturePosition.IsTracked = isTracked;
                gesturePosition.SkeletonId = body.TrackingId;
                if (isTracked)
                { 
                    lastTracked = i;
                    var jc = body.Joints;
                    gesturePosition.RightHand = OrientJoint(jc[JointType.HandRight]);
                    gesturePosition.LeftHand = OrientJoint(jc[JointType.HandLeft]);
                    gesturePosition.RightShoulder = OrientJoint(jc[JointType.ShoulderRight]);
                    gesturePosition.LeftShoulder = OrientJoint(jc[JointType.ShoulderLeft]);
                    gesturePosition.RightWrist = OrientJoint(jc[JointType.WristRight]);
                    gesturePosition.LeftWrist = OrientJoint(jc[JointType.WristLeft]);
                    gesturePosition.Head = OrientJoint(jc[JointType.Head]);
                }
            }
            if(GestureUpdateHandler != null)
                GestureUpdateHandler(_gesturePositions, lastTracked);
        }

        private static readonly double METERS_TO_INCHES = 3.28*12;
        private Vector3D OrientJoint(Joint joint)
        {
            return new Vector3D
                       {
                           X = joint.Position.Z * METERS_TO_INCHES + CameraPosition.X,
                           Y = joint.Position.X * METERS_TO_INCHES + CameraPosition.Y,
                           Z = joint.Position.Y * METERS_TO_INCHES + CameraPosition.Z
                       };
        }

        private void SetupShadow(KinectSensor sensor)
        {
//            sensor.DepthStream.Enable(DepthFormat);
//            _depthPixels = new DepthImagePixel[sensor.DepthStream.FramePixelDataLength];
//            _imagePixels = new byte[sensor.DepthStream.FramePixelDataLength];
//            _width = _sensor.DepthStream.FrameWidth;
//            _height = _sensor.DepthStream.FrameHeight;
//            sensor.DepthFrameReady += SensorOnDepthFrameReady;
        }
//
//        private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
//        {
//            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
//            {
//                if (depthFrame != null)
//                {
//                    depthFrame.CopyDepthImagePixelDataTo(_depthPixels);
// 
//                    int minDepth = depthFrame.MinDepth;
//                    int maxDepth = depthFrame.MaxDepth;
//
//                    for (int i = 0; i < _depthPixels.Count(); i++)
//                    {
//                        _imagePixels[i] = (byte)(FallOff*_imagePixels[i]);
//                        if (_imagePixels[i] < 10)
//                            _imagePixels[i] = 0;
//                        short depth = _depthPixels[i].Depth;
//                        if (depth > minDepth && depth < maxDepth)
//                        {
//                            _imagePixels[i] = 255;
////                            _colorPixels[colorIndex++] = _imagePixels[i];
////                            _colorPixels[colorIndex++] = _imagePixels[i];
////                            _colorPixels[colorIndex++] = _imagePixels[i];
////                            colorIndex++;
//                        }
//                    }
//                    // Write the pixel data into our bitmap
////                    _colorBitmap.WritePixels(
////                        new Int32Rect(0, 0, _colorBitmap.PixelWidth, _colorBitmap.PixelHeight),
////                        _colorPixels,
////                        _colorBitmap.PixelWidth * sizeof(int),
////                        0);
//                }
//            }
//        }
//
        public bool HasKinect
        {
            get
            {
                if (_sensor != null)
                    return true;
                Setup();
                return _sensor != null;
            }
        }


        public bool Enabled
        {
            get { return _enabled && _sensor != null; }
            set { 
                _enabled = value && HasKinect; 
            }
        }

        public double XMax { get; set; }
        public double XMin { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }

        public double FallOff { get; set; }

        public byte Applies(double x, double y)
        {
            x = (x - XMin)/(XMax - XMin);
            y = (y - YMin)/(YMax - YMin);
            if (!Enabled)
                return 0;

            int xpos = (int)(x*(_width - 1));
            int ypos = (int) ((1 - y)*(_height - 1));

            return _imagePixels[ypos*_width + xpos];
        }
    }
}
