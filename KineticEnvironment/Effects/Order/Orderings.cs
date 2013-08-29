using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    public class Orderings
    {
        public const string OrderingTypeKey = "type";
        public const string OrderingKey = "ordering";

        public static IOrdering GetOrdering(string orderingType, string ordering)
        {
            if (OrderingTypes.Group.Equals(orderingType))
                return GroupOrderings.GetOrdering(ordering);
            if (OrderingTypes.Spatial.Equals(orderingType))
                return SpatialOrderings.GetOrdering(ordering);
            throw new ArgumentException("Invalid OrderingType '"+orderingType+"'.", "orderingType");
        }

        public static readonly PositionFunc InOutFunc = (pos, min, total) =>
                                                            {
                                                                pos = (pos - min) / (total - min);
                                                                if (pos <= .5)
                                                                    pos = pos * 2;
                                                                else
                                                                    pos = (1 - pos) * 2;
                                                                return pos * (total - min) + min;
                                                            };

        public static readonly PositionFunc OutInFunc = (pos, min, total) =>
                                                            {
                                                                pos = InOutFunc(pos, min, total);
                                                                return ReverseFunc(pos, min, total);
                                                            };

        public static readonly PositionFunc ReverseFunc = (pos, min, total) => (total-min) - (pos - min) + min;

        public readonly static PositionFunc ForwardFunc = (pos, min, total) => pos;

        public delegate double PositionFunc(double pos, double min, double max);
    }
}
