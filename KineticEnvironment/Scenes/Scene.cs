using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public delegate void PatternDeletedHandler(Pattern pattern);

    public class Scene : IStateProvider
    { 
        private const String TEMP_GROUP = "____TEMP____GROUP";

        private readonly Dictionary<string, MasterGroup> _nameToGroup = new Dictionary<string, MasterGroup>();
        private readonly Dictionary<string, Pattern> _patterns = new Dictionary<string, Pattern>();
        private volatile List<IGroup> _selectedGroups = new List<IGroup>();
        private readonly Dictionary<LightAddress, LEDNode> _addressToNode;
        private readonly object _lock = new object();

        public Scene()
        {
            _addressToNode = new Dictionary<LightAddress, LEDNode>(
                    LightSystemProvider.Lights.ToDictionary(node=>node.Address)
                );
            LightSystemProvider.OnLightsUpdated += LightsUpdated;
            IsActive = false;
        }

        private void LightsUpdated(bool added)
        {
            if (added)
            {
                foreach (LEDNode node in LightSystemProvider.Lights)
                {
                    if (!_addressToNode.ContainsKey(node.Address))
                    {
                        node.IsActive = IsActive;
                        _addressToNode[node.Address] = node;
                    }
                }
            }
            else
            {
                ISet<LightAddress> addresses = new HashSet<LightAddress>(LightSystemProvider.Lights.Select(l=>l.Address));
                IList<LightAddress> toRemove = _addressToNode.Keys.Where(address => !addresses.Contains(address)).ToList();
                foreach (var remove in toRemove)
                {
                    _addressToNode.Remove(remove);
                }
            }
        }
      
        public IGroup SetGroup(String name, List<LightAddress> addresses)
        {
            return SetGroup(new GroupStub(name, addresses));
        }

        public IGroup SetGroup(IGroup group)
        {
            lock (_lock)
            {
                if (_nameToGroup.ContainsKey(group.Name))
                {
                    _nameToGroup[group.Name].UpdateMembers(group.Lights);
                }
                else
                {
                    _nameToGroup[group.Name] = new MasterGroup(group.Name, group.Lights, this);
                }
            }

            var groupSingle = new List<string> {group.Name};
            foreach (var pattern in _patterns.Values)
            {
                if (pattern.Groups.Contains(group.Name))
                {
                    pattern.Groups = new List<string>(pattern.Groups.Except(groupSingle));
                }
            }
            SelectGroups(new List<string>{group.Name});
            return group;
        }

        public void DeleteGroup(string groupName)
        {
            _nameToGroup.Remove(groupName);
            var remGroup = new List<string> {groupName};
            foreach (var pattern in _patterns.Values)
            {
                if(pattern.Groups.Contains(groupName))
                    pattern.Groups = new List<string>(pattern.Groups.Except(remGroup));
            }
        }

        public void RenameGroup(string oldName, IGroup newGroup)
        {
            throw new NotImplementedException("This functionality does not yet exist: Renaming Groups");
        }

        public void SetPattern(Pattern pattern)
        {
            Pattern existing = null;

            if (_patterns.ContainsKey(pattern.Name))
            {

                existing = _patterns[pattern.Name];
            }


            if (existing == null)
                _patterns[pattern.Name] = pattern;
            else
                existing.Set(pattern);
        }

        public void DeletePattern(string patternName)
        {
//            var pattern = _patterns[patternName];
            _patterns.Remove(patternName);
        }

        public void RenamePattern(String oldName, Pattern pattern)
        {
            var existing = _patterns[oldName];
            if (existing == null)
                _patterns[pattern.Name] = pattern;
            else
            {
                _patterns[pattern.Name] = existing;
                _patterns.Remove(oldName);
                existing.Set(pattern);
            }
        }

        public void SelectLights(IEnumerable<LightAddress> lights )
        {
            SelectedGroups = new List<IGroup>{new MasterGroup(TEMP_GROUP, lights, this)};
            ApplySelected();
        }

        public void SelectGroups(IEnumerable<string> groups )
        {
            var selected = new List<IGroup>();
            foreach (var group in groups)
            {
                selected.Add(_nameToGroup[group]);
            }
            SelectedGroups = selected;
            ApplySelected();
        }

        public void ApplySelected()
        {
            foreach (LEDNode node in _addressToNode.Values)
            {
                node.Color = Colors.Black;
            }
            foreach (var group in _selectedGroups)
            {
                foreach (var led in group.LEDNodes)
                {
                    led.Color = Colors.Red;
                }
            }
            if (IsActive)
            {
                LightSystemProvider.LightSystem.UpdateLights();
            }
        }

        public IEnumerable<IGroup> SelectedGroups
        {
            get { return _selectedGroups; }
            private set
            {
                _selectedGroups = new List<IGroup>(value);
                ApplySelected();
            }
        }

        public IList<IGroup> GetGroups(IEnumerable<string> groupNames )
        {
            return new List<IGroup>(Groups.FindAll(g=>groupNames.Contains(g.Name)));
        }

        internal IList<IGroup> GetGroupRefs(IEnumerable<string> groupNames, IActivatable lightProvider)
        {
                return new List<IGroup>( 
                    _nameToGroup.Values.ToList()
                                .FindAll(g => groupNames.Contains(g.Name))
                                .Select(master => master.GetReference(lightProvider)));
        }

        public List<IGroup> Groups { get { return new List<IGroup>(_nameToGroup.Values); } }
        public IEnumerable<Pattern> Patterns { get { return _patterns.Values; } } 

        public IGroup GetGroup(string groupName)
        {
            return _nameToGroup[groupName];
        }

        public Pattern GetPattern(string patternName)
        {
            return _patterns[patternName];
        }

        public int EndTime { 
            get { return 0; }
            set { }
        }

        public int Time 
        { 
            get { return 0; }
            set { }
        }
        public bool IsPlaying {
            get { return false; }
            set { }
        }

        public IEnumerable<LightState> LightState
        {
            get { return Enumerable.Empty<LightState>(); }
        }

        public IEnumerable<LEDNode> Nodes {
            get { return _addressToNode.Values; }
        }

        public bool IsActive { get; set; }
    }
}
