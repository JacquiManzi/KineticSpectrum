using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment
{
    public interface IStateProvider : IActivatable
    {
        int Time { get; set; }
        bool IsPlaying { get; set; }
        IList<LightState> LightState { get; }
        int EndTime { get; }
        void WriteRange(Stream stream, int start, int end);
    }
}
