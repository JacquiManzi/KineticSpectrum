using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KinectAPI
{
    public class Device
    {
        private readonly int _id;
        private readonly string _serial;
        private readonly Timer _timer;
        private LedMode _ledMode;
        
        private readonly Dictionary<CameraType, CameraRef> _cameraRefs;

        public Motor Motor { get; private set; }

        internal IntPtr CameraPointer { get; private set; }

        public Accelerometer Accelerometer { get; private set; }
        
        internal Device(int id, string serial)
        {
            if(serial == null) throw new ArgumentNullException("serial");
            _id = id;
            _serial = serial;
            _cameraRefs = new Dictionary<CameraType, CameraRef>();
            _timer = new Timer {Interval = 1000/30};
            _timer.Elapsed += (s,e) => Update();

            _ledMode = LedMode.Off;
            Motor = new Motor(serial);
            Accelerometer = new Accelerometer(Motor);
        }

        public LedMode LedMode
        {
            get { return _ledMode; }
            set
            {
                _ledMode = value;
                KinectDevice.SetMotorLED(Motor.MotorPtr, (byte)value);
            }
        }

        #region Camera Methods
        public BitmapSource GetCamera(CameraType type, Dispatcher dispatcher, int timeoutMs = CameraRef.DEFAULT_TIMEOUT)
        {
            return AllocateCamera(type, dispatcher, timeoutMs).BitmapSource;
        }

        public DepthMatrix GetDepthMatrix()
        {
            
            CameraRef cRef = AllocateCamera(CameraType.DepthCorrected8, null, CameraRef.DEFAULT_TIMEOUT);
            BitmapSource src = cRef.BitmapSource;
            return cRef.DepthMatrix;
        }

        private CameraRef AllocateCamera(CameraType type, Dispatcher dispatcher, int timeoutMs)
        {
            CameraRef cRef;
            lock (_cameraRefs)
            {
                if (_cameraRefs.Count == 0)
                {
                    CameraPointer = KinectDevice.CreateCamera( _serial );
                    if(!KinectDevice.StartCamera(CameraPointer))
                    {
                        throw new ApplicationException("Unexpectedly unable to start the Kinect Camera for device: " + this);
                    }
                }

                if (!_cameraRefs.TryGetValue(type, out cRef))
                {
                    cRef = new CameraRef(type, this, dispatcher, timeoutMs);
                    _cameraRefs[type] = cRef;
                }
                _timer.Enabled = true;
                if (!cRef.HasDispatcher) cRef.Dispatcher = dispatcher;
            }

            return cRef;
        }

        private void Update()
        {
            Dictionary<CameraType, CameraRef> refs; 
            lock(_cameraRefs)
            {
              refs = new Dictionary<CameraType,CameraRef>(_cameraRefs);
            }

            foreach (var entry in refs.Where(entry => !entry.Value.Update()))
            {
                RemoveRef(entry.Key);
            }
        }

        private void RemoveRef(CameraType cType)
        {
            CameraRef cRef = _cameraRefs[cType];
            lock (_cameraRefs)
            {
                if (!cRef.Update())
                {
                    _cameraRefs.Remove(cType);
                    if (_cameraRefs.Count == 0)
                    {
                        _timer.Enabled = false;
                        KinectDevice.StopCamera(CameraPointer);
                        KinectDevice.DestroyCamera(CameraPointer);
                        CameraPointer = IntPtr.Zero;
                    }
                }
            }
            cRef.Dispose();
        }
        #endregion

        public int Id
        {
            get { return _id; }
        }

        public string Serial
        {
            [DebuggerStepThrough]
            get { return _serial; }
        }

        #region Object Overrides: GetHashCode, Equals, ==, !=, ToString

        public override int GetHashCode()
        {
            return _serial.GetHashCode();
        } 

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return _serial == ((Device) obj)._serial;
        }

        public static bool operator ==(Device lhs, Device rhs)
        {
            if(ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Device lhs, Device rhs)
        {
            return !(lhs == rhs);
        }

        public override String ToString()
        {
            return "Device #" + _id + " with serial number '" + _serial + "'";
        }

        #endregion
    }
}
