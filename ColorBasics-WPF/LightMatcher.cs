using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using KinectDisplay;
using KineticControl;

namespace RevKitt.LightBuilder
{
    internal class LightMatcher
    {
        private readonly IList<LightAddress> _unassignedAddresses;
        private readonly LightSystem _lightSystem;

        private readonly List<Light> _unKnownLights = new List<Light>();
        private readonly List<Light> _lights = new List<Light>();

        private LightSearch _lightSearch;

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
            return _lights;
        }

        public bool IsSearching { get { return _lightSearch != null && !_lightSearch.Done; } }

        public void BeginSearch()
        {
            if(_lightSearch == null || _lightSearch.Done)
                _lightSearch = new LightSearch(this);
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
                }
            }
            return missing;
        }


        private class LightSearch
        {
            public Boolean Done { get; private set; }

            private int _currentIndex = 300;

            private readonly LightMatcher _matcher;

            private readonly HashSet<Light> _expectedMissing = new HashSet<Light>();
            private List<Light> _onNext = new List<Light>();

            private DateTime _lastUpdate = DateTime.Now;

            private bool _reset;

            public LightSearch(LightMatcher matcher)
            {
                _matcher = matcher;
                Done = false;
                Reset();
            }

            private void Reset()
            {
                foreach(var address in _matcher._lightSystem.LightAddresses)
                {
                    _matcher._lightSystem[address] = Colors.White;
                }
                _onNext = new List<Light>();
                _reset = true;
            }

            private void IndexOn(int index, bool on)
            {
                if (index < _matcher._unassignedAddresses.Count)
                {
                    Color color = on ? Colors.White : Colors.Black;
                    _matcher._lightSystem[_matcher._unassignedAddresses[index]] = color;
                }
            }

            public IEnumerable<Light> SearchUpdate(ISet<Light> missingLights)
            {
                DateTime now = DateTime.Now;
                if ((now - _lastUpdate) < TimeSpan.FromMilliseconds(500))
                    return Enumerable.Empty<Light>();
                _lastUpdate = now;

                if(_reset)
                {
                    //we just waited for a reset cycle with all on,
                    //so, let's turn one off and wait a cycle
                     IndexOn(_currentIndex, false);
                    _currentIndex++;
                    _reset = false;
                }

                if (_currentIndex > _matcher._unassignedAddresses.Count + 1)
                    Done = true;

                IndexOn(_currentIndex-1, true);
                IndexOn(_currentIndex, false);
                //remove the missing items we expect to be missing
                missingLights.ExceptWith(_expectedMissing);

                var backOn = FindBackOn(missingLights);

                //We just turned off one light, so it could be any of the remaining missing lights
                //so, let's add them to the list of lights we expect to be on next time
                _onNext = new List<Light>(missingLights);

                

                return backOn;
            }

            private IEnumerable<Light> FindBackOn(ISet<Light> missingLights)
            {
                // let's look at the lights we expect to be on this time (that were off last time). Any lights still missing
                //should be moved to the expected missing
                List<Light> backOn = new List<Light>();
                foreach (var light in _onNext)
                {
                    if (missingLights.Contains(light))
                        _expectedMissing.Add(light);
                    else
                        backOn.Add(light);
                }
                if (backOn.Count > 1)
                {
                    //OK, too many on. Let's reset and try again
                    Reset();
                    _currentIndex--;
                    backOn.Clear();
                }
                else if (backOn.Count == 1)
                {
                    backOn.First().Address = _matcher._unassignedAddresses[_currentIndex - 1];
                }
                else
                {
                    _currentIndex++;
                }
                return backOn;
            }
        }
    }
}
