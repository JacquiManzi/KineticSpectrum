using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Windows.Media.Media3D;
using ILNumerics;
using KineticControl;

namespace RevKitt.ks.KinectCV.Scenes
{
    class ScenePoint : IEquatable<ScenePoint>, IComparable<ScenePoint>
    {
        public Point3D Vertex { get; set; }
        public LightAddress Address { get; set; }

        public ILArray<double> Matrix
        {
            get
            {
                double[] data = {Vertex.X, Vertex.Y, Vertex.Z};
                return ILMath.array(data,3,1);
            }
            set
            {
                Vertex = new Point3D(value.GetValue(0), value.GetValue(1), value.GetValue(2));
            }
        } 


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ScenePoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Vertex.GetHashCode()*397) ^ Address.GetHashCode();
            }
        }

        public static bool operator ==(ScenePoint left, ScenePoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScenePoint left, ScenePoint right)
        {
            return !Equals(left, right);
        }

        public bool Equals(ScenePoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Vertex.Equals(other.Vertex) && Address.Equals(other.Address);
        }

        public int CompareTo(ScenePoint other)
        {
            int comp = Address.CompareTo(other.Address);
            return comp;
        }
    }
}
