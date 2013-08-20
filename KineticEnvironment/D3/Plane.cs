using System;
using System.Windows.Media.Media3D;

namespace RevKitt.KS.KineticEnvironment.D3
{
    class Plane
    {
        public static readonly Plane PlaneX = new Plane(new Vector3D(1, 0, 0), new Vector3D(0, 0, 0));
        public static readonly Plane PlaneY = new Plane(new Vector3D(0, 1, 0), new Vector3D(0, 0, 0));
        public static readonly Plane PlaneZ = new Plane(new Vector3D(0, 0, 1), new Vector3D(0, 0, 0));

        private readonly Vector3D _normal;
        private readonly Vector3D _centerPoint;

        public Plane(Vector3D normal, Vector3D point)
        {
            normal = new Vector3D(normal.X, normal.Y, normal.Z);
            normal.Normalize();
            _normal = normal;
            _centerPoint = new Vector3D(point.X, point.Y, point.Z);
        }

        public Plane WithPoint(Vector3D point)
        {
            return new Plane(_normal, point);
        }

        public double Distance(Vector3D point)
        {
            return (CalcD(_normal, point) - D)/
                   Math.Sqrt(Math.Pow(_normal.X, 2) +
                             Math.Pow(_normal.Y, 2) +
                             Math.Pow(_normal.Z, 3));
        }

        public Vector3D Projection(Vector3D point)
        {
            double v = (D - D3Util.DotProduct(point, _normal))/D3Util.DotProduct(_normal, _normal);
            return point + _normal*v;
        }

        private static double CalcD(Vector3D normal, Vector3D point)
        {
            return -( normal.X * point.X +
                      normal.Y * point.Y +
                      normal.Z * point.Z );
        }

        public Plane Inverted {
            get { return new Plane(new Vector3D(-_normal.X, -_normal.Y, -_normal.Z), _centerPoint); }
        }

        public Vector3D Normal { get { return new Vector3D(_normal.X, _normal.Y, _normal.Z); } }
        public Vector3D Center { get { return new Vector3D(_centerPoint.X, _centerPoint.Y, _centerPoint.Z); } }

        public double X { get { return _normal.X; } }
        public double Y { get { return _normal.Y; } }
        public double Z { get { return _normal.Z; } }

        private double _dCache = Double.NaN;
        public double D
        {
            //return the cached value if already calculated
            get { return Double.IsNaN(_dCache)? _dCache = CalcD(_normal, _centerPoint) : _dCache; }
        }
    }
}
