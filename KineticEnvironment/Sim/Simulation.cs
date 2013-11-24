using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Scenes;
using Timer = System.Timers.Timer;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class Simulation : IStateProvider
    {
        private const String TEMP_NAME = "___TEMP__SIMULATION";
        private readonly Scene _scene;
        private readonly List<PatternStart> _patternStarts = new List<PatternStart>();
        private readonly IDictionary<LightAddress, LightState> _stateMap = new Dictionary<LightAddress, LightState>();
        private readonly List<LightState> _lightState = new List<LightState>();
        private readonly List<LEDNode> _lights = LightSystemProvider.Lights; 

        private readonly Timer _updateTimer;
        private int _currentTime;
        private int _endTime;

        public readonly String Name;

        public Simulation(Scene scene) : this(TEMP_NAME, scene){ }

        public Simulation(String name, Scene scene)
        {
            Name = name;
            _scene = scene;
            _updateTimer = new Timer();
            _updateTimer.Elapsed += (o, args) => AdvanceTime();
            _updateTimer.Interval = 1000/60;
            GC.KeepAlive(_updateTimer);

            foreach (var light in _lights)
            {
                var lState = new LightState {Address = light.Address, Color = light.Color};
                _stateMap[light.Address] = lState;
                _lightState.Add(lState);
            }
            LightSystemProvider.OnLightsUpdated += LightsUpdated;
        }

        private void LightsUpdated(bool added)
        {
            if (added)
            { // if they've been added check all against the current set
                ISet<LightAddress> addresses = new HashSet<LightAddress>(_lights.Select(l=>l.Address));
                foreach (LEDNode node in LightSystemProvider.Lights)
                {
                    if (!addresses.Contains(node.Address))
                    {
                        node.IsActive = IsActive;
                        _lights.Add(node);
                        var lState = new LightState {Address = node.Address, Color = node.Color};
                        _lightState.Add(lState);
                        _stateMap[node.Address] = lState;
                    }
                }
            }
            else
            {// if they've been removed check current set against all
                ISet<LightAddress> addresses = new HashSet<LightAddress>(LightSystemProvider.Lights.Select(l=>l.Address));

                for (int i = 0; i < _lights.Count; )
                {
                    if (addresses.Contains(_lights[i].Address))
                    {
                        i++;
                    }
                    else
                    {
                        LightAddress toRemove = _lights[i].Address;
                        _lights.RemoveAt(i);
                        var lState = _stateMap[toRemove];
                        _stateMap.Remove(toRemove);
                        _lightState.Remove(lState);
                    }
                }
            }
        }

        public IEnumerable<LEDNode> Nodes { get { return _lights; } }
        public bool IsActive { get; set; }

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


        public PatternStart AddPattern(string patternName, int startTime, int id)
        {
            Pattern pattern = _scene.Patterns.First(p => p.Name.Equals(patternName));
            return AddPattern(pattern, startTime, id);
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id)
        {
            PatternStart pStart = new PatternStart(this,_scene, id, startTime, pattern );
            _endTime = Math.Max(_endTime, pStart.EndTime);
            _patternStarts.RemoveAll(start => start.Id == id);
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

        private volatile bool _inProcessing = default(bool);

        /// <summary>
        /// Guard method to determine if processing should begin. This will prevent more than
        /// one thread from entering a block that is tested via this. Note _inProcessing should
        /// be set to false when done.
        /// </summary>
        /// <returns>true if it's safe to enter processing block, false otherwise</returns>
        private bool EnterProcessing()
        {
            lock (this)
            {
                bool wasProcessing = _inProcessing;
                if (!wasProcessing)
                    _inProcessing = true;
                return !wasProcessing;
            }
        }
        
        private void UpdateState(int time)
        {
            if (!EnterProcessing()) return;

            try
            {
                List<PatternStart> starts = _patternStarts.Where(patternStart => patternStart.Applies(time)).ToList();
                starts.Sort(PatternStart.PriorityComparer);
                ActivePatterns = starts;

                foreach (var light in _lights)
                {
                    light.Color = ColorUtil.Empty;
                }
                foreach (var patternStart in starts)
                {
                    patternStart.Apply(time);
                }
                foreach (var light in _lights)
                {
                    _stateMap[light.Address].Color = light.Color;
                }
                if (IsActive)
                {
                    LightSystemProvider.UpdateLights();
                }
            }
            finally
            {
                _inProcessing = false;
            }
        }

    }
}
