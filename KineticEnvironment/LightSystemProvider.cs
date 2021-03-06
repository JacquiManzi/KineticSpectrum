﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment
{
    public delegate void LightsUpdatedEventHandler(bool added);

    public class LightSystemProvider
    {
        public static readonly LightSystem LightSystem = new LightSystem();

        public static void Init(String hostInterface)
        {
            LightSystem.AutoUpdate = false;
            Console.WriteLine("Attempting to set network interface to: " + hostInterface);
            if (!LightSystem.NetworkInterfaces.Contains(hostInterface))
            {
                Console.WriteLine(
                    "Host Interface '" + hostInterface + "' does not exist on this computer. Available options are:\n" +
                    String.Join("\n", LightSystem.NetworkInterfaces));
                Console.WriteLine("Starting without any network interfaces.");
                return;
            }

            LightSystem.SelectedInterface = hostInterface;

            Console.WriteLine("Searching for Color Kinetics Devices...");
            LightSystem.RefreshLightList();
            Console.WriteLine("Done Searching.");
        }

        public static event LightsUpdatedEventHandler OnLightsUpdated;

        private static void LightsUpdated(bool added)
        {
            if (OnLightsUpdated != null)
                OnLightsUpdated(added);
        }

        private static readonly Dictionary<LightAddress, LEDNode> _nodes = new Dictionary<LightAddress, LEDNode>();
        private static int _unaddressed = 0;

        public static List<LEDNode> Lights { get { return new List<LEDNode>(_nodes.Values.Select(node=>new LEDNode(node.Address, node.Position))); } } 

        public static void UpdateLights()
        {
            LightSystem.UpdateLights();
        }

        public static IDictionary<LightAddress, LEDNode> GetNodeMapping(IEnumerable<LightAddress> addresses )
        {
            IDictionary<LightAddress, LEDNode> mapping = new Dictionary<LightAddress, LEDNode>();
            foreach (var lightAddress in addresses)
            {
                if(!mapping.ContainsKey(lightAddress))
                    mapping[lightAddress] =  _nodes[lightAddress];
            }
            return mapping;
        }

        public static void RemoveLED(LightAddress address)
        {
            if (_nodes.ContainsKey(address))
            {
                _nodes.Remove(address);
                LightsUpdated(false);
            }
        }

        public static IList<LightAddress> AddLEDs(IEnumerable<LEDNode> nodes)
        {
            IList<LightAddress> addresses = new List<LightAddress>();
            foreach (LEDNode node in nodes)
            {
                LEDNode uNode = node;
                if (node.Address.IsUnknown)
                {
                    uNode = new LEDNode(new LightAddress(-1,-1, _unaddressed++), node.Position );
                }
                _nodes.Add(uNode.Address, uNode);
                addresses.Add(uNode.Address);
            }
            LightsUpdated(true);
            return addresses;
        }

        public static LightAddress AddLED(LEDNode node)
        {
            if (_nodes.ContainsKey(node.Address))
                throw new ArgumentException("Node with address '" + node.Address + "' already exists.");
            if (node.Address.IsUnknown)
            {
                node = new LEDNode(new LightAddress(-1,-1, _unaddressed++), node.Position );
            }

            _nodes.Add(node.Address, node);
            LightsUpdated(true);
            return node.Address;
        }

        private const string LineMarker = "###";
        public static IDictionary<LightAddress, LEDNode> ParseProps(Stream stream, out IDictionary<string, string> nameToSection )
        {
            StreamReader reader = new StreamReader(stream);
            nameToSection = new Dictionary<string, string>();

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if(line != null && line.StartsWith(LineMarker))
                {
                    if(line.Contains("Fixtures"))
                    {
                        ParseFixtures(reader);
                    }
                    else if(line.Contains("Lights"))
                    {
                        ParseLights(reader);
                    }
                    else
                    {
                        string sectionName = line.Substring(LineMarker.Length + 1);
                        nameToSection.Add(sectionName, ParseSection(reader));
                    }
                }
            }
            return new Dictionary<LightAddress, LEDNode>(_nodes);
        }

        private static string ReadSectionLine(StreamReader reader)
        {
            if (reader.EndOfStream) return null;
            if (((char)reader.Peek()).Equals('#')) return null;

            return reader.ReadLine();
        }

        private const string Separater = " ";

        private static string ParseSection(StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();
            string line;
            while (null != (line = ReadSectionLine(reader)))
            {
                builder.Append(line).Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        private static void ParseLights(StreamReader reader)
        {
            int duplicates = 0;
            string line;
            while(null != (line = ReadSectionLine(reader)))
            {
                string[] fixture = line.Split(Separater.ToCharArray());
                double x,y,z;
                LightAddress address;
                if(fixture.Length != 4 || !LightAddress.TryParse(fixture[0], out address)
                                       || !double.TryParse(fixture[1], out x)
                                       || !double.TryParse(fixture[2], out y)
                                       || !double.TryParse(fixture[3], out z))
                    continue;

                if(!_nodes.ContainsKey(address))
                    _nodes[address] = new LEDNode(address, new Vector3D(x,y,z));
                else
                {
                    duplicates++;
                    Console.WriteLine("Found Duplicates: address: " + address + " orig pos: "+ _nodes[address].Position + " dup pos:" + new Vector3D(x,y,z));
                }
            }
            LightsUpdated(true);
            Console.WriteLine("Found " + duplicates + " duplicates.");
        }

        private static void ParseFixtures(StreamReader reader)
        {
            Dictionary<string, int> fixtureMap = new Dictionary<string, int>();
            string line;
            while (null != (line = ReadSectionLine(reader)))
            {
                string[] fixture = line.Split(Separater.ToCharArray());
                int fixtureNo;
                if (fixture.Length != 2 || !int.TryParse(fixture[0], out fixtureNo))
                    continue;

                fixtureMap.Add(fixture[1], fixtureNo);
            }
            LightSystem.SetFixtureAddresses(fixtureMap);
        }

        public static IDictionary<int, String> getFixtures()
        {
            return LightSystem.getFixtures();
        }
    }
}
