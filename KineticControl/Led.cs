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

        internal Led(byte[] bytes, int initialLength, int position)
        {
            _byteArray = bytes;
            _pos = position;
            _initialLength = initialLength;
            Color = Colors.Black;
        }

        public Color Color
        {
            get
            {
                if (_initialLength + _pos * 3 >= _byteArray.Length) throw new IndexOutOfRangeException();
                Color color = new Color
                {
                    R = _byteArray[_initialLength + _pos * 3],
                    G = _byteArray[_initialLength + _pos * 3 + 1],
                    B = _byteArray[_initialLength + _pos * 3 + 2]
                };
                return color;
            }
            set
            {
                if (_initialLength + _pos * 3 > _byteArray.Length) throw new IndexOutOfRangeException();
                _byteArray[_initialLength + _pos * 3] = value.R;
                _byteArray[_initialLength + _pos * 3 + 1] = value.G;
                _byteArray[_initialLength + _pos * 3 + 2] = value.B;
            }
        }


    }
}
