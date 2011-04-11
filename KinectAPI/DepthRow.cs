using System;

namespace KinectAPI
{
    public abstract class DepthRow
    {
        protected readonly int _offset;
        public readonly int Width;
        protected readonly IntPtr _section;

        

        internal DepthRow(IntPtr memSection, int width, int offset)
        {
            _offset = offset;
            Width = width;
            _section = memSection;
        }
    }

    public class ByteDepthRow: DepthRow
    {
        public ByteDepthRow(IntPtr memSection, int width, int offset) : base(memSection, width, offset)
        {
        }

        public byte this[int column]
        {
            get
            {
                unsafe
                {
                    if (column >= Width) throw new IndexOutOfRangeException();
                    byte* section = (byte*)_section.ToPointer();
                    byte* ptr =  section + _offset*sizeof(byte) + column*sizeof(byte);
                    return *ptr;
                }
            }
        }
    }

    public class IntDepthRow: DepthRow
    {
       public IntDepthRow(IntPtr memSection, int width, int offset) : base(memSection, width, offset)
       {
       }

        public int this[int column]
        {
            get
            {
                unsafe
                {
                    if (column >= Width) throw new IndexOutOfRangeException();
                    int* section = (int*)_section.ToPointer();
                    int* ptr =  section + _offset*sizeof(int) + column*sizeof(int);
                    return *ptr;
                }
            }
        } 
    }

    public class DistanceDepthRow : DepthRow
    {
        public DistanceDepthRow(IntPtr memSection, int width, int offset)
            : base(memSection, width, offset)
        {
        }

        public float this[int column]
        {
            get
            {
                int val;
                unsafe
                {
                    if (column >= Width) throw new IndexOutOfRangeException();
                    int* section = (int*)_section.ToPointer();
                    int* ptr = section + _offset * sizeof(int) + column * sizeof(int);
                    val =  *ptr;
                }
                return KinectDevice.RawDepthToMeters(val);
            }
        }
    }
}