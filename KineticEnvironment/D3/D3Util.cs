using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using ILNumerics;

namespace RevKitt.KS.KineticEnvironment.D3
{

    public enum Dimention
    {
        //Absolute dimentions
        X,Y,Z,
        
        //planar dimentions
        PX,PY,PZ
    }

    public class Extent
    {
        private readonly double _min;
        private readonly double _max;
        private readonly Vector3D _minPoint;
        private readonly Vector3D _maxPoint;

        public double Min { get { return _min; } }
        public double Max { get { return _max; } }
        public Vector3D MinPoint { get { return _minPoint; } }
        public Vector3D MaxPoint { get { return _maxPoint; } }

        public Extent(double min, Vector3D minPoint, double max, Vector3D maxPoint)
        {
            _min = min;
            _max = max;
            _minPoint = minPoint;
            _maxPoint = maxPoint;
        }
    }

    class D3Util
    {
        private const int NUM_DIM = 3;
        public static ILArray<double> Convert(Vector3D from)
        {
            return ILMath.array(from.X, from.Y, from.Z);
        }

        public static Vector3D Convert(ILArray<double> from)
        {
            if(from.S[0] != 3)
                throw new ArgumentException("Input size is not a valid 3D vector.");
            return new Vector3D(from.GetValue(0), from.GetValue(1), from.GetValue(2));
        }

        public static IList<ILArray<double>> Convert(IList<Vector3D> from)
        {
            return from.Select(Convert).ToList();
        }

        public static Vector3D Center(IList<Vector3D> points)
        {
            return points.Aggregate(new Vector3D(), (s, vec) => s + vec)/points.Count();
        }

        public static void FindFurthest(IList<Vector3D> vecList, out Vector3D one, out Vector3D two)
        {
            if(vecList == null || vecList.Count == 0)
                throw new ArgumentException("Vector list cannot be empty or null");
            one = two = vecList.First();
            double distance = 0;
            foreach (var first in vecList)
            {
                foreach (var second in vecList)
                {
                    double aDist = (first - second).Length;
                    if ( aDist > distance)
                    {
                        one = first;
                        two = second;
                        distance = aDist;
                    }
                }
            }
        }

        public static double DotProduct(Vector3D v1, Vector3D v2 )
        {
            return (v1.X*v2.X + v2.Y*v2.Y + v1.Z*v2.Z);
        }

        public static double Distance(Vector3D v1, Vector3D v2 )
        {
            return (v1 - v2).Length;
        }


        public static Boolean Is3D(IList<Vector3D> vectors)
        {
            Plane plane = FindPlane(vectors);
            return Is3D(vectors, plane);
        }

        public static Boolean Is3D(IList<Vector3D> vectors, Plane plane)
        {
            double maxR = vectors.Max(v => Distance(plane.Projection(v), plane.Center));
            double maxZ = vectors.Max(v => plane.Distance(v));

            return maxR < maxZ*4;
        }

        public static Extent GetExtents(IList<Vector3D> vectors, Plane plane)
        {
            if(vectors == null || vectors.Count == 0)
                throw new ArgumentException("Vector list cannot be empty or null");

            double min = Double.MaxValue;
            double max = Double.MinValue;
            Vector3D minPoint = new Vector3D();
            Vector3D maxPoint = new Vector3D();

            foreach (Vector3D vector in vectors)
            {
                double planeDist = plane.Distance(vector);
                if (min > planeDist)
                {
                    min = planeDist;
                }
                if (max < planeDist)
                {
                    max = planeDist;
                }
            }

            return new Extent(min,minPoint, max, maxPoint);
        }


        public static Plane FindPlane(IList<Vector3D> vectors)
        {
            return FindPlane(Convert(vectors));
        }

        public static Plane FindPlane(IList<ILArray<double>> vectors)
        {
            var mean = FindMean(vectors);

            ILArray<double> A = vectors.Aggregate(ILMath.zeros(NUM_DIM, NUM_DIM),
                                    (s, vecs)=>ILMath.multiply(ILMath.subtract(mean,vecs),
                                                                ILMath.subtract(mean, vecs).T));

            ILArray<double> o = ILMath.zeros<double>(0);
            ILArray<double> eigenVecs = ILMath.eigSymm(A, o);

            int minIndex = 0;
            for (int i = 0; i < NUM_DIM; i++)
            {
                if (eigenVecs.GetValue(i, i) < eigenVecs.GetValue(minIndex, minIndex))
                    minIndex = i;
            }

            ILArray<double> evec = o.a[ILMath.full, minIndex];
            return new Plane(Convert(evec), Convert(mean));
        }

        public static Vector3D FindMean(IList<Vector3D> vectors)
        {
            return Convert(FindMean(Convert(vectors)));
        }

        public static ILArray<double> FindMean(IList<ILArray<double>> vectors)
        {
            ILArray<double> sum = ILMath.zeros(NUM_DIM);
            sum = vectors.Aggregate(sum, (s, vecs) => ILMath.add(s, vecs));
            ILArray<double> mean = ILMath.array(sum.GetValue(0)/vectors.Count,
                sum.GetValue(1)/vectors.Count,
                sum.GetValue(2)/vectors.Count);
            return mean;
        }
    }
}
