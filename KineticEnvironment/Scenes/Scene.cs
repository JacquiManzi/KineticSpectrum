using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using KineticControl;

namespace RevKitt.KS.KineticEnvironment.Scenes
{
    public delegate void PatternDeletedHandler(Pattern pattern);

    public class Scene : IStateProvider
    { 
        private Dictionary<string, Group> _nameToGroup = new Dictionary<string, Group>();
        private Dictionary<string, Pattern> _patterns = new Dictionary<string, Pattern>();
        private List<Group> _selectedGroups = new List<Group>();

        public event PatternDeletedHandler OnPatternDeleted;

       
        public Group SetGroup(String name, List<LightAddress> addresses)
        {
            var addressToNode = LightSystemProvider.GetNodeMapping(addresses);
            return SetGroup(new Group(name, addressToNode.Values));
        }

        public Group SetGroup(Group group)
        {
            _nameToGroup[group.Name] = group;

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

        public void RenameGroup(string oldName, Group newGroup)
        {
            if (!_nameToGroup.Remove(oldName))
                throw new ArgumentException("No group exists with name: " + oldName);

            _nameToGroup[newGroup.Name] = newGroup;
            var remGroup = new List<string> { oldName };
            foreach (var pattern in _patterns.Values)
            {
                if(pattern.Groups.Contains(oldName))
                {
                    List<string> groups = new List<string>(pattern.Groups.Except(remGroup));
                    groups.Add(newGroup.Name);
                    pattern.Groups = groups;
                }
                else if(pattern.Groups.Contains(newGroup.Name))
                {
                    pattern.Groups = pattern.Groups;
                }
            }
        }

        public void SetPattern(Pattern pattern)
        {
            var existing = _patterns[pattern.Name];
            if (existing == null)
                _patterns[pattern.Name] = pattern;
            else
                existing.Set(pattern);
        }

        public void DeletePattern(string patternName)
        {
            var pattern = _patterns[patternName];
            if(pattern != null)
            {
                OnPatternDeleted(pattern);
            }
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
            var LEDs = LightSystemProvider.GetNodeMapping(lights).Values;

            SelectedGroups = new List<Group>{new Group("TempGroup", LEDs)};
        }

        public void SelectGroups(IEnumerable<string> groups )
        {
            var selected = new List<Group>();
            foreach (var group in groups)
            {
                selected.Add(_nameToGroup[group]);
            }
            SelectedGroups = selected;
        }

        public void ApplySelected()
        {
            foreach (LEDNode node in LightSystemProvider.Lights)
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
            LightSystemProvider.LightSystem.UpdateLights();
        }

        public IEnumerable<Group> SelectedGroups
        {
            get { return _selectedGroups; }
            set
            {
                _selectedGroups = new List<Group>(value);
                ApplySelected();
            }
        }

        public IList<Group> GetGroups(IEnumerable<string> groupNames )
        {
            return new List<Group>(Groups.FindAll(g=>groupNames.Contains(g.Name)));
        }

        public List<Group> Groups { get { return new List<Group>(_nameToGroup.Values); } }
        public IEnumerable<Pattern> Patterns { get { return _patterns.Values; } } 

        public Group GetGroup(string groupName)
        {
            return _nameToGroup[groupName];
        }

        public Pattern GetPattern(string patternName)
        {
            return _patterns[patternName];
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

        public int EndTime { get; private set; }
    }
}
