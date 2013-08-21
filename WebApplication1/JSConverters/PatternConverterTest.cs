using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media;
using NUnit.Framework;
using RevKitt.KS.KineticEnvironment.Effects;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffect;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace WebApplication1.JSConverters
{
    [TestFixture]
    public class PatternConverterTest
    {

        public static Pattern TestPattern
        {
            get
            {
                Pattern pattern = new Pattern();
                pattern.EffectName = "Sweep";
                pattern.Groups = new List<string>();
                pattern.Name = "Test Pattern";
                pattern.EffectProperties = new EffectProperties();

                pattern.EffectProperties[PropertyDefinition.Duration.Name] = 5;
                pattern.EffectProperties[PropertyDefinition.RepeatCount.Name] = 3;
                pattern.EffectProperties[PropertyDefinition.RepeatMethod.Name] = RepeatMethods.Restart;
                pattern.EffectProperties[PropertyDefinition.Ordering.Name] = GroupOrderings.Reverse;
                pattern.EffectProperties[PropertyDefinition.Easing.Name] = Easings.Sinusoidal;
                pattern.EffectProperties[Sweep.StartColor.Name] = new FixedColor(Colors.Green);
                pattern.EffectProperties[Sweep.EndColor.Name] = new FixedColor(Colors.BlueViolet);
                return pattern;
            }
        }

        [Test]
        public void TestWritePattern()
        {
            string patternString = Serializer.ToString(TestPattern);
            patternString.GetHashCode();
        }

        [Test]
        public void TestReadPattern()
        {
            string patternString = Serializer.ToString(TestPattern);
            var pattern = Serializer.FromString<Pattern>(patternString);
        }
    }
}