using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    public class Blob
    {
        public Boolean ContainsPoint(int x, int y)
        {
            return x <= XMax && x >= XMin && y <= YMax && y >= YMin;
        }

        public double OverlapPercent(Blob b)
        {
            return OverlapDirection(XMax, XMin, b.XMax, b.XMin)*
                   OverlapDirection(YMax, YMin, b.YMax, b.YMin);
        }

        private static double OverlapDirection(int max, int min, int max2, int min2)
        {
            if (max < min2 || max2 < min) return 0;

            if (min2 >= min && max2 <= max)
                return ((double)(max2 - min2)) / (max - min);
            if (min2 > min)
                return ((double)(max - min2)) / (max - min);

            return Math.Min(((double)(max2 - min)) / (max - min), 1); 
        }

        public static Image<Gray, byte> AddOutline(Image<Gray, byte> baseImg, IEnumerable<Blob> blobs, MCvFont font)
        {
            foreach(Blob blob in blobs)
            {
                baseImg.Draw(Rectangle.Round(new RectangleF(blob.XMin, blob.YMin, blob.XMax-blob.XMin, blob.YMax-blob.YMin)), new Gray(255), 2 );
                baseImg.Draw(blob.Name, ref font, new Point(blob.XMax,blob.YMax), new Gray(255) );  
            }
            return baseImg;
        }

        public String Name { get; internal set; }
        public Image<Gray, byte> BlobImg { get; internal set; }

        public int XMax { get; internal set; }
        public int YMax { get; internal set; }
        public int XMin { get; internal set; }
        public int YMin { get; internal set; }
        public int ZMin { get; internal set; }
        public int ZMax { get; internal set; }
        public int Area { get; internal set; }

        public int XCenter { get; internal set; }
        public int ZCenter { get; internal set; }
        public int YCenter { get; internal set; }
    }
}
