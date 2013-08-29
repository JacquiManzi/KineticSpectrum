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
        public readonly int Cycle;

        public readonly int Total;
        public readonly double PortionTotal;

        public TimeRange(int current, int end, int cycle, int total)
        {
            Current = current;
            End = end;
            Portion = 1.0*current/end;
            Cycle = cycle;
            Total = total;
            PortionTotal = (1.0 * cycle * end + current) / total;
        }
    }
}
