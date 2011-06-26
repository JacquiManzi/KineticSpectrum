using System.Collections.Generic;
using System.Timers;

namespace KinectAPI
{
    public class DeviceLoader
    {
        public static readonly DeviceLoader Instance = new DeviceLoader();
        private static IList<Device> _devices;


        private readonly Timer _timer;
        private IList<Device> _cachedDevices = new List<Device>();

        private DeviceLoader()
        {
            _timer = new Timer { Interval = 1000 / 30 };
            _timer.Elapsed += (s, e) => Update();
            _timer.Start();
        }

        private volatile bool updating = false;
        private void Update()
        {
            if (updating) return;
            updating = true;
            foreach (Device cachedDevice in _cachedDevices)
            {
                cachedDevice.Update();
            }
            updating = false;
        }

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
                _cachedDevices = _devices;
                return _devices;
            }
        }
    }
}
