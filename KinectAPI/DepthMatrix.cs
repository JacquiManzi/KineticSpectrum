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

    public class ShortDepthMatrix : DepthMatrix
    {
        public ShortDepthMatrix(IntPtr memorySection, int width, int height) : base(memorySection, width, height)
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

        public Image<Gray,short> AsImage()
        {
            Image<Gray,short> image = new Image<Gray, short>(Width, Height);
            int width = (Width * 2 + 3) & ~3;

 
            short[,,] data = new short[Height,width,1];
            unsafe
            {
                byte* section = (byte*) _section.ToPointer();
                byte byt = 0;

                for(int i=0; i<Height; i++)
                {
                    for(int j=0; j<Width; j++)
                    {
                        if(byt == 0)
                        {
                            data[i, j, 0] = (short) (*(section++) << 4);
                            byt = (*section++);
                            data[i, j, 0] += (short) (byt >> 4);
                        }
                        else
                        {
                            data[i, j, 0] = (short) (byt << 4);
                            data[i, j, 0] += *(section++);
                            byt = 0;
                        }
                    }
                }
            }

            image.Data = data;

            return image;
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

        public Image<Gray,byte> AsImage()
        {
          Image<Gray,Byte> image = new Image<Gray, byte>(Width, Height);
            int width = Width;
            if(Width % 4 != 0)
                width = Width + (4-Width%4);

             byte[,,] data = new byte[Height,width,1];
            unsafe
            {
                byte* section = (byte*) _section.ToPointer();
                for(int i=0; i<Height; i++)
                {
                    for(int j=0; j<Width; j++)
                    {
                        if (*section > 50)
                            data[i, j, 0] = *section;
                        else
                            data[i, j, 0] = 0;
                        section++;
                    }
                }
            }

            image.Data = data;

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

        public Image<Bgr, byte> AsImage()
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(Width, Height);

            byte[, ,] data = new byte[Height, Width, 3];
            unsafe
            {
                byte* section = (byte*)_section.ToPointer();
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        data[i, j, 0] = *(section++);
                        data[i, j, 1] = *(section++);
                        data[i, j, 2] = *(section++);
                        section++; //discard the last byte
                    }
                }
            }

            image.Data = data;

            return image;
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
