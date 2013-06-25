using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects
{
    public class EffectPropertyTypes
    {
        public const string ColorEffect = "ColorEffect";
        public const string Int = "Int";
        public const string Float = "Float";
        public const string Ordering = "Ordering";
        public const string Easing = "Easing";
        public const string Time = "Time";
        public const string RepeatMethod = "RepeatMethod";
    }

    public class RepeatMethods
    {
        public const string Restart = "Restart";
        public const string Reverse = "Reverse";

        public static readonly IList<string> All = new List<string>{Restart, Reverse}.AsReadOnly();  
    }

    public class OrderingTypes
    {
        public const string Group = "Group";
        public const string Spatial = "Spatial";

        public static IList<string> GetOrderingSubTypes(string orderingType)
        {
            if (Group.Equals(orderingType))
                return GroupOrderingTypes.All;
            
            return SpatialOrderingTypes.All;
        }

        public static readonly IList<string> All = new List<string>{Group, Spatial}.AsReadOnly();
    }

    public class GroupOrderingTypes
    {
        public const string Forward = "Forward";
        public const string Reverse = "Reverse";
        public const string OutIn = "Out-In";
        public const string InOut = "In-Out";

        public static readonly IList<string> All = new List<string> { Forward, Reverse, OutIn, InOut }.AsReadOnly();
    }

    public class SpatialOrderingTypes
    {
        public const string OneDirection = "One Direction";
        public const string TwoDirectionsIn = "Two Directions (In)";
        public const string TwoDirectionsOut = "TwoDirections (Out)";
        public const string Shrink = "Shrink";
        public const string Expand = "Expand";

        public static readonly IList<string> All = new List<string> { OneDirection, TwoDirectionsIn, TwoDirectionsOut, Shrink, Expand }.AsReadOnly();
    }
}
