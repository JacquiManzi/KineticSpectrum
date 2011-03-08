using System.Collections.Generic;

namespace KinectAndGLTest.Kinect
{
    class DeviceLoader
    {
        public static readonly DeviceLoader Instance = new DeviceLoader();
        private static IList<Device> _devices;

        private DeviceLoader() {}

        public IList<Device> Devices
        {
            get
            {
                int deviceCount = KinectDevice.GetDeviceCount();
                if(_devices == null)
                {
                  _devices = new List<Device>(deviceCount);  
                }

                //save the existing set of devices
                ISet<Device> existing = new HashSet<Device>(_devices);

                for (int i = 0; i < deviceCount; i++ )
                {
                    var device = new Device(i, KinectDevice.GetDeviceSerial(i));
                    existing.Remove(device);
                    if(!_devices.Contains(device))
                    {
                        _devices.Add(device);
                    }
                }

                foreach (var d in existing)
                {
                    _devices.Remove(d);
                }
                return _devices;
            }
        }


    }
}
