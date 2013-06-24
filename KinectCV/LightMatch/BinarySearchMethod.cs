using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using KineticControl;

namespace RevKitt.ks.KinectCV.LightMatch
{
    class BinarySearchMethod : SearchMethod
    {
        public Boolean Done { get; private set; }

        private readonly int _numPasses;
        private int _power;
        private int _passCount = -1;

        private readonly List<LightAddress> _set1;
        private readonly List<LightAddress> _set2;

        private readonly List<Light> _startingLights;
        private readonly LightSystem _system;

        private readonly bool[][] _lightState;

        private DateTime _lastUpdate = DateTime.Now;

        private bool _doneFirst = false;

        public BinarySearchMethod(LightMatcher matcher)
        {
            _set1 = new List<LightAddress>(matcher.UnassignedAddresses.FindAll(a => a.LightNo%2 == 0));
            _set2 = new List<LightAddress>(matcher.UnassignedAddresses.FindAll(a => a.LightNo%2 == 1));
            _system = matcher.LightSystem;
            _startingLights = new List<Light>(matcher.PossibleLights);

            _numPasses = (int)Math.Log(CalcPow(), 2) * 2;
            _lightState = new bool[_startingLights.Count][];

            foreach (var address in _system.LightAddresses)
            {
                _system[address] = Colors.White;
            }
        }

        private int CalcPow()
        {
            return nextPowerOfTwo(Math.Max(_set1.Count, _set2.Count));
        }

        private void Reset()
        {
            foreach (var address in _system.LightAddresses)
            {
                _system[address] = Colors.White;
            }
            for (int i = 0; i < _startingLights.Count; i++ )
            {
                _lightState[i] = new bool[_numPasses];
            }
            _passCount = 0;
            _power = CalcPow();
            
        }

        private void IndexOn(List<LightAddress> set, int index, bool on)
        {
            if (index < set.Count)
            {
                Color color = on ? Colors.White : Colors.Black;
                _system[set[index]] = color;
            }
        }

        public IEnumerable<Light> SearchUpdate(ISet<Light> missingLights)
        {
            DateTime now = DateTime.Now;
            if ((now - _lastUpdate) < TimeSpan.FromMilliseconds(500))
                return Enumerable.Empty<Light>();
            _lastUpdate = now;

            //if this is our first run, get the lights in their proper state and return
            if(_passCount == -1)
            {
                Reset();
                SetLights(_set1, _power, false);
                return Enumerable.Empty<Light>();
            }

            //Detect the state of each of the possible lights in the scene and record that state
            ProcessLights();

            //Move on to getting ready for the next pass
            _passCount++;
            bool oddPass = _passCount%2 == 1;
            if (_passCount > 0 && !oddPass)
                _power = _power/2;

            //if our power is 1, then there is nothing to be done with the current
            //set. Let's figure out what lights we've detected from this set
            //and get the nest set started
            if(_power == 1)
            {
                IEnumerable<Light> updatedLights = CalculateLights(_doneFirst ? _set2 : _set1);
                Reset();

                if (!_doneFirst)
                {
                    _doneFirst = true;
                    SetLights(_set2, _power, false);
                }   
                else Done = true;

                _passCount = 0;
                return updatedLights;
            }

            List<LightAddress> currentSet = _doneFirst ? _set2 : _set1;
            //set the light state for the pass
            SetLights(currentSet, _power, oddPass);

            //return any lights we've determined
            return Enumerable.Empty<Light>();
        }

        private int CalcValue(bool[] bools)
        {
            int val = 0;
            for(int i=bools.Count() - 1; i>=0 ; i--)
            {
                if(bools[i])
                    val |= 1 << i;
            }
            return val;
        }

        private bool Matches(int index, bool[] bools)
        {
            int power = (int) Math.Pow(2, _numPasses/2);
            for(int i=0; i<_numPasses; i++)
            {
                bool oddPass = i%2 == 1;
                if(bools[i] && !IsIndexOn(index, power, i%2==1))
                    return false;
                if (oddPass)
                    power /= 2;
                
            }
            return true;
        }

        private IEnumerable<Light> CalculateLights(List<LightAddress> lightAddresses)
        {
            List<Light> matchedLights = new List<Light>();
            for (int i=0; i<_startingLights.Count; i++)
            {
                bool[] lightState = _lightState[i];
                for(int j=0; j<lightAddresses.Count; j++)
                {
                    if (!Matches(j, lightState)) continue;

                    Light light = _startingLights[i];
                    light.Address = lightAddresses[j];
                    matchedLights.Add(light);
                    break;
                }

            }
            return matchedLights;
        }

        private void SetLights(List<LightAddress> set, int power, bool oddPass )
        {
            for(int i=0; i< set.Count; i++)
            {
                bool indexOn = IsIndexOn(i, power, oddPass);
                IndexOn(set, i, indexOn);
            }
        }

        private void ProcessLights()
        {
            for(int i=0; i<_startingLights.Count; i++)
            {
                _lightState[i][_passCount] = _startingLights[i].SensedOn;
            }
        }

        private bool IsIndexOn(int index, int currentPower, bool oddPass)
        {
            if (index % currentPower < currentPower/2)
                return oddPass;
            return !oddPass;
        }

        private int nextPowerOfTwo(int n)
        {
            n--;
            n |= n >> 1;   // Divide by 2^k for consecutive doublings of k up to 32,
            n |= n >> 2;   // and then or the results.
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n++;           // The result is a number of 1 bits equal to the number
            // of bits in the original number, plus 1. That's the
            // next highest power of 2.
            return n;
        }
    }
}
