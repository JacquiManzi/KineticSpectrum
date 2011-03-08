using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KinectAndGLTest.Kinect
{
    class Device
    {
        private readonly int _id;
        private readonly string _serial;
        private readonly Timer _timer;
        private LedMode _ledMode;

        private readonly IList<CameraRef> _cameraRefs;

        public Motor Motor { get; private set; }

        internal IntPtr CameraPointer { get; private set; }

        public Accelerometer Accelerometer { get; private set; }
        
        internal Device(int id, string serial)
        {
            if(_serial == null) throw new ArgumentNullException("serial");
            _id = id;
            _serial = serial;
            _cameraRefs = new List<CameraRef>();
            _timer = new Timer(Update,null, Timeout.Infinite, Timeout.Infinite );
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
                    _timer.Change(0, 1000/30);
                }
                cRef = new CameraRef( type, this, dispatcher, timeoutMs );
                _cameraRefs.Add(cRef);
            }
            return cRef.BitmapSource;
        }

        private void Update(object ignored)
        {
            foreach (var cRef in _cameraRefs.Where(cRef => !cRef.Update()))
            {
                RemoveRef(cRef);
            }
        }

        private void RemoveRef(CameraRef cRef)
        {
            lock (_cameraRefs)
            {
                _cameraRefs.Remove(cRef);
                if (_cameraRefs.Count == 0)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    KinectDevice.StopCamera(CameraPointer);
                    KinectDevice.DestroyCamera(CameraPointer);
                    CameraPointer = IntPtr.Zero;
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
