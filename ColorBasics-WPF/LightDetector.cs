using System;
using System.Collections.Generic;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;
using KinectDisplay;

namespace RevKitt.LightBuilder
{
    class LightDetector
    {
        private readonly bool[,] _temp;
        private readonly int _minArea;
        private readonly int _height, _width;

        public delegate bool ColorDetector(byte r, byte g, byte b);

        public LightDetector(int height, int width, int minArea)
        {
            _temp = new bool[height,width];
            _height = height;
            _width = width;
            _minArea = minArea;
        }

        public Image<Gray, byte> Mask(Image<Rgba,byte> image )
        {
            int minVal = Math.Max(100,ImageMax(image)-50);
            var mask = new Image<Gray, byte>(image.Width, image.Height);
            byte[,,] mData = mask.Data;
            byte[,,] data = image.Data;

            for(int y=0; y<_height; y++)
                for(int x=0; x<_width; x++)
                {
                    int brightness = data[y, x, 0] + data[y, x, 1] + data[y, x, 2];
                    if ( brightness > minVal)
                        mData[y, x, 0] = (byte) (brightness/3);
                    else
                        mData[y, x, 0] = 0;
                }
            mask._Erode(1);
            mask._Dilate(2);
            return mask;
        }

        private int ImageMax(Image<Rgba, byte> image )
        {
            int max = 0;
            byte[,,] data = image.Data;


            for(int y=0; y<_height; y++)
                for(int x=0; x<_width; x++)
                {
                    int val = data[y, x, 0] + data[y, x, 1] + data[y, x, 2];
                    if (val > max)
                        max = val;
                }
            return max;
        }

        public IList<Blob> FindBlobs(Image<Gray, byte> maskImage, Image<Rgba, byte> colorImage )
        {
            if(maskImage.Height != _height || maskImage.Width != _width)
                throw new ArgumentException("Image dimentions don't match initialized dimentions");

            IList<Blob> bList = new List<Blob>();

            byte[,,] maskData = maskImage.Data;
            byte[,,] colorData = colorImage.Data;

            for(int y=0; y<_height; y++)
                for(int x=0; x<_width; x++)
                {
                    if(maskData[y,x,0] != 0)
                    {
                        bool doFill = true;
                        foreach (Blob existingBlob in bList)
                        {
                            if (existingBlob.ContainsPoint(x, y))
                            {
                                x = existingBlob.XMax + 1;
                                doFill = false;
                                break;
                            }
                        }
                        if (doFill)
                        {
                            Blob newBlob = FloodFill(y, x, maskData, colorData);
                            if (newBlob.Area >= _minArea)
                                bList.Add(newBlob);
                        }
                    }
                   
                }
            return bList;
        }

        

        private Blob FloodFill(int ymin, int xmin, byte[,,] maskData, byte[,,] imageData)
        {
            int height = maskData.GetLength(0), width = maskData.GetLength(1);
            int xmax = xmin, ymax = ymin;

            Queue<Point> toProcess = new Queue<Point>();
            toProcess.Enqueue(new Point(xmin,ymin));
            int x, y, area = 0;
            int ysum;
            int xsum = ysum = 0;
            int brightness = 0;
           
            while(toProcess.Count > 0)
            {
                Point p = toProcess.Dequeue();
                x =(int) p.X; y =(int) p.Y;
                area++;
                brightness += imageData[y, x, 0] + imageData[y, x, 0] + imageData[y, x, 0];
                _temp[y, x] = true;
                if (x > xmax) xmax = x;
                if (y > ymax) ymax = y;
                if (x < xmin) xmin = x;
                
                xsum += x; ysum += y; 

                if (x > 0 && !_temp[y, x - 1] && maskData[y, x - 1,0] != 0)
                {
                    _temp[y, x - 1] = true;
                    toProcess.Enqueue(new Point(x - 1, y));
                }
                if (x < width - 1 && !_temp[y, x + 1] && maskData[y, x + 1,0] !=0)
                {
                    _temp[y, x + 1] = true;
                    toProcess.Enqueue(new Point(x + 1, y));
                }
                if (y > 0 && !_temp[y - 1, x] && maskData[y - 1, x,0] !=0 )
                {
                    _temp[y - 1, x] = true;
                    toProcess.Enqueue(new Point(x, y - 1));
                }
                if (y < height - 1 && !_temp[y + 1, x] && maskData[y + 1, x,0] != 0)
                {
                    _temp[y + 1, x] = true;
                    toProcess.Enqueue(new Point(x, y + 1));
                }
            }

            //byte[,,] bData = new byte[ymax-ymin+1,xmax-xmin+1,1];
            for (y = ymin; y <= ymax; y++)
                for (x = xmin; x <= xmax; x++ )
                {
                    if(_temp[y,x])
                    {
                        //bData[y - ymin, x - xmin, 0] = byte.MaxValue;
                        _temp[y, x] = false;
                    }
                }

            return new Blob
            {
                XMin = xmin,
                YMin = ymin,
                XMax = xmax,
                YMax = ymax,
                Area = area,
                Brightness = brightness / area,
// ReSharper disable PossibleLossOfFraction
                Center = new Point(xsum/area, ysum/area)
// ReSharper restore PossibleLossOfFraction
            };
        }
    }
}
