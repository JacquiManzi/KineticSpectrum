using System;
using System.Collections.Generic;
using System.Linq;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    class ProgrammedPatternProvider : IPatternProvider
    {
        private readonly List<PatternStart> _patternStarts = new List<PatternStart>();
        private readonly Scene _scene;

        private int _endTime;

        public ProgrammedPatternProvider(Scene scene)
        {
            _scene = scene;
        }

        public IActivatable Simulation { get; set; }

        public IList<PatternStart> PatternStarts { get { return _patternStarts.AsReadOnly(); } }
        public int EndTime { get { return _endTime; } }
        public void Clear()
        {
            _patternStarts.Clear();
            _endTime = 0;
        }

        public void Reset()
        {
            new ColorBuilder(_patternStarts).RandomizeColoring();
        }

        public void MarkComplete(Pattern pattern)
        {
            /*no-op*/
        }

        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            Pattern pattern = _scene.Patterns.First(p => p.Name.Equals(patternName));
            return AddPattern(pattern, startTime, id, priority);
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            PatternStart pStart = new PatternStart(Simulation, _scene, id, startTime, pattern, priority);
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
                if (patternStart.StartTime >= shiftAfterTime)
                {
                    patternStart.StartTime += timeToShift;
                }
                _endTime = Math.Max(_endTime, patternStart.EndTime);
            }
        }

        public void RemovePattern(int id)
        {
            for (int i = 0; i < _patternStarts.Count; i++)
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

        public IList<PatternStart> GetActive(int time)
        {
            List<PatternStart> starts = _patternStarts.Where(patternStart => patternStart.Applies(time)).ToList();
            starts.Sort(PatternStart.PriorityComparer);
            return starts;
        }
    }
}
