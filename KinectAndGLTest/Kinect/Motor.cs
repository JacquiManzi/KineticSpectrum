using System;
using System.Runtime.InteropServices;

namespace KinectAndGLTest.Kinect
{
    class Motor : SafeHandle
    {
        private short _position;
        internal IntPtr MotorPtr { get; private set; }

        internal Motor(String serial) : base(IntPtr.Zero, true)
        {
            MotorPtr = KinectDevice.CreateMotor(serial);
            Position = 0;
        }

        public short Position
        {
            get { return _position; }
            set 
            { 
                KinectDevice.SetMotorPosition(MotorPtr, value);
                _position = value;
            }
        }

        protected override bool ReleaseHandle()
        {
            KinectDevice.DestroyMotor(MotorPtr);
            MotorPtr = IntPtr.Zero;
            return true;
        }

        public override bool IsInvalid
        {
            get { return MotorPtr == IntPtr.Zero; }

        }
    }
}
