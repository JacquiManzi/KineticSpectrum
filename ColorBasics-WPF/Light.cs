using KinectDisplay;
using KineticControl;

namespace RevKitt.LightBuilder
{
    public class Light
    {
        private Blob _lightBlob;

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
            set { 
                _lightBlob = value;
                _lightBlob.Name = Address.ToString();
             }
        }

        public LightAddress Address { get; set; }
    }
}
