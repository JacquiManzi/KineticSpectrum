using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticControl
{
    public class LightType
    {
        public static readonly LightType None = new LightType() {Name = "None", Spacing = 0, NoLights = 0};
        public static readonly LightType Short = new LightType() {Name = "iColor Flex 12", Spacing = 12, NoLights = 50};
        public static readonly LightType Long = new LightType() {Name = "iColor Flex 4", Spacing = 4, NoLights = 50};

        public static List<LightType> Types = new List<LightType>()
        {
            None,
            Short,
            Long
        };

        public String Name { get; internal set; }
        public int Spacing { get; internal set; }
        public int NoLights { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            LightType other = obj as LightType;
            if ((object)other == null) return false;

            return Name.Equals(other.Name) &&
                    Spacing == other.Spacing &&
                    NoLights == other.NoLights;
           
        }

        public static bool operator ==(LightType l1, LightType l2)
        {
            if (ReferenceEquals(l1, l2)) return true;
            if (((object)l1 == null) || ((object)l2 == null)) return false;

            return l1.Name == l2.Name && l1.NoLights == l2.NoLights && l1.Spacing == l2.Spacing;
        }

        public static bool operator !=(LightType l1, LightType l2)
        {
            return !(l1 == l2);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() << 10 +
                   Spacing << 8 + 
                   NoLights;
        }

        public override string ToString()
        {
            return Name;
        }
    }


}
