using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Scenes;
using Timer = System.Timers.Timer;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class Simulation : IStateProvider
    {
        private readonly Scene _scene;
        private readonly List<PatternStart> _patternStarts = new List<PatternStart>();
        private int _idCounter = 1;
        private readonly IDictionary<LightAddress, LightState> _stateMap = new Dictionary<LightAddress, LightState>();
        private readonly List<LightState> _lightState = new List<LightState>();

        private readonly Timer _updateTimer;
        private int _currentTime;
        private int _endTime;

        public Simulation(Scene scene)
        {
            _scene = scene;
            _updateTimer = new Timer();
            _updateTimer.Elapsed += (o, args) => AdvanceTime();
            _updateTimer.Interval = 1000/60;
            GC.KeepAlive(_updateTimer);

            foreach (var light in LightSystemProvider.Lights)
            {
                var lState = new LightState() {Address = light.Address, Color = light.Color};
                _stateMap[light.Address] = lState;
                _lightState.Add(lState);
            }
        }

        public void Clear()
        {
            _patternStarts.Clear();
            _endTime = _currentTime = 0;
            foreach (var state in _lightState)
            {
                state.Color = Colors.Black;
            }
        }

        public IList<PatternStart> PatternStarts
        {
            get { return _patternStarts; }
        }


        public PatternStart AddPattern(string patternName, int startTime)
        {
            Pattern pattern = _scene.Patterns.First(p => p.Name.Equals(patternName));
            return AddPattern(pattern, startTime);
        }

        public PatternStart AddPattern(Pattern pattern, int startTime)
        {
            Interlocked.Increment(ref _idCounter);
            PatternStart pStart = new PatternStart(_scene, _idCounter, startTime, pattern );
            _endTime = Math.Max(_endTime, pStart.EndTime);
            _patternStarts.Add(pStart);
            _patternStarts.Sort(PatternStart.StartTimeComparer);
            return pStart;
        }

        public void ShiftAfter(int shiftAfterTime, int timeToShift)
        {
            _endTime = 0;
            foreach (var patternStart in _patternStarts)
            {
                if(patternStart.StartTime >= shiftAfterTime)
                {
                    patternStart.StartTime += timeToShift;
                }
                _endTime = Math.Max(_endTime, patternStart.EndTime);
            }
        }

        public void RemovePattern(int id)
        {
            for (int i = 0; i < _patternStarts.Count; i++ )
            {
                if (_patternStarts[i].Id == id)
                {
                    _patternStarts.RemoveAt(i);
                    UpdateEndTime();
                    return;
                }
            }
        }

        private void UpdateEndTime()
        {
            _endTime = 0;
            foreach (var patternStart in _patternStarts)
            {
                _endTime = Math.Max(_endTime, patternStart.EndTime);
            }
        }

        public IList<PatternStart> ActivePatterns { get; private set; }

        public int Time
        {
            get { return _currentTime; }
            set
            {
                if (value >= _endTime)
                    value = _endTime - 1;
                else if (value < 0)
                    value = 0;
                _currentTime = value;
                UpdateState(value);
            }
        }

        public bool IsPlaying
        {
            get { return _updateTimer.Enabled; }
            set
            {
                if (value)
                    _lastRun = DateTime.Now;
                _updateTimer.Enabled = value;
            }
        }

        public int EndTime { get { return _endTime; } }

        public IEnumerable<LightState> LightState
        {
            get { return _lightState.AsReadOnly(); }
        }

        private DateTime _lastRun = DateTime.Now;

        private void AdvanceTime()
        {
            DateTime thisRun = DateTime.Now;
            int newTime = Time + (int)(thisRun-_lastRun).TotalMilliseconds;
            _lastRun = thisRun;
            if (newTime > _endTime)
                newTime = 0;
            Time = newTime;
        }

        private void UpdateState(int time)
        {
            List<PatternStart> starts = _patternStarts.Where(patternStart => patternStart.Applies(time)).ToList();
            starts.Sort(PatternStart.PriorityComparer);
            ActivePatterns = starts;

            foreach (var light in LightSystemProvider.Lights)
            {
                light.Color = Colors.Black;
            }

            foreach (var patternStart in starts)
            {
                patternStart.Apply(time);
            }

            foreach (var light in LightSystemProvider.Lights)
            {
                _stateMap[light.Address].Color = light.Color;
            }
            LightSystemProvider.LightSystem.UpdateLights();
        }

    }
}
