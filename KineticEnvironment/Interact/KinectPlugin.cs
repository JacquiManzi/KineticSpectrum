using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Interact
{
    public class KinectPlugin
    {
        private KinectSensor _sensor;
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        private bool _enabled;

        public KinectPlugin()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    _sensor = potentialSensor;
                    break;
                }
            } 
            _sensor.DepthStream.Enable(DepthFormat);
        }

        public bool HasKinect { get { return _sensor != null; } }

        public bool Enabled { get { return _enabled && HasKinect; } set { _enabled = value; } }

        public void Apply(IEnumerable<LEDNode> ledNodes)
        {
        }
    }
}
