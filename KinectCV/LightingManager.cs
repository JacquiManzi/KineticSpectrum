using KineticControl;

namespace RevKitt.ks.KinectCV
{
    public class LightingManager
    {
        private readonly LightSystem _lightSystem = new LightSystem();

        public LightingManager(string hostInterface)
        {
            
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
