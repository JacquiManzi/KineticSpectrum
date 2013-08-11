using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace KineticControl
{
    public class LightSystem
    {
        private readonly Network _network;
        private IList<PDS> _pdss = new List<PDS>();

        private readonly Timer _updateTimer;

        public LightSystem() : this(Network.GetInstance()) {}

        public LightSystem(Network network)
        {
            _network = network;
            _updateTimer = new Timer();
            _updateTimer.Elapsed += (o, args) => UpdateLights();
            _updateTimer.Interval = 1000/30;
            GC.KeepAlive(_updateTimer);
        }

        public void UpdateLights()
        {
            foreach (var pds in _pdss)
            {
                pds.UpdateSystem();
            }
        }
        public List<String> NetworkInterfaces
        {
            get { return new List<string>(_network.RetrieveNetworkCards().Select(card => card.Name));}
        }

        public Task<IList<LightAddress>> RefreshLightList()
        {
            return Task.Factory.StartNew(() => FindNetworks());
        }

        public Color GetColor(LightAddress lightAddress)
        {
            return _pdss[lightAddress.FixtureNo][lightAddress.PortNo][lightAddress.LightNo];
        }

        public void SetColor(LightAddress lightAddress, Color color)
        {
            _pdss[lightAddress.FixtureNo][lightAddress.PortNo][lightAddress.LightNo] = color;
        }

        public Color this[LightAddress lightAddress]
        {
            get { return _pdss[lightAddress.FixtureNo][lightAddress.PortNo][lightAddress.LightNo]; }
            set { _pdss[lightAddress.FixtureNo][lightAddress.PortNo][lightAddress.LightNo] = value; }
        }

        private IList<LightAddress> FindNetworks()
        {
            _network.BroadCast();
            IList<PDS> pdss = _network.PDSs;
            
            foreach (var pds in pdss)
            {
                if (!_pdss.Contains(pds, new PDSAddressComparitor()))
                    _pdss.Add(pds);
            }
            _updateTimer.Enabled = true;
            return LightAddresses;
        }

        public IDictionary<int, String> getFixtures()
        {
            Dictionary<int, String> addressToFixture = new Dictionary<int, string>();

            for (int i = 0; i < _pdss.Count; i++)
            {
                addressToFixture.Add(i, _pdss[i].EndPoint.Address.ToString());
            }
            return addressToFixture;
        }

        public bool AutoUpdate
        {
            get { return _updateTimer.Enabled; }
            set { _updateTimer.Enabled = value; }
        }

        public String GetNetworkAddress(LightAddress address)
        {
            return _pdss[address.FixtureNo].EndPoint.Address.ToString();
        }

        /// <summary>
        /// Using the map from ip address to fixture number, assigns the specified relationship to
        /// this LightSystem. This align's this system configuration with a previous configuration,
        /// so that previous configuration's addressing can be used.
        /// </summary>
        /// <param name="ipToFixtureNo"></param>
        /// <returns>A list of IP addresses that aren't detected on the current network</returns>
        public IEnumerable<String> SetFixtureAddresses(IDictionary<String, int> ipToFixtureNo )
        {
            int max = ipToFixtureNo.Values.Max() + 1 + _pdss.Count;
            ipToFixtureNo = new Dictionary<string, int>(ipToFixtureNo);

            PDS[] newPDSs = new PDS[max];
            List<int> missing = new List<int>(Enumerable.Range(0,max).Except(ipToFixtureNo.Values));
            
            foreach (var pds in _pdss)
            {
                String ipAddress = pds.EndPoint.Address.ToString();
                if (ipToFixtureNo.ContainsKey(ipAddress))
                {
                    newPDSs[ipToFixtureNo[ipAddress]] = pds;
                    ipToFixtureNo.Remove(ipAddress);
                }
                else
                {
                    newPDSs[missing[0]] = pds;
                    missing.RemoveAt(0);
                }
            }
            foreach (var i in ipToFixtureNo)
            {
                newPDSs[i.Value] = new NoOpPDS(Enumerable.Repeat(50,8).ToArray(), IPAddress.Parse(i.Key));
            }
            //This assumes that there are no missing values in the input list (or at least that for each missing value
            //there is an item now that didn't exist before. Likely will be a source of problems in the future.
            _pdss = new List<PDS>(newPDSs.TakeWhile(p => p != null));

            return ipToFixtureNo.Keys;
        }


        public List<LightAddress> LightAddresses
        {
            get
            {
                List<LightAddress> addresses = new List<LightAddress>();
                for (int j = 0; j < _pdss.Count; j++ )
                {
                    var colorDataList = _pdss[j].AllColorData;
                    for (int k = 0; k < colorDataList.Count; k++)
                    {
                        var colorData = colorDataList[k];
                        for (int i = 0; i < colorData.Count; i++)
                        {
                            addresses.Add(new LightAddress(j, k, i));
                        }
                    }
                }
                return addresses;
            }
        }

        public String SelectedInterface
        {
            get 
            { 
                NetworkInterface card = _network.NetworkCard;
                return card == null ? null : card.Name;
            }

            set
            {
                _network.SetInterface(value);
            }
        }

        
    }
}
