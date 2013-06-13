using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineticControl;

namespace RevKitt.LightBuilder
{
    class LightingManager
    {
        private readonly LightSystem _lightSystem = new LightSystem();

        public LightingManager()
        {
            string hostInterface = Properties.Resources.HostInterface;
            if (LightSystem.NetworkInterfaces.Contains(hostInterface))
                LightSystem.SelectedInterface = hostInterface;
        }

        public bool NeedsPrompt
        {
            get { return LightSystem.SelectedInterface == null; }
        }

        public LightSystem LightSystem
        {
            get { return _lightSystem; }
        }
        
    }
}
