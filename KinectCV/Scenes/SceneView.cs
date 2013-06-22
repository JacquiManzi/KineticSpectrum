using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace RevKitt.ks.KinectCV.Scenes
{
    class SceneView
    {
        private readonly List<ScenePoint> _scenePoints;
        private readonly List<Light> _sceneLights;

        internal SceneView(IEnumerable<Light> lights)
        {
            _scenePoints = new List<ScenePoint>();
            _sceneLights = new List<Light>();
            foreach (var light in lights)
            {
                if (!light.IsUnknown)
                {
                    _scenePoints.Add(new ScenePoint {Address = light.Address, Vertex = light.LightBlob.Vertex});
                    _sceneLights.Add(new Light(light));
                }
            }
        }

        public IEnumerable<Light> Lights { get { return _sceneLights; } }
        public IEnumerable<ScenePoint> Points { get { return _scenePoints; } } 

    }
}
