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

        public static readonly PositionFunc InOutFunc = (pos, total) =>
                                                         {
                                                             if (pos <= total/2)
                                                                 return total - pos*2;
                                                             return (pos - total/2 - 1)*2;
                                                         };

        public static readonly PositionFunc OutInFunc = (pos, total) =>
                                                         {
                                                             if (pos <= total/2)
                                                                 return pos*2;
                                                             return (total - pos)*2;
                                                         };

        public static readonly PositionFunc ReverseFunc = (pos, total) => total - pos;

        public readonly static PositionFunc ForwardFunc = (pos, total) => pos;

        public delegate double PositionFunc(double pos, double total);
    }
}
