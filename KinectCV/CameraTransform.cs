using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RevKitt.ks.KinectCV
{
    public  class CameraTransform
    {
        public static readonly IDictionary<String,CameraTransform> Transforms;

        static CameraTransform()
        {
            Transforms = new Dictionary<String,CameraTransform>();
            CameraTransform t = new CameraTransform("779722104935", new Matrix3D());
            Transforms.Add(t.Serial, t);
            //t = new CameraTransform("135875605138", new Matrix(1, 0, 0, 1, -253.866666666667, -59.7333333333333));
//            Matrix3D matrix = new Matrix3D(0.999061302243977, -0.043318752966462, 0.043318752966462, 0.999061302243977,
//                                       -258.133333333333, -64);
            Matrix3D matrix = new Matrix3D();
            matrix.Invert();
            t = new CameraTransform("135875605138", matrix);
            Transforms.Add(t.Serial, t);
        }

        public readonly String Serial;
        public readonly Matrix3D Transform;

        public CameraTransform(String serial, Matrix3D transform)
        {
            Serial = serial;
            Transform = transform;
        }

        public Blob TransformBlob(Blob blob)
        {
            return new Blob
                       {
                           Name = "Transformed",
                                       TopLeft = Transform.Transform(blob.TopLeft),
                                       BottomRight = Transform.Transform(blob.BottomRight),
                                       Center = Transform.Transform(blob.Center)
                                   };
        }


        public Point3D DoTransform(Point3D point)
        {
            return Transform.Transform(point);
        }
    }
}
