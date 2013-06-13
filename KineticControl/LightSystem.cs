using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace KineticControl
{
    public class LightSystem
    {
        private readonly Network _network;
        private readonly List<ColorData> _fixtureList;
        private IList<PDS> _pdss = new List<PDS>();

        private readonly Timer _updateTimer;

        public LightSystem() : this(Network.GetInstance()) {}

        public LightSystem(Network network)
        {
            _network = network;
            _fixtureList = new List<ColorData>();
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
            return _fixtureList[lightAddress.FixtureNo][lightAddress.LightNo];
        }

        public void SetColor(LightAddress lightAddress, Color color)
        {
            _fixtureList[lightAddress.FixtureNo][lightAddress.LightNo] = color;
        }

        public Color this[LightAddress lightAddress]
        {
            get { return _fixtureList[lightAddress.FixtureNo][lightAddress.LightNo]; }
            set { _fixtureList[lightAddress.FixtureNo][lightAddress.LightNo] = value; }
        }

        private IList<LightAddress> FindNetworks()
        {
            _network.BroadCast();
            _pdss = _network.PDSs;
            foreach (var pds in _pdss)
            {
                foreach (var colarData in pds.AllColorData )
                {
                    if(!_fixtureList.Contains(colarData))
                    {
                        _fixtureList.Add(colarData);
                    }
                }
            }
            _updateTimer.Enabled = true;
            return LightAddresses;
        }

        public List<LightAddress> LightAddresses
        {
            get
            {
                List<LightAddress> addresses = new List<LightAddress>();
                for (int j = 0; j < _fixtureList.Count; j++ )
                {
                    var colorData = _fixtureList[j];
                    for (int i = 0; i < colorData.Count; i++)
                    {
                        addresses.Add(new LightAddress(j,i));
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
