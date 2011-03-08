using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectAPI
{
    public class DepthMatrix
    {
        private readonly IntPtr _section;
        public readonly int Width;
        public readonly int Height;

        internal DepthMatrix(IntPtr memorySection, int width, int height)
        {
            _section = memorySection;
            Width = width;
            Height = height;
        }

        public DepthRow this[int row]
        {
           get
           {
               if (row >= Height) throw new IndexOutOfRangeException();
               return new DepthRow(_section, Width, Width*row);
           } 
        }
    }
}
