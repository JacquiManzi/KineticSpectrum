using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Coloring;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;

namespace RevKitt.KS.KineticEnvironment.Sim
{
    internal class ColorBuilder
    {
        private IList<PatternStart> _patternStarts;

        internal ColorBuilder(List<PatternStart> starts)
        {
            _patternStarts = starts;
        }

        public IList<PatternStart> InternalStarts { get { return _patternStarts; } } 

        public IList<PatternStart> RandomizeColoring()
        {
            //original color
            Dictionary<PatternStart, IList<Color>> startToColor = new Dictionary<PatternStart, IList<Color>>();
            //keep track of the active patterns. Will contain new color
            List<PatternStart> active = new List<PatternStart>();

            foreach (var start in _patternStarts)
            {
                IList<Color> colors = EffectsToColors(start.SampleEffect.ColorEffects);
                startToColor[start] = colors;
                OrderedSet<Color> distinct = new OrderedSet<Color>();
                distinct.AddAll(colors);
                IDictionary<Color, Color> oldToNew = OldToNew(startToColor, active);
                Color randBase = distinct.FirstOrDefault(oldToNew.ContainsKey);
                IList<Color> newColors = randBase == default(Color)
                                            ? ColorPicker.PickColors(distinct.Count)
                                            : ColorPicker.PickColors(randBase, distinct.Count);
                IEnumerator<Color> newEnum = newColors.GetEnumerator();
                IEnumerator<Color> oldEnum = distinct.GetEnumerator();
                while(oldEnum.MoveNext())
                {
                    newEnum.MoveNext();
                    if (!oldToNew.ContainsKey(oldEnum.Current))
                    {
                        oldToNew[oldEnum.Current] = newEnum.Current;
                    }
                }
                start.ApplyColors(colors.Select(c=>oldToNew[c]).ToList());
                active.RemoveAll(s => s.EndTime <= start.StartTime);
                active.Add(start);
            }
            return _patternStarts;
        }

        private IDictionary<Color, Color> OldToNew(Dictionary<PatternStart, IList<Color>> startToColor, List<PatternStart> active)
        {
            Dictionary<Color,Color> colorMap = new Dictionary<Color, Color>();
            foreach (var start in active)
            {
                IList<Color> endColors = EffectsToColors(start.SampleEffect.ColorEffects);
                IList<Color> startColors = startToColor[start];
                for (int i = 0; i < endColors.Count; i++)
                {
                    colorMap[startColors[i]] = endColors[i];
                }
            }
            return colorMap;
        }

        private IList<Color> EffectsToColors(IEnumerable<IColorEffect> effects)
        {
            return effects.SelectMany(e => e.Colors).ToList();
        }
    }
}
