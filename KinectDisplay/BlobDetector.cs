using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    
    class BlobDetector
    {
        private readonly bool[,] _temp;
        private readonly int _minArea;
        private readonly int _height, _width;

        public BlobDetector(int height, int width, int minArea)
        {
            _temp = new bool[height,width];
            _height = height;
            _width = width;
            _minArea = minArea;
        }

        public IList<Blob> FindBlobs(Image<Gray, byte> image)
        {
            if(image.Height != _height || image.Width != _width)
                throw new ArgumentException("Image dimentions don't match initialized dimentions");

            IList<Blob> bList = new List<Blob>();

            byte[,,] img = image.Data;
            for(int y=0; y<_height; y++)
                for(int x=0; x<_width; x++)
                {
                    bool doFill = true;
                    if (img[y, x, 0] != byte.MaxValue) continue;
                    foreach(Blob existingBlob in bList)
                    {
                        if(existingBlob.ContainsPoint(x,y))
                        {
                            x = existingBlob.XMax + 1;
                            doFill = false;
                            break;
                        }
                    }
                    if(doFill)
                    {
                        Blob newBlob = FloodFill(y, x, img);
                        if(newBlob.Area >= _minArea)
                            bList.Add(newBlob);
                    }
                }
            return bList;
        }

        private Blob FloodFill(int ymin, int xmin, byte[,,] img)
        {
            int height = img.GetLength(0), width = img.GetLength(1);
            int xmax = xmin, ymax = ymin;

            Queue<Point> toProcess = new Queue<Point>();
            toProcess.Enqueue(new Point(xmin,ymin));
            int x, y, area = 0;
           
            while(toProcess.Count > 0)
            {
                Point p = toProcess.Dequeue();
                x = p.X;
                y = p.Y;
                area++;
                _temp[y, x] = true;
                if (x > xmax) xmax = x;
                if (y > ymax) ymax = y;
                if (x < xmin) xmin = x;

                if (x > 0 && img[y, x - 1, 0] == byte.MaxValue && !_temp[y, x - 1])
                {
                    _temp[y, x - 1] = true;
                    toProcess.Enqueue(new Point(x - 1, y));
                }
                if (x < width - 1 && img[y, x + 1, 0] == byte.MaxValue && !_temp[y, x + 1])
                {
                    _temp[y, x + 1] = true;
                    toProcess.Enqueue(new Point(x + 1, y));
                }
                if (y > 0 && img[y - 1, x, 0] == byte.MaxValue && !_temp[y - 1, x])
                {
                    _temp[y - 1, x] = true;
                    toProcess.Enqueue(new Point(x, y - 1));
                }
                if (y < height - 1 && img[y + 1, x, 0] == byte.MaxValue && !_temp[y + 1, x])
                {
                    _temp[y + 1, x] = true;
                    toProcess.Enqueue(new Point(x, y + 1));
                }
            }

            byte[,,] bData = new byte[ymax-ymin+1,xmax-xmin+1,1];
            for (y = ymin; y <= ymax; y++)
                for (x = xmin; x <= xmax; x++ )
                {
                    if(_temp[y,x])
                    {
                        bData[y - ymin, x - xmin, 0] = byte.MaxValue;
                        _temp[y, x] = false;
                    }
                }


            return new Blob()
            {
                XMin = xmin,
                YMin = ymin,
                XMax = xmax,
                YMax = ymax,
                Area = area,
                BlobImg = new Image<Gray, byte>(bData)
            };
        }
    }
}
