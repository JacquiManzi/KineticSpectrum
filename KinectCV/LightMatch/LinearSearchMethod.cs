

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace RevKitt.ks.KinectCV.LightMatch
{
    class LinearSearchMethod : SearchMethod
    {
        public Boolean Done { get; private set; }

            private int _currentIndex = 300;

            private readonly LightMatcher _matcher;

            private HashSet<Light> _initialyUnknown;
            private readonly HashSet<Light> _expectedMissing = new HashSet<Light>();
            private List<Light> _onNext = new List<Light>();

            private DateTime _lastUpdate = DateTime.Now;

            private bool _reset;

            public LinearSearchMethod(LightMatcher matcher)
            {
                _matcher = matcher;
                _initialyUnknown = new HashSet<Light>(matcher.PossibleLights);
                Done = false;
                Reset();
            }

            private void Reset()
            {
                _lastUpdate = DateTime.Now;
                foreach(var address in _matcher.LightSystem.LightAddresses)
                {
                    _matcher.LightSystem[address] = Colors.White;
                }
                _onNext = new List<Light>();
                _reset = true;
            }

            private void IndexOn(int index, bool on)
            {
                if (index < _matcher.UnassignedAddresses.Count)
                {
                    Color color = on ? Colors.White : Colors.Black;
                    _matcher.LightSystem[_matcher.UnassignedAddresses[index]] = color;
                }
            }


            //Each light address will pass through this code Three times
            //On its first pass, the light will be turned off. At this point it is the currentIndex.
            //On its second pass, we will attempt to narrow down which light it is, by taking all the lights
            //  that have turned off since the last pass and saving them in a list (onNext) to be inspected next time
            //On its third pass we find all lights that were missing in the last pass and checking if any of them
            // are now present. If all of them are still not present, then the light isn't in the field of view.
            // if one is present, it must be the right one. If two are present, then there was a chance blink
            // that has messed everything up, so we must return to the first pass for that light.
            public IEnumerable<Light> SearchUpdate(ISet<Light> missingLights)
            {
                DateTime now = DateTime.Now;
                if ((now - _lastUpdate) < TimeSpan.FromMilliseconds(500))
                    return Enumerable.Empty<Light>();
                _lastUpdate = now;

                //First pass through, so we don't have to worry about the other two steps
                //Just turn off the current index
                if(_reset)
                {
                    //we just waited for a reset cycle with all on,
                    //so, let's turn one off and wait a cycle
                     IndexOn(_currentIndex, false);
                    _currentIndex++;
                    _reset = false;
                    return Enumerable.Empty<Light>();
                }

                if (_currentIndex > _matcher.UnassignedAddresses.Count + 1)
                {
                    Done = true;
                    _initialyUnknown.IntersectWith(_matcher.PossibleLights);
                    _matcher.NotLights.AddRange(_initialyUnknown);
                    _matcher.PossibleLights.RemoveAll(l => _initialyUnknown.Contains(l));
                }
                    

                IndexOn(_currentIndex-1, true);
                IndexOn(_currentIndex, false);
                //remove the missing items we expect to be missing
                AdjustMissing(missingLights);
                

                var backOn = FindBackOn(missingLights);

                //We just turned off one light, so it could be any of the remaining missing lights
                //so, let's add them to the list of lights we expect to be on next time
                _onNext = new List<Light>(missingLights);

                return backOn;
            }

            private void AdjustMissing(ISet<Light> missing)
            {
                foreach(var expected in _expectedMissing)
                {
                   if(missing.Contains(expected))
                   {
                       _matcher.PossibleLights.Remove(expected);
                   }
                   else if(expected.IsUnknown)
                   {//TODO: Watch out for this, it might cause problems with lights that are just too far away
                       _matcher.PossibleLights.Remove(expected);
                       _matcher.NotLights.Add(expected);
                   }
                }
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
                    _currentIndex -= 2;
                    backOn.Clear();
                    missingLights.Clear();
                }
                else if (backOn.Count == 1)
                {
                    backOn.First().Address = _matcher.UnassignedAddresses[_currentIndex - 2];
                }
                else
                {
                    _currentIndex++;
                }
                return backOn;
            }
        
    }
}
