using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using ILNumerics;
using KineticControl;

namespace RevKitt.ks.KinectCV.Scenes
{
    public class Scene
    {
        private readonly List<SceneView> _views = new List<SceneView>();

        public void AddScene(IEnumerable<Light> lights )
        {
            _views.Add(new SceneView(lights));
        }

        public void WriteObjs(String fileName, LightSystem lightSystem)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine("#");
            writer.WriteLine("# Object Writer for RevKitt Kinetic Spectrum");
            writer.WriteLine("#");
            writer.WriteLine();

            IEnumerable<Light> lights = _views.SelectMany(view => view.Lights);
            foreach (var light in lights)
            {
                var scenePoint = new ScenePoint {Address = light.Address, Vertex = light.LightBlob.Vertex};
                WritePoint(writer, scenePoint, lightSystem.GetNetworkAddress(light.Address));
            }
            writer.Close();
        }

        public void WriteScene(String fileName, LightSystem lightSystem)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine("#");
            writer.WriteLine("# Object Writer for RevKitt Kinetic Spectrum");
            writer.WriteLine("#");

            IEnumerable<ScenePoint> scenePoints = BuildScene();
            foreach (var point in scenePoints)
            {
                WritePoint(writer, point, lightSystem.GetNetworkAddress(point.Address));
            }
            writer.Close();  
        }

        private IEnumerable<ScenePoint> BuildScene()
        {
            if (_views.Count == 0)
                return Enumerable.Empty<ScenePoint>();
            List<ScenePoint> scene = new List<ScenePoint>(_views[0].Points);
            for(int i=1;i <_views.Count; i++)
            {
                AddToScene(scene, new List<ScenePoint>(_views[i].Points) );
            }
            return scene;
        }

        private void AddToScene(List<ScenePoint> scene, List<ScenePoint> newPoints)
        {
            Func<IEnumerable<ScenePoint>,Predicate<ScenePoint>> findContains = n=> s => n.Contains(s, new LambdaComparer<ScenePoint>((a, b) => a.Address.Equals(b.Address)));
            List<ScenePoint> intersectScene = new List<ScenePoint>(scene.FindAll(findContains(newPoints)).Distinct());
            intersectScene.Sort();
            List<ScenePoint> intersectNew = new List<ScenePoint>(newPoints.FindAll(findContains(scene)).Distinct());
            intersectNew.Sort();

            ILArray<double> transform;
            ILArray<double> rotMatrix = CreateMatrix(BuildMatrix(intersectNew), BuildMatrix(intersectScene), out transform);

            foreach (var newPoint in newPoints.Except(intersectNew))
            {
                var newScenePoint = new ScenePoint();
                newScenePoint.Address = newPoint.Address;
                newScenePoint.Matrix = ILMath.multiply(rotMatrix, newPoint.Matrix) + transform;
                scene.Add(newScenePoint);
            }
        }

        internal static ILArray<double> CreateMatrix(ILArray<double> fromPoints, ILArray<double> toPoints, out ILArray<double> r )
        {
            int dim = 3;
            int noPoints = fromPoints.Size[1];
            if(!fromPoints.Size.Equals(toPoints.Size))
            {
                throw new ArgumentException("Matricies must have equal dimentions. From points has: " + fromPoints.Size + "  To points has: " + toPoints.Size);
            }
            ILArray<double> ones = ILMath.ones(1, noPoints);
            ILArray<double> normVec = ones/noPoints;

            ILArray<double> fromCentroid = ILMath.multiply(fromPoints,normVec.T);
            ILArray<double> toCentroid = ILMath.multiply(toPoints,normVec.T);

            ILArray<double> fromCentered = fromPoints - ILMath.multiply(fromCentroid,ones);
            ILArray<double> toCentered = toPoints - ILMath.multiply(toCentroid,ones);

            ILArray<double> pdm = ILMath.zeros(dim, noPoints);
            for(int i=0; i<noPoints; i++)
            {
                for(int j=0; j<dim; j++)
                {
                    pdm.SetValue(normVec.GetValue(0,i)*fromCentered.GetValue(j,i), j,i);
                }
            }

            ILArray<double> C = ILMath.multiply(pdm,toCentered.T);
            ILArray<double> W = ILMath.empty();
            ILArray<double> V = ILMath.empty();

            ILMath.svd(C, V, W);
            W = W.a; V = V.a;

            ILArray<double> U = ILMath.multiply(W, V.T);


            r = toCentroid - ILMath.multiply(U,fromCentroid);

            return U;
        }

        internal static ILArray<double> BuildMatrix(IList<ScenePoint> points )
        {
            double[] data = new double[3*points.Count];
            for(int i=0; i<points.Count; i++)
            {
                Point3D point = points[i].Vertex;
                data[3*i] = point.X;
                data[3*i+1] = point.Y;
                data[3*i+2] = point.Z;
            }
            return ILMath.array(data, 3, points.Count);
        }

        

        private void WritePoint(StreamWriter writer, ScenePoint point, String networkAddress)
        {
            writer.WriteLine();
            Point3D vertex = point.Vertex;
            writer.WriteLine(networkAddress + ":" + point.Address + "\t" + vertex.X + "\t" + vertex.Y + "\t" + vertex.Z);
        }

        private void WriteLight(StreamWriter writer, Light light, ref int vertexCount)
        {
            writer.WriteLine();
            writer.WriteLine("g " + light.LightBlob.Name);
            writer.WriteLine();
            foreach (var vertex in light.LightBlob.Verticies)
            {
                writer.WriteLine("v " + vertex.X + " " + vertex.Y + " " + vertex.Z);
            }
            writer.WriteLine();
            foreach (var face in light.LightBlob.Faces)
            {
                writer.WriteLine("f " + (face.X + vertexCount) + " " + (face.Y + vertexCount) + " "
                                      + (face.Z + vertexCount) + " " + (face.W + vertexCount));
            }
            writer.WriteLine();
            writer.WriteLine();
            vertexCount += light.LightBlob.Verticies.Count;
        }

        public int ViewCount { get { return _views.Count; } }

    }
}
