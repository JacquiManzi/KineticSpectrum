using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace KinectDisplay
{
    public  class CameraTransform
    {
        public static readonly IDictionary<String,CameraTransform> Transforms;

        static CameraTransform()
        {
            Transforms = new Dictionary<String,CameraTransform>();
            CameraTransform t = new CameraTransform("779722104935", new Matrix());
            Transforms.Add(t.Serial, t);
            t = new CameraTransform("135875605138", new Matrix(1, 0, 0, 1, -253.866666666667, -59.7333333333333));
            Transforms.Add(t.Serial, t);
        }

        public readonly String Serial;
        public readonly Matrix Transform;

        public CameraTransform(String serial, Matrix transform)
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


        public Point DoTransform(Point point)
        {
            return Transform.Transform(point);
        }
    }
}
