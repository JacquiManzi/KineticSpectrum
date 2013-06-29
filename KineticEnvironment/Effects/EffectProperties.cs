using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects.ColorEffects;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Tweening;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public class EffectProperties
    {
        public IDictionary<string, object> Properties { get; private set; }

        public object this[string propertyName]
        {
            get { return Properties[propertyName]; }
            set { Properties[propertyName] = value; }
        }

        public IColorEffect GetColorEffect(string propertyName)
        {
            return (IColorEffect) this[propertyName];
        }

        public int GetTime(string propertyName)
        {
            return (int) ((double) this[propertyName]*1000);
        }

        public int GetInt(string propertyName)
        {
            return (int) this[propertyName];
        }

        public string GetRepeatMethod()
        {
            if (Properties.ContainsKey(PropertyDefinition.RepeatMethod.Name))
                return (string) this[PropertyDefinition.RepeatMethod.Name];
            return RepeatMethods.Restart;
        }

        public int GetRepeatCount()
        {
            if (Properties.ContainsKey(PropertyDefinition.RepeatCount.Name))
                return (int) this[PropertyDefinition.RepeatCount.Name];
            return 1;
        }

        public int GetDurration()
        {
            return GetTime(PropertyDefinition.Durration.Name);
        }

        public IOrdering GetOrdering(Group group )
        {
            string[] orderings = (string[]) this[PropertyDefinition.Ordering.Name];
            if(orderings.Count() != 2)
                throw new InvalidDataException("Invalid EffectProperties. Ordering is not an array of two strings.");
            IOrdering ordering = Orderings.GetOrdering(orderings[0], orderings[1]);
            ordering.Group = group;
            return ordering;
        }

        public double GetFloat(string propertyName)
        {
            return (double) this[propertyName];
        }

        public IEasing GetEasing(string propertyName)
        {
            return Easings.GetEasingForName((string) this[propertyName]);
        }
    }
}
