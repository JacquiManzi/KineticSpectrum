using System;
using System.Windows.Media;

namespace KineticControl
{
    public class Led
    {
        public LedPosition LedPosition { get; set; }
        private readonly byte[] _byteArray;
        private readonly int _initialLength;
        private readonly int _pos;
        private double _brightness = 1;

        internal Led(byte[] bytes, int initialLength, int position)
        {
            _byteArray = bytes;
            _pos = position;
            _initialLength = initialLength;
            Color = Colors.Black;
        }

        public double Brightness
        {
            get { return _brightness; }
            set { _brightness = Math.Max(0, Math.Min(1, value)); }
        }

        public Color Color
        {
            get
            {
                if (_initialLength + _pos * 3 >= _byteArray.Length) throw new IndexOutOfRangeException();
                Color color = new Color
                {
                    R = ReverseBrightness(_byteArray[_initialLength + _pos * 3]),
                    G = ReverseBrightness(_byteArray[_initialLength + _pos * 3 + 1]),
                    B = ReverseBrightness(_byteArray[_initialLength + _pos * 3 + 2]),
                };
                return color;
            }
            set
            {
                if (_initialLength + _pos * 3 > _byteArray.Length) throw new IndexOutOfRangeException();
                _byteArray[_initialLength + _pos * 3] = (AdjustBrightness(value.R));
                _byteArray[_initialLength + _pos * 3 + 1] = (AdjustBrightness(value.G));
                _byteArray[_initialLength + _pos * 3 + 2] = (AdjustBrightness(value.B));
            }
        }

        private byte ReverseBrightness(byte byteValue)
        {
            if (_brightness == 0)
                return 0;
            byte newValue = (byte) (byteValue/_brightness);
            return Math.Max((byte) 0, Math.Min(newValue, (byte) 255));
        }
        private byte AdjustBrightness(byte byteValue)
        {
            byte newValue = (byte) (byteValue*_brightness);
            return Math.Max((byte) 0, Math.Min(newValue, (byte) 255));
        }
    }
}
