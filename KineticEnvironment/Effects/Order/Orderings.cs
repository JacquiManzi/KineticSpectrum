using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    public class Orderings
    {
        public static IOrdering GetOrdering(string orderingType, string ordering)
        {
            if (OrderingTypes.Group.Equals(orderingType))
                return GroupOrderings.GetOrdering(ordering);
            if (OrderingTypes.Spatial.Equals(orderingType))
                return SpatialOrderings.GetOrdering(ordering);
            throw new ArgumentException("Invalid OrderingType '"+orderingType+"'.", "orderingType");
        }
    }
}
