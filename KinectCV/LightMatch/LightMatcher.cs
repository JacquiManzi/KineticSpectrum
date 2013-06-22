using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using KineticControl;

namespace RevKitt.ks.KinectCV.LightMatch
{
    internal class LightMatcher
    {
        private readonly List<LightAddress> _unassignedAddresses;
        private readonly LightSystem _lightSystem;

        private readonly List<Light> _unKnownLights = new List<Light>();
        private readonly List<Light> _lights = new List<Light>();
        private readonly List<Light> _ignoredLights = new List<Light>();

        private SearchMethod _lightSearch;

        internal List<LightAddress> UnassignedAddresses { get { return _unassignedAddresses; } } 
        internal List<Light> PossibleLights { get { return _unKnownLights; } }
        internal List<Light> KnownLights { get { return _lights; } }
        internal List<Light> NotLights { get { return _ignoredLights; } }
        internal LightSystem LightSystem { get { return _lightSystem; } }

        public LightMatcher(LightSystem lightSystem)
        {
            _lightSystem = lightSystem;
            _unassignedAddresses = lightSystem.LightAddresses;
        }

        public List<Light> UpdateLights(IList<Blob> blobs, Image<Rgba, byte> image)
        {
            List<Blob> remaining = new List<Blob>(blobs);
            List<Light> missingLights = FindMissingLights(_unKnownLights, remaining);
            missingLights.AddRange(FindMissingLights(_lights, remaining));
            FindMissingLights(_ignoredLights, remaining);

            foreach (var blob in remaining)
            {
                _unKnownLights.Add(new Light(blob));
            }

            if (_lightSearch != null && !_lightSearch.Done)
            {
                IEnumerable<Light> foundLights = _lightSearch.SearchUpdate(new HashSet<Light>(missingLights));
                foreach(var light in foundLights)
                {
                    _unassignedAddresses.Remove(light.Address);
                    _unKnownLights.Remove(light);
                    _lights.Add(light);
                }
                
            }
            else if (_lightSearch == null)
            {
                _lightSearch = new BinarySearchMethod(this);
            }
            return new List<Light>(_lights.Union(_unKnownLights));
        }

        public bool IsSearching { get { return _lightSearch != null && !_lightSearch.Done; } }

        public void BeginSearch()
        {
            if(_lightSearch != null && _lightSearch.Done)
                _lightSearch = new BinarySearchMethod(this);
        }


        private List<Light> FindMissingLights(IEnumerable<Light> lights, IList<Blob> blobs)
        {
            List<Light> missing = new List<Light>();
            foreach (var light in lights)
            {
                bool found = false;
                for (int i = 0; i < blobs.Count; i++)
                {
                    Blob blob = blobs[i];
                    if (light.Contains(blob))
                    {
                        blobs.RemoveAt(i);
                        light.LightBlob = blob;
                        found = true;
                    }
                }
                if (!found)
                {
                    missing.Add(light);
                    light.Brightness = 0;
                }
            }
            return missing;
        }


    }
}
