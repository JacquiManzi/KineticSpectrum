using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.Order
{
    public class GroupOrderings
    {
        public static IOrdering GetOrdering(string groupOrderingType)
        {
            switch (groupOrderingType)
            {
                case GroupOrderingTypes.Forward:
                    return Forward;
                case GroupOrderingTypes.Reverse:
                    return Reverse;
                case GroupOrderingTypes.InOut:
                    return InOut;
                case GroupOrderingTypes.OutIn:
                    return OutIn;
                default:
                    throw new ArgumentException("Type '" + groupOrderingType + "' is not a valid GroupOrderingType");
            } 
        }


        public static IOrdering Forward
        {
            get { return new GroupOrdering(GroupOrderingTypes.Forward, Orderings.ForwardFunc); } 
        }

 
        public static IOrdering Reverse
        {
            get{return new GroupOrdering(GroupOrderingTypes.Reverse, Orderings.ReverseFunc);}
        }

        public static IOrdering OutIn
        {
            get{ return  new GroupOrdering(GroupOrderingTypes.OutIn, Orderings.OutInFunc);}
        }


        public static IOrdering InOut
        {
            get{ return new GroupOrdering(GroupOrderingTypes.InOut, Orderings.InOutFunc);}
        }


        protected class GroupOrdering : IOrdering
        {
            
            private Orderings.PositionFunc Position { get;  set; }
            public string Type { get { return OrderingTypes.Group; } }
            public string Ordering { get; private set; }
            public bool Runnable { get { return Group != null; } }

            private Group _group;
            private readonly Dictionary<LightAddress, int> _addressToPos = new Dictionary<LightAddress, int>();
            private int _last;


            internal GroupOrdering(string ordering, Orderings.PositionFunc positionFunc)
            {
                Ordering = ordering;
                Position = positionFunc;
            }

            public double GetLEDPosition(LEDNode ledNode)
            {
                var address = ledNode.Address;
                double pos = Position(_addressToPos[address], _last);
                return pos;
            }

            public double GetMax()
            {
                return _last;
            }

            public double GetMin()
            {
                return 0;
            }

            public Group Group
            {
                get { return _group; }
                set
                {
                    if(value == null)
                        throw new ArgumentNullException("value");
                    _group = value;
                    _addressToPos.Clear();
                    var addresses = _group.Lights;
                    _last = addresses.Count;
                    for (int i = 0; i < addresses.Count; i++)
                    {
                        if (!_addressToPos.ContainsKey(addresses[i]))
                        {
                            _addressToPos[addresses[i]] = i;
                        }
                    }
                    
                }
            } 
        }
    }
}
