using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffect
{
    public class ColorEffects
    {
        public static IColorEffect GetEffect(String effectName, IList<Color> colors)
        {
            if (!EffectDict.ContainsKey(effectName))
                throw new ArgumentException("No color effect exists with name '" + effectName + '\'');
            return EffectDict[effectName](colors);
        }

        private delegate IColorEffect ColorEffectFactory(IList<Color> colors);

        private static readonly IDictionary<string, ColorEffectFactory> EffectDict =
            new Dictionary<string, ColorEffectFactory>
                {
                    {ColorFade.EffectName, c => new ColorFade(c)},
                    {FixedColor.EffectName, c => new FixedColor(c.First())},
                    {ChasingColors.EffectName, c=>new ChasingColors(c)},
                    {ImageEffect.EffectName, c=>new ImageEffect()}
                };
    }
}
