using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment
{
    public class TimeRange
    {
        public readonly double Portion;
        public readonly int Current;
        public readonly int End;

        public TimeRange(int current, int end)
        {
            Current = current;
            End = end;
            Portion = 1.0*current/end;
        }
    }
}
