﻿using System;
using KineticControl;

namespace RevKitt.ks.KinectCV
{
    public class Light
    {
        private Blob _lightBlob;
        private int _maxBrightness;

        public Light(Light light) : this(light.LightBlob, light.Address) {}

        public Light(Blob blob) : this(blob, LightAddress.Unknown) {}

        public Light(Blob blob, LightAddress address)
        {
            Address = address;
            LightBlob = blob;
        }

        public bool Contains(Blob b)
        {
            return LightBlob.OverlapPercent(b) > 0;
        }

        public bool IsUnknown { get { return Address.IsUnknown; } }

        public Blob LightBlob {
            get { return _lightBlob; }
            set
            {
                if (value.ZCenter == 0 && _lightBlob != null)
                    value.ZCenter = _lightBlob.ZCenter;
                _lightBlob = value;
                _lightBlob.Name = Address.ToString();
                Brightness = _lightBlob.Brightness;
                _maxBrightness = Math.Max(Brightness, _maxBrightness);
            }
        }

        public LightAddress Address { get; set; }

        public int Brightness { get; internal set; }

        public bool SensedOn { get { return Brightness > _maxBrightness/2; } }
    }
}