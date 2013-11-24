using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using KineticControl;
using RevKitt.KS.KineticEnvironment.D3;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    class SpatialOrderings
    {
        public static IOrdering GetOrdering(string orderingName)
        {
            switch (orderingName)
            {
                    
                case SpatialOrderingTypes.OneDirection:
                    return new LinearSpatialOrdering(SpatialOrderingTypes.OneDirection, Orderings.ForwardFunc);
                case SpatialOrderingTypes.TwoDirectionsIn:
                    return new LinearSpatialOrdering(SpatialOrderingTypes.TwoDirectionsIn, Orderings.OutInFunc);
                case SpatialOrderingTypes.TwoDirectionsOut:
                    return new LinearSpatialOrdering(SpatialOrderingTypes.TwoDirectionsOut, Orderings.InOutFunc);
                case SpatialOrderingTypes.Shrink:
                    return new PolarSpatialOrdering(SpatialOrderingTypes.Shrink, Orderings.ReverseFunc, false);
                case SpatialOrderingTypes.Expand: 
                    return new PolarSpatialOrdering(SpatialOrderingTypes.Expand, Orderings.ForwardFunc, false);
                case SpatialOrderingTypes.ShrinkRand:
                    return new PolarSpatialOrdering(SpatialOrderingTypes.ShrinkRand, Orderings.ReverseFunc, true);
                case SpatialOrderingTypes.ExpandRand:
                    return new PolarSpatialOrdering(SpatialOrderingTypes.ExpandRand, Orderings.ForwardFunc, true);
                default:
                    throw new ArgumentException("Type '" + orderingName + "' is not a valid SpatialOrderingType");
            }
        }

    }

    class PolarSpatialOrdering : SpatialOrdering
    {
        private readonly bool _rand;
        
        public PolarSpatialOrdering(string ordering, Orderings.PositionFunc positionFunc, bool rand) : base(ordering, positionFunc)
        {
            _rand = rand;
        }

        protected override IList<double> VectorsToPositions(IList<Vector3D> vectors )
        {
            if (_rand)
            {
                Vector3D center = vectors[(int)(Rand.NextDouble() * vectors.Count)];
                return vectors.Select(v => D3Util.Distance(center, v)).ToList();
            }
            Plane plane = D3Util.FindPlane(vectors);
            return vectors.Select(plane.Projection)
                .Select(v => D3Util.Distance(plane.Center, v)).ToList();
        }
    }

    class LinearSpatialOrdering : SpatialOrdering
    {
        public LinearSpatialOrdering(string ordering, Orderings.PositionFunc positionFunc) : base(ordering, positionFunc)
        {
        }

        protected override IList<double> VectorsToPositions(IList<Vector3D> vectors )
        {
            Plane plane = D3Util.FindPlane(vectors);

            if (D3Util.Is3D(vectors, plane))
                plane = get3DPlane(vectors, plane.Center);
            else
                plane = get2DPlane(vectors, plane);

            return vectors.Select(v => plane.Distance(v)).ToList();
        }

        private Plane get3DPlane(IEnumerable<Vector3D> vectors, Vector3D center)
        {
            Vector3D planeAngle = new Vector3D(Rand.NextDouble(), Rand.NextDouble(), Rand.NextDouble());
            planeAngle.Normalize();
            Plane newPlane = new Plane(planeAngle, center);

            Vector3D firstPoint = vectors.Aggregate(center,
                                                    (v1, v2) => newPlane.Distance(v1) < newPlane.Distance(v2) ? v1 : v2);

            return newPlane.WithPoint(firstPoint);
        }

        private Plane get2DPlane(IEnumerable<Vector3D> vectors, Plane plane)
        {
            Vector3D center = plane.Center;
            Vector3D start = vectors.Select(plane.Projection).
                Aggregate(center, (v1,v2) =>
                    D3Util.Distance(center,v1) > D3Util.Distance(center,v2)?v1:v2);

            Vector3D normal = start - center;
            return new Plane(normal, start);
        }
    }

    abstract class SpatialOrdering : IOrdering
    {
        protected static readonly Random Rand = new Random();
        private IGroup _group;
        private IDictionary<LightAddress, double> _addressToPos;
        private Orderings.PositionFunc Position { get;  set; }
        private double _max;
        private double _min;

        public IGroup Group {
            get { return _group; }
            set { 
                _group = value;
                RemapAddresses(value);
            }
        }

        public string Type { get { return OrderingTypes.Spatial; }}

        public string Ordering { get; private set; }

        public bool Runnable { get { return Group != null; } }

        protected SpatialOrdering(string ordering, Orderings.PositionFunc positionFunc)
        {
            Ordering = ordering;
            Position = positionFunc;
        }

        private void RemapAddresses(IGroup group)
        {
            IList<LEDNode> nodes = group.LEDNodes;
            IList<Vector3D> vectors = nodes.Select(node => node.Position).ToList();

            IList<double> pos = VectorsToPositions(vectors);

            _addressToPos = new Dictionary<LightAddress, double>();
            double max = Double.MinValue;
            double min = Double.MaxValue;
            for (int i = 0; i < pos.Count; i++)
            {
                _addressToPos[nodes[i].Address] = pos[i];
                max = Math.Max(max, pos[i]);
                min = Math.Min(min, pos[i]);
            }
            _max = max; _min = min;
        }

        protected abstract IList<double> VectorsToPositions(IList<Vector3D> vectors );


        public double GetLEDPosition(LEDNode ledNode)
        {
            var address = ledNode.Address;
            double pos = Position(_addressToPos[address], _min, _max);
            return pos;
        }

        public double GetAngle(LEDNode node)
        {
            return 0;
        }

        public double GetMax()
        {
            return _max;
        }


        public double GetMin()
        {
            return _min;
        }


    }

//        public const string OneDirection = "One Direction";
//        public const string TwoDirectionsIn = "Two Directions (In)";
//        public const string TwoDirectionsOut = "TwoDirections (Out)";
//        public const string Shrink = "Shrink";
//        public const string Expand = "Expand";
}
