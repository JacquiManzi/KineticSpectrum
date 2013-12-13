using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Media;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;
using Timer = System.Timers.Timer;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public class Simulation : ISimulation
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
        private KinectPlugin _plugin;

        public readonly String Name;

        public Simulation(Scene scene) : this(TEMP_NAME, scene){ }

        public Simulation(String name, Scene scene)
        {
            Name = name;
            _scene = scene;
            _updateTimer = new Timer();
            _updateTimer.Elapsed += (o, args) => AdvanceTime();
            _updateTimer.Interval = 1000/60;
            Speed = 1;
            GC.KeepAlive(_updateTimer);

            foreach (var light in _lights)
            {
                var lState = new LightState {Address = light.Address, Color = light.Color};
                _stateMap[light.Address] = lState;
                _lightState.Add(lState);
            }
            LightSystemProvider.OnLightsUpdated += LightsUpdated;
            ActivePatterns = new List<PatternStart>();
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


        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            Pattern pattern = _scene.Patterns.First(p => p.Name.Equals(patternName));
            return AddPattern(pattern, startTime, id, priority);
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            PatternStart pStart = new PatternStart(this,_scene, id, startTime, pattern, priority);
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

        public void WriteRange(Stream stream, int start, int end)
        {
            var streamWriter = new StreamWriter(stream);
            if (end > EndTime || start < 0)
            {
                throw new ArgumentException("Range (" + start + ',' + end + ") exceeds Pattern Range (0," + EndTime + ").");
            }

            int steps = (end - start) * 30 / 1000;
            streamWriter.Write('[');
            for (int i = 0; i < steps; i++)
            {
                int time = start + i * 1000 / 30;
                Time = time;
                WriteState(streamWriter, time, LightState, i == steps - 1);
            }
            streamWriter.Write(']');
            streamWriter.Flush();
        }

        private static void WriteState(StreamWriter streamWriter, int time, IList<LightState> states, bool last)
        {
            streamWriter.Write('[');
            streamWriter.Write(time);
            streamWriter.Write(',');
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                LightAddress a = state.Address;
                streamWriter.Write(a.FixtureNo);
                streamWriter.Write(',');
                streamWriter.Write(a.PortNo);
                streamWriter.Write(',');
                streamWriter.Write(a.LightNo);
                streamWriter.Write(',');
                streamWriter.Write(ColorUtil.ToInt(state.Color));
                if (i < states.Count - 1)
                    streamWriter.Write(',');
            }
            streamWriter.Write(']');
            if (!last)
            {
                streamWriter.Write(',');
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

        public double Speed { get; set; }
        public KinectPlugin Plugin { get { return _plugin; } set { _plugin = value; } }

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
                if (_currentTime > value)
                {
                    new ColorBuilder(_patternStarts).RandomizeColoring();
                }
                _currentTime = value;
                if (value == 0)
                {
                    groupBackground.Clear();
                    groupBackgroundNext.Clear();
                }

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

        public IList<LightState> LightState
        {
            get { return _lightState.AsReadOnly(); }
        }

        private DateTime _lastRun = DateTime.Now;

        
        private void AdvanceTime()
        {
            DateTime thisRun = DateTime.Now;
            int newTime = Time + (int)(Speed * (thisRun-_lastRun).TotalMilliseconds);
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
                HandlePatternChanges(new List<PatternStart>(ActivePatterns), starts);
                ActivePatterns = starts;

                IList<IEffectApplier> appliers = starts.SelectMany(start => start.GetApplier(time)).ToList();

                foreach (var light in _lights)
                {
                    light.Color = ColorUtil.Empty;
                }
                ApplyForTime(appliers, time);
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

        private void HandlePatternChanges(IList<PatternStart> current, IList<PatternStart> update)
        {
           foreach (PatternStart start in update)
           {
               current.Remove(start);
           }
            if (current.Count > 0)
            {
                var effect = current.First().SampleEffect as Sweep;
                if (effect != null && effect.StartEffect != effect.EndEffect)
                {
                    _kinectState = (_kinectState == PluginMode.NoBack) ? 
                        PluginMode.NoFore : PluginMode.NoBack;
                }
            }

        }

        private readonly IDictionary<string, IEffectApplier> groupBackground = new Dictionary<string, IEffectApplier>();
        private readonly IDictionary<string, NextUp> groupBackgroundNext = new Dictionary<string, NextUp>();

        private void ApplyForTime(IList<IEffectApplier> appliers, int time)
        {
            var groupApply = appliers.GroupBy(a => a.Group.Name);

            HandleBackground(time, groupApply);

            foreach (var light in _lights)
            {
                foreach (IGrouping<string, IEffectApplier> effectAppliers in groupApply)
                {
                    bool backgroundApplied = false;
                    IEffectApplier background;
                    if (groupBackground.TryGetValue(effectAppliers.Key, out background))
                    {
                        Color nodeColor = background.GetNodeColor(light, background.EndColor);
                        WriteBackground(light, nodeColor);
                        backgroundApplied = true;
                    }
                    foreach (IEffectApplier applier in effectAppliers)
                    {
                        IColorEffect colorEffect = applier.GetEffect(light);
                        if (colorEffect != applier.StartColor)
                        {
                            Color nodeColor = applier.GetNodeColor(light, colorEffect);
                            WriteForeground(light, nodeColor);
                        }
                        else if (!backgroundApplied)
                        {
                            Color nodeColor = applier.GetNodeColor(light, colorEffect);
                            WriteBackground(light, nodeColor);
                        }
                        backgroundApplied = true;
                    }
                }
            }
        }

        private void WriteBackground(LEDNode light, Color color)
        {
            if (_plugin != null && _plugin.Enabled && 
                _kinectState == PluginMode.NoBack)
            {
                byte amount = _plugin.Applies(light.Position.X, light.Position.Y); 
                if (amount != 0)
                {
                    light.Color = ColorUtil.Interpolate(color, Colors.Black, amount/255.0);
                    return;
                }
            }
            light.Color = color;
        }

        private void WriteForeground(LEDNode light, Color color)
        {
            if (_plugin != null && _plugin.Enabled &&
                _kinectState == PluginMode.NoFore)
            {
                byte amount = _plugin.Applies(light.Position.X, light.Position.Y); 
                if (amount != 0)
                {
                    light.Color = ColorUtil.Interpolate(color, Colors.Black, amount/255.0);
                    return;
                }
            }
            light.Color = color;
        }

        private enum PluginMode {NoBack, NoFore}
        private PluginMode _kinectState = PluginMode.NoBack;

        private void HandleBackground(int time, IEnumerable<IGrouping<string, IEffectApplier>> groupApply)
        {
            foreach (IGrouping<string, IEffectApplier> effectAppliers in groupApply)
            {
                if (groupBackgroundNext.ContainsKey(effectAppliers.Key))
                {
                    int whenApply = groupBackgroundNext[effectAppliers.Key].AtTime;
                    if(whenApply <= time)
                    {
                        var nextUp = groupBackgroundNext[effectAppliers.Key];
                        groupBackgroundNext.Remove(effectAppliers.Key);
                        if (effectAppliers.Count() == 1)
                        {
                            groupBackgroundNext[effectAppliers.Key] = new NextUp(effectAppliers.First().EndTime);
                        }
                        if (nextUp.ToApply == null)
                            groupBackground.Remove(effectAppliers.Key);
                        else if(effectAppliers.Count() != 0)
                            groupBackground[effectAppliers.Key] = nextUp.ToApply;
                    }
                }
                if (effectAppliers.Count() > 1 && !groupBackgroundNext.ContainsKey(effectAppliers.Key))
                {
                    groupBackgroundNext[effectAppliers.Key] = new NextUp(effectAppliers.First());
                }
            }
        }

        private class NextUp
        {
            public readonly int AtTime;
            public readonly IEffectApplier ToApply;

            public NextUp(IEffectApplier applier)
            {
                AtTime = applier.EndTime;
                ToApply = applier;
            }

            public NextUp(int atTime)
            {
                AtTime = atTime;
            }
        }
    }
}
