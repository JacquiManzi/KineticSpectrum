using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevKitt.KS.KineticEnvironment.Effects;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public delegate void ChangedEventHandler(Pattern updated);

    public class Pattern
    {
        private List<string> _groups = new List<string>(); 

        public string Name { get; set; }

        public IList<string> Groups
        {
            get { return new List<string>(_groups).AsReadOnly(); }
            set
            {
                _groups = new List<string>(value);
                Changed();
            }
        }

        public string EffectName { get; set; }

        public EffectProperties EffectProperties { get; set; }

        public int Priority { get; set; }

        public void Set(Pattern pattern)
        {
            _groups = new List<string>(pattern.Groups);
            Name = pattern.Name;
            EffectName = pattern.EffectName;
            EffectProperties = pattern.EffectProperties;
            Priority = pattern.Priority;
            Changed();
        }

        public event ChangedEventHandler OnChanged;

        private void Changed()
        {
            if (OnChanged != null)
                OnChanged(this);
        }
    }
}
