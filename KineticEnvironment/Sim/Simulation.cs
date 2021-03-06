﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IDictionary<LightAddress, LightState> _stateMap = new Dictionary<LightAddress, LightState>();
        private readonly List<LightState> _lightState = new List<LightState>();
        private readonly List<LEDNode> _lights = LightSystemProvider.Lights; 

        private readonly Timer _updateTimer;
        private int _currentTime;
        private KinectPlugin _plugin;

        public readonly String Name;

        public static Simulation TempSimulation(Scene scene)
        {
            return new Simulation(TEMP_NAME, new ProgrammedPatternProvider(scene));
        }

        public static Simulation ProgrammedSimulation(String name, Scene scene)
        {
            return new Simulation(name, new ProgrammedPatternProvider(scene));
        }

        public static Simulation GenerativeSimulation(Scene scene)
        {
            return new Simulation(TEMP_NAME, new GenerativePatternProvider(scene));
        }

        public IPatternProvider PatternProvider { get; private set; }

        public Simulation(IPatternProvider patternProvider) : this(TEMP_NAME, patternProvider)
        { }

        public Simulation(String name, IPatternProvider patternProvider)
        {
            Name = name;
            PatternProvider = patternProvider;
            PatternProvider.Simulation = this;
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
            PatternProvider.Clear();
            _currentTime = 0;
            foreach (var state in _lightState)
            {
                state.Color = Colors.Black;
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
        public double Speed { get; set; }
        public KinectPlugin Plugin { get { return _plugin; } set { _plugin = value; } }


        public IList<PatternStart> ActivePatterns { get; private set; }

        public int Time
        {
            get { return _currentTime; }
            set
            {
                if (value >= EndTime)
                    value = EndTime - 1;
                else if (value < 0)
                    value = 0;
                if (_currentTime > value)
                {
                    PatternProvider.Reset();
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

        public int EndTime { get { return PatternProvider.EndTime; } }

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
            if (newTime > EndTime)
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
                //todo: I shouldn't have to make a copy here, but there was a concurrent modification
                //exception when getting the appiers below. Make sure there is no concurrent access...
                IList<PatternStart> nowActive = new List<PatternStart>(PatternProvider.GetActive(time));
                HandlePatternChanges(new List<PatternStart>(ActivePatterns), nowActive);
                ActivePatterns = nowActive;

                IList<IEffectApplier> appliers = nowActive.SelectMany(start => start.GetApplier(time)).ToList();

                foreach (var light in _lights)
                {
                    light.Color = ColorUtil.Transparent;
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

        private void ApplyForTime(IEnumerable<IEffectApplier> appliers, int time)
        {
            var groupApply = appliers.GroupBy(a => a.Group.Name).ToList();

//            HandleBackground(time, groupApply);

            foreach (var light in _lights)
            {
                bool lightDone = false;
                foreach (IGrouping<string, IEffectApplier> effectAppliers in groupApply)
                {
                    foreach (IEffectApplier applier in effectAppliers)
                    {
                        IColorEffect colorEffect = applier.GetEffect(light);
                        Color nodeColor = applier.GetNodeColor(light, colorEffect);
                        if (WriteForeground(light, nodeColor))
                        {
                            lightDone = true;
                            break;
                        }
                    }
                    if (lightDone) break;
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

        private bool WriteForeground(LEDNode light, Color color)
        {
            light.Color = ColorUtil.Interpolate(color, light.Color);
            if (light.Color.A >= 230)
            {
                return true;
            }
            return false;
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
