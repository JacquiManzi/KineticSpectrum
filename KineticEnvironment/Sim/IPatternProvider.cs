using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    public interface IPatternProvider
    {
        IList<PatternStart> PatternStarts { get; }
        int EndTime { get; }

        void Clear();
        void Reset();

        IActivatable Simulation { get; set; }
        void MarkComplete(Pattern pattern);
        PatternStart AddPattern(string patternName, int startTime, int id, int priority);
        PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority);
        void ShiftAfter(int shiftAfterTime, int timeToShift);
        void RemovePattern(int id);
        IList<PatternStart> GetActive(int time);
    }

    public class CompositePatternProvider : IPatternProvider
    {
        private readonly IPatternProvider _first;
        private readonly IPatternProvider _second;

        public CompositePatternProvider(IPatternProvider first, IPatternProvider second)
        {
            _first = first;
            _second = second;
        }

        public IList<PatternStart> PatternStarts { get { return _first.PatternStarts; } }
        public int EndTime { get; private set; }
        public void Clear()
        {
            _first.Clear();
            _second.Clear();
        }

        public void Reset()
        {
            _first.Reset();
            _second.Reset();
        }

        public IActivatable Simulation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void MarkComplete(Pattern pattern)
        {
            throw new NotImplementedException();
        }

        public PatternStart AddPattern(string patternName, int startTime, int id, int priority)
        {
            PatternStart start = _first.AddPattern(patternName, startTime, id, priority);
            _second.AddPattern(patternName, startTime, id, priority);
            return start;
        }

        public PatternStart AddPattern(Pattern pattern, int startTime, int id, int priority)
        {
            PatternStart start = _first.AddPattern(pattern, startTime, id, priority);
            _second.AddPattern(pattern, startTime, id, priority);
            return start;
        }

        public void ShiftAfter(int shiftAfterTime, int timeToShift)
        {
            _first.ShiftAfter(shiftAfterTime, timeToShift);
            _second.ShiftAfter(shiftAfterTime, timeToShift);
        }

        public void RemovePattern(int id)
        {
            _first.RemovePattern(id);
            _second.RemovePattern(id);
        }

        public IList<PatternStart> GetActive(int time)
        {
            throw new NotImplementedException();
        }
    }
}
