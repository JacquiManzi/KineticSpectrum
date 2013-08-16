using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffects
{
    public class ColorEffectDefinition
    {
        public readonly string Name;
        public readonly IDictionary<string, object> Properties;

        public ColorEffectDefinition(string name, IDictionary<string, object> properties )
        {
            Name = name;
            Properties = new Dictionary<string, object>(properties);
        }

        public const string ColorProperty = "Color";
        public const string ColorsProperty = "Colors";


        private static readonly IDictionary<string, object> TwoColors = new Dictionary<string, object>
                                                                            {
                                                                                {ColorsProperty, new List<int>
                                                                                    {
                                                                                        ColorUtil.ToInt(Colors.Red),
                                                                                        ColorUtil.ToInt(Colors.Blue)
                                                                                    }}
                                                                            }; 
        public static readonly ColorEffectDefinition RedFixed = new ColorEffectDefinition(FixedColor.EffectName, new Dictionary<string, object>
                                                                        {
                                                                            {
                                                                                ColorProperty,
                                                                                ColorUtil.ToInt(
                                                                                    Colors.Red)
                                                                            }
                                                                        });

        public static readonly ColorEffectDefinition GreenFixed = new ColorEffectDefinition(FixedColor.EffectName, new Dictionary<string, object>
                                                                        {
                                                                            {
                                                                                ColorProperty,
                                                                                ColorUtil.ToInt(
                                                                                    Colors.Green)
                                                                            }
                                                                        });
        public static readonly ColorEffectDefinition BlueFixed = new ColorEffectDefinition(FixedColor.EffectName, new Dictionary<string, object>
                                                                        {
                                                                            {
                                                                                ColorProperty,
                                                                                ColorUtil.ToInt(
                                                                                    Colors.Blue)
                                                                            }
                                                                        });

        public static readonly ColorEffectDefinition DefaultRainbow = new ColorEffectDefinition("Rainbow", new Dictionary<string, object>());

        public static readonly ColorEffectDefinition DefaultFade = new ColorEffectDefinition("ColorFade", TwoColors);
        public static readonly ColorEffectDefinition DefaultChasing = new ColorEffectDefinition("ChasingColors", TwoColors);

        public static readonly IEnumerable<ColorEffectDefinition> AllDefaults = new List<ColorEffectDefinition>{RedFixed, DefaultRainbow, DefaultFade, DefaultChasing}; 
    }
}
