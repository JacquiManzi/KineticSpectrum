using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using KineticControl;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment
{
    public class LightSystemProvider
    {
        public static readonly LightSystem LightSystem = new LightSystem();

        public static void Init(String hostInterface)
        {
            Console.WriteLine("Attempting to set network interface to: " + hostInterface);
            if (!LightSystem.NetworkInterfaces.Contains(hostInterface))
            {
                Console.WriteLine(
                    "Host Interface '" + hostInterface + "' does not exist on this computer. Available options are:\n" +
                    String.Join("\n", LightSystem.NetworkInterfaces), "hostInterface");
                Console.WriteLine("Starting without any network interfaces.");
                return;
            }

            LightSystem.SelectedInterface = hostInterface;

            Console.WriteLine("Searching for Color Kinetics Devices...");
            LightSystem.RefreshLightList();
            Console.WriteLine("Done Searching.");
        }

        private static readonly Dictionary<LightAddress, LEDNode> _nodes = new Dictionary<LightAddress, LEDNode>();

        public static IList<LEDNode> Lights { get { return new List<LEDNode>(_nodes.Values); } } 

        public static IDictionary<LightAddress, LEDNode> GetNodeMapping(IEnumerable<LightAddress> addresses )
        {
            IDictionary<LightAddress, LEDNode> mapping = new Dictionary<LightAddress, LEDNode>();
            foreach (var lightAddress in addresses)
            {
                mapping[lightAddress] =  _nodes[lightAddress];
            }
            return mapping;
        }

        private const string LineMarker = "###";
        public static IDictionary<LightAddress, LEDNode> ParseProps(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

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

        private static void ParseLights(StreamReader reader)
        {
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

                _nodes.Add(address, new LEDNode(address, new Vector3D(x,y,z)));
            }
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
    }
}
