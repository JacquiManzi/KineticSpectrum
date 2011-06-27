﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Point = System.Windows.Point;

namespace KinectDisplay
{
    public class Blob
    {
        private DateTime born = DateTime.Now;

        public Boolean ContainsPoint(int x, int y)
        {
            return x <= XMax && x >= XMin && y <= YMax && y >= YMin;
        }

        public double OverlapPercent(Blob b)
        {
            return OverlapDirection(XMax, XMin, b.XMax, b.XMin)*
                   OverlapDirection(YMax, YMin, b.YMax, b.YMin);
        }

        public double Score(Blob b)
        {
            return (OverlapPercent(b) + b.OverlapPercent(this))*100;
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
                baseImg.Draw(blob.Name, ref font, new System.Drawing.Point(blob.XMax,blob.YMax), new Gray(255) );  
            }
            return baseImg;
        }

        public Blob ClosestBlob(IList<Blob> blobs)
        {
            Blob closestMatch = null;
            double maxScore = 0;

            foreach (Blob blob in blobs)
            {
                double score = Score(blob);
                if (score > maxScore)
                {
                    maxScore = score;
                    closestMatch = blob;
                }
            }
            return closestMatch;
        }

        public Blob ClosestBlob(CameraTransform transform, IList<Blob> blobs)
        {
            Blob closestMatch = null;
            double maxScore = 0;

            foreach (Blob blob in blobs)
            {
                double score = Score(transform.TransformBlob(blob));
                if (score > maxScore)
                {
                    maxScore = score;
                    closestMatch = blob;
                }
            }
            return closestMatch; 
        }
        
        public void MergeBlobs(Blob other)
        {
            if(Name != other.Name)
            {
                if(born < other.born)
                {
                    other.SetProperties(this);
                }
            }
        }

        private void SetProperties(Blob blob)
        {
            Name = blob.Name;
            born = blob.born;
        }

        public String Name { get; internal set; }
        //public Image<Gray, byte> BlobImg { get; internal set; }

        public Point BottomRight { 
            get
            {
                return new Point(XMax, YMax);
            }
            internal set 
            { 
                XMax = (int)value.X;
                YMax = (int)value.Y;
            } 
        }

        public Point TopLeft
        {
            get
            {
                return new Point(XMin, YMin);
            } 
            internal set 
            {
                XMin = (int)value.X;
                YMin = (int)value.Y;
            }
        }

        public int XMax { get; internal set; }
        public int YMax { get; internal set; }
        public int XMin { get; internal set; }
        public int YMin { get; internal set; }

        public int ZMin { get; internal set; }
        public int ZMax { get; internal set; }
        public int Area { get; internal set; }

        public Point Center { get; internal set; }
//        public int XCenter { get; internal set; }
        public int ZCenter { get; internal set; }
        //public int YCenter { get; internal set; }


        public void AbsorbDimentionsOf(Blob other)
        {
            BottomRight = other.BottomRight;
            TopLeft = other.TopLeft;
            ZMin = other.ZMin;
            ZMax = other.ZMax;
            ZCenter = other.ZCenter;
            Center = other.Center;
            Area = other.Area;
        }
    }
}