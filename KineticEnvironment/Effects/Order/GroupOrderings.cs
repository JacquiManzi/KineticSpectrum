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

        private readonly static PositionFunc ForwardFunc = (pos, total) => pos;

        public static IOrdering Forward
        {
            get { return new GroupOrdering(GroupOrderingTypes.Forward, ForwardFunc); } 
        }

        private static readonly PositionFunc ReverseFunc = (pos, total) => total - pos;
 
        public static IOrdering Reverse
        {
            get{return new GroupOrdering(GroupOrderingTypes.Reverse, ReverseFunc);}
        }

        private static readonly PositionFunc OutInFunc = (pos, total) =>
                                                         {
                                                             if (pos <= total/2)
                                                                 return pos*2;
                                                             return (total - pos)*2;
                                                         };
        public static IOrdering OutIn
        {
            get{ return  new GroupOrdering(GroupOrderingTypes.OutIn, OutInFunc);}
        }

        private static readonly PositionFunc InOutFunc = (pos, total) =>
                                                         {
                                                             if (pos <= total/2)
                                                                 return total - pos*2;
                                                             return (pos - total/2 - 1)*2;
                                                         };

        public static IOrdering InOut
        {
            get{ return new GroupOrdering(GroupOrderingTypes.InOut, InOutFunc);}
        }

        protected delegate int PositionFunc(int pos, int total);

        protected class GroupOrdering : IOrdering
        {
            
            private PositionFunc Position { get;  set; }
            public string Type { get { return OrderingTypes.Group; } }
            public string Ordering { get; private set; }
            public bool Runnable { get { return Group != null; } }

            private Group _group;
            private readonly Dictionary<LightAddress, int> _addressToPos = new Dictionary<LightAddress, int>();
            private int _last;


            internal GroupOrdering(string ordering, PositionFunc positionFunc)
            {
                Ordering = ordering;
                Position = positionFunc;
            }

            public double GetLEDPosition(LEDNode ledNode)
            {
                var address = ledNode.Address;
                int pos = Position(_addressToPos[address], _last);
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
                    _last = addresses.Count - 1;
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
