using System;

namespace KinectAPI
{
    public class DepthRow
    {
        private readonly int _offset;
        public readonly int Width;
        private readonly IntPtr _section;

        

        internal DepthRow(IntPtr memSection, int width, int offset)
        {
            _offset = offset;
            Width = width;
            _section = memSection;
        }

        public byte this[int column]
        {
            get
            {
                unsafe
                {
                    if (column >= Width) throw new IndexOutOfRangeException();
                    byte* section = (byte*)_section.ToPointer();
                    byte* ptr =  section + _offset + column;
                    return *ptr;
                }
            }
        }
    }
}