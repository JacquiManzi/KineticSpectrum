using System;
using System.Collections.Generic;
using System.IO;
using RevKitt.KS.KineticEnvironment.Interact;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    class GenerativePatternProvider : IPatternProvider
    {
        private readonly List<PatternStart> _patternStarts = new List<PatternStart>();
        private readonly PatternGenerator _patternGenerator;
        private readonly Scene _scene;

        private KinectPatternStart _kinectPattern;

        public GenerativePatternProvider(Scene scene)
        {
            _scene = scene;
            _patternGenerator = new PatternGenerator(scene);
        }

        public IActivatable Simulation { get; set; }
        public IList<PatternStart> PatternStarts { get { return _patternStarts.AsReadOnly(); } }
        public int EndTime { get { return int.MaxValue; } }

        public void Clear()
        {
            _patternStarts.Clear();
        }

        public void Reset()
        {
            _patternStarts.Clear();
        }

        public void MarkComplete(Pattern pattern)
        {
            foreach (var start in PatternStarts)
            {
                if (start.Pattern == pattern)
                {
                    _patternStarts.Remove(start);
                    return;
                }
            }
        }

        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            throw new NotImplementedException();
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            throw new NotImplementedException();
        }

        public void ShiftAfter(int shiftAfterTime, int timeToShift)
        {
            throw new NotImplementedException();
        }

        public void RemovePattern(int id)
        {
            throw new NotImplementedException();
        }

        public IList<PatternStart> GetActive(int time)
        {
            _patternStarts.RemoveAll(p => p.EndTime < time);
            _patternStarts.AddRange(_patternGenerator.GetPatterns(time, Simulation, _patternStarts.Count, 2));
            if (_kinectPattern != null)
                return new List<PatternStart>(_patternStarts) {_kinectPattern};
            return _patternStarts;
        }

        public void WritePatterns(StreamWriter writer)
        {
            _patternGenerator.WriteParameters(writer);
        }

        public void ReadPatterns(string parameters)
        {
            _patternGenerator.ReadParameters(parameters);
            if (Plugin != null)
            {
                _kinectPattern = new KinectPatternStart(Simulation, _scene, _plugin);
            }
        }

        private KinectPlugin _plugin;
        public KinectPlugin Plugin
        {
            get { return _plugin; }
            set
            {
                _plugin = value;
            }
        }
    }
}
