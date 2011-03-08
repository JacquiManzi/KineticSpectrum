namespace KinectAndGLTest.Kinect
{
    class Accelerometer
    {
        private readonly Motor _motor;

        internal Accelerometer(Motor motor)
        {
            _motor = motor;
        }

        public short XValue { 
            get
            {
                short x=0, y=0, z=0;
                KinectDevice.GetMotorAccelerometer(_motor.MotorPtr, ref x, ref y, ref z);
                return x;
            } 
        }

        public short YValue
        {
            get
            {
                short x = 0, y = 0, z = 0;
                KinectDevice.GetMotorAccelerometer(_motor.MotorPtr, ref x, ref y, ref z);
                return y;
            }
        }

        public short ZValue { 
            get
            {
                short x = 0, y = 0, z = 0;
                KinectDevice.GetMotorAccelerometer(_motor.MotorPtr, ref x, ref y, ref z);
                return z;
            }
        }
    }
}
