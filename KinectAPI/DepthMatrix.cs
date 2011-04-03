using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectAPI
{
    public abstract class DepthMatrix
    {
        protected readonly IntPtr _section;
        public readonly int Width;
        public readonly int Height;

        internal DepthMatrix(IntPtr memorySection, int width, int height)
        {
            _section = memorySection;
            Width = width;
            Height = height;
        }

        

        
    }

    public class ByteDepthMatrix : DepthMatrix
    {
        public ByteDepthMatrix(IntPtr memorySection, int width, int height) : base(memorySection, width, height)
        {
        }

        public ByteDepthRow this[int row]
        {
            get
            {
                if (row >= Height) throw new IndexOutOfRangeException();
                return new ByteDepthRow(_section, Width, Width * row);
            }
        }

        public Image<Gray,Byte> asImage()
        {
          Image<Gray,Byte> image = new Image<Gray, byte>(Width, Height, CameraRef.CalculateStride(CameraType.DepthCorrected8, Width), _section);
            return image;
        }
    }

    public class IntDepthMatrix : DepthMatrix
    {
        public IntDepthMatrix(IntPtr memorySection, int width, int height)
            : base(memorySection, width, height)
        {
        }

        public IntDepthRow this[int row]
        {
            get
            {
                if (row >= Height) throw new IndexOutOfRangeException();
                return new IntDepthRow(_section, Width, Width * row);
            }
        }
    }

    public class DistanceDepthMatrix : DepthMatrix
    {
        public DistanceDepthMatrix(IntPtr memorySection, int width, int height)
            : base(memorySection, width, height)
        {
        }

        public DistanceDepthRow this[int row]
        {
            get
            {
                if (row >= Height) throw new IndexOutOfRangeException();
                return new DistanceDepthRow(_section, Width, Width * row);
            }
        }
    }
}
