using System;
using System.Collections.Generic;
using System.Linq;
using KineticControl;
using Newtonsoft.Json;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public class MasterGroup : IGroup
    {
        private readonly GroupReference _me;
        private readonly List<WeakReference<GroupReference>> _referring = new List<WeakReference<GroupReference>>();
        private readonly object _lock = new object();

        internal MasterGroup(string name, IEnumerable<LightAddress> addresses, IActivatable lightProvider)
        {
            _me = new GroupReference(name, addresses, lightProvider);
        }

        internal void UpdateMembers(IList<LightAddress> addresses)
        {
            _me.UpdateMembers(addresses);
            GetRefs().AsParallel().ForAll(r=>UpdateMembers(addresses));
        }

        public IGroup GetReference(IActivatable lightProvider)
        {
            lock (_lock)
            {
                GroupReference gRef = new GroupReference(Name, Lights, lightProvider);
                _referring.Add(new WeakReference<GroupReference>(gRef));
                return gRef;
            }
        }

        private IEnumerable<GroupReference> GetRefs()
        {
            List<GroupReference> toUpdate = new List<GroupReference>();
            int index = 0;
            lock (_lock)
            {
                while (index < _referring.Count)
                {
                    GroupReference stub;
                    if (!_referring[index].TryGetTarget(out stub))
                    {
                        _referring.RemoveAt(index);
                    }
                    else
                    {
                        toUpdate.Add(stub);
                        index++;
                    }
                }
            }
            return toUpdate;
        }

        public string Name { get { return _me.Name; } }
        public IList<LightAddress> Lights { get { return _me.Lights; } }
        public IList<LEDNode> LEDNodes { get { return _me.LEDNodes; } }
    }
}
