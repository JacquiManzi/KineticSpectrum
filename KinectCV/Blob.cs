using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Emgu.CV;
using Emgu.CV.Structure;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace RevKitt.ks.KinectCV
{
    public class Blob : IEquatable<Blob>
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

        public static Image<Rgba, byte> AddOutline(Image<Rgba, byte> baseImg, IEnumerable<Blob> blobs, MCvFont font)
        {
            return AddOutline(baseImg, blobs, font, Colors.White);
        }

        public static Image<Rgba, byte> AddOutline(Image<Rgba, byte> baseImg, IEnumerable<Blob> blobs, MCvFont font, Color color)
        {
            Rgba rgbaColor = new Rgba(color.R, color.G, color.B, color.A);
            foreach (Blob blob in blobs)
            {
                baseImg.Draw(Rectangle.Round(new RectangleF(blob.XMin, blob.YMin, blob.XMax - blob.XMin, blob.YMax - blob.YMin)), rgbaColor, 2);
                baseImg.Draw(blob.Name, ref font, new System.Drawing.Point(blob.XMax, blob.YMax), rgbaColor);
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
                else
                {
                    SetProperties(other);
                }
            }
        }

        private void SetProperties(Blob blob)
        {
            Name = blob.Name;
            born = blob.born;
        }

        public String Name { get;  set; }
        //public Image<Gray, byte> BlobImg { get; internal set; }

        public Point3D BottomRight { 
            get
            {
                return new Point3D(XMax, YMax, ZCenter);
            }
            internal set 
            { 
                XMax = (int)value.X;
                YMax = (int)value.Y;
            } 
        }

        public Point3D TopLeft
        {
            get
            {
                return new Point3D(XMin, YMin, ZCenter);
            } 
            internal set 
            {
                XMin = (int)value.X;
                YMin = (int)value.Y;
            }
        }

        public int XMax { get;  set; }
        public int YMax { get;  set; }
        public int XMin { get;  set; }
        public int YMin { get;  set; }

        public int ZMin { get;  set; }
        public int ZMax { get;  set; }
        public int Area { get;  set; }
        public int Brightness { get; set; }

        public Point3D Center { get;  set; }
//        public int XCenter { get; internal set; }
        public int ZCenter { get;  set; }
        //public int YCenter { get; internal set; }

        public List<Point3D> Verticies
        {
            get
            {
                return new List<Point3D>
                           {
                               CreatePoint(XMin, YMin, ZMin),
                               CreatePoint(XMin, YMin, ZMax),
                               CreatePoint(XMin, YMax, ZMin),
                               CreatePoint(XMin, YMax, ZMax),
                               CreatePoint(XMax, YMin, ZMin),
                               CreatePoint(XMax, YMin, ZMax),
                               CreatePoint(XMax, YMax, ZMin),
                               CreatePoint(XMax, YMax, ZMax)
                           };
            }
        }

        public Point3D Vertex
        {
            get { return CreatePoint(Center.X, Center.Y, Center.Z); }
        }

        public List<Point4D> Faces
        {
            get
            {
                return new List<Point4D>
                           {
                               new Point4D(1, 2, 4, 3),
                               new Point4D(1, 2, 6, 5),
                               new Point4D(1, 3, 7, 5),
                               new Point4D(5, 6, 8, 7),
                               new Point4D(3, 4, 8, 7),
                               new Point4D(2, 4, 8, 6)
                           };
            }
        }

        private Point3D CreatePoint(double x, double y, double z)
        {
            double angleX = -(x*28.5*2/640 - 28.5)*Math.PI/180;
            double angleY = -(y*21.5*2/480 - 21.5)*Math.PI/180;

            double px = Math.Sin(angleX)*z;
            double pz = Math.Sin(angleY)*z;

            double py = Math.Sqrt(Math.Pow(z,2) - Math.Pow(px, 2) - Math.Pow(pz, 2));

            return new Point3D(px/1000,py/1000,pz/1000);
        }

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

        public bool Equals(Blob other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return XMax == other.XMax && YMax == other.YMax && XMin == other.XMin && YMin == other.YMin && Area == other.Area && Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Blob) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = XMax;
                hashCode = (hashCode*397) ^ YMax;
                hashCode = (hashCode*397) ^ XMin;
                hashCode = (hashCode*397) ^ YMin;
                hashCode = (hashCode*397) ^ Area;
                hashCode = (hashCode*397) ^ Center.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Blob left, Blob right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Blob left, Blob right)
        {
            return !Equals(left, right);
        }
    }
}
