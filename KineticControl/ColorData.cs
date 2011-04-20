using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;

namespace KineticControl
{
    public class ColorData : IEnumerable<Color>
    {
        private readonly byte[] _byteArray;
        private readonly int _initialLength;
        private readonly int _length;

        public ColorData(byte[] initialData, int length)
        {
            _initialLength = initialData.Length;
            _length = length;
            _byteArray = new byte[_initialLength + HexStrings.addressOff.Length/2];
            _byteArray.Initialize();
            initialData.CopyTo(_byteArray, 0);
        }

        public int Count { get { return (_byteArray.Length - _initialLength)/3; } }

        public byte[] Bytes { get { return _byteArray; } }

        public Color this[int pos]
        {
            get
            {
                if(_initialLength + pos*3 >= _byteArray.Length) throw new IndexOutOfRangeException();
                Color color = new Color
                                  {
                                      R = _byteArray[_initialLength + pos*3],
                                      G = _byteArray[_initialLength + pos*3 + 1],
                                      B = _byteArray[_initialLength + pos*3 + 2]
                                  };
                return color;
            }
            set
            {
                if (_initialLength + pos * 3 > _byteArray.Length) throw new IndexOutOfRangeException();
                _byteArray[_initialLength + pos*3] = value.R;
                _byteArray[_initialLength + pos*3 + 1] = value.G;
                _byteArray[_initialLength + pos*3 + 2] = value.B;
            }
        }

        public IEnumerator<Color> GetEnumerator()
        {
           int length = Count;
           for(int i=0; i<length; i++)
           {
               yield return this[i];
           }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
