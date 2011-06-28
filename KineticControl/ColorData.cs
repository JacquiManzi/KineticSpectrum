using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Linq;

namespace KineticControl
{
    public class ColorData : IEnumerable<Color>
    {
        private readonly byte[] _byteArray;
        private readonly IList<Led> _leds;
        private readonly int _initialLength;
        private readonly int _initial;
        public LightType LightType { get; set; }

        public ColorData(byte[] initialData, LightType lightType, int initial = 0)
        {
            LightType = lightType;
            _initialLength = initialData.Length;
            _byteArray = new byte[_initialLength + HexStrings.addressOff.Length/2];
            _byteArray.Initialize();
            initialData.CopyTo(_byteArray, 0);
            _initial = initial;

            _leds = new List<Led>(lightType.NoLights);
            for (int i = 0; i < lightType.NoLights; i++ )
            {
                _leds.Add(new Led(_byteArray, _initialLength, i));
            }
        }

       // public int Count { get { return (_byteArray.Length - _initialLength)/3; } }
        public int Count { get { return LightType.NoLights; } }

        public byte[] Bytes { get { return _byteArray; } }

        public int Initial { get { return _initial; } }

        public IList<Led> Leds { get { return _leds; } }

        public Color this[int pos]
        {
            get { return _leds[pos].Color; }
            set { _leds[pos].Color = value; }
        }

        public IEnumerator<Color> GetEnumerator()
        {
           int length = Count;
           for(int i=0; i<length; i++)
           {
               yield return this[i];
           }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public XElement ToXml()
        {
            XElement cData = new XElement("ColorData");
            byte[] initialData = new byte[_initialLength];
            for(int i=0;i<_initialLength;i++)
            {
                initialData[i] = _byteArray[i];
            }
            cData.Add(new XElement("InitialData"), initialData );
            
            XElement lightType = new XElement("LightType");
            lightType.Add(new XElement("Name", LightType.Name));
            lightType.Add(new XElement("Spacing", LightType.Spacing));
            lightType.Add(new XElement("NoLights", LightType.NoLights));

            cData.Add(lightType);
            cData.Add(new XElement("Initial", _initial));

            XElement leds = new XElement("LEDs");
            foreach(Led led in _leds)
            {
                XElement ledPos = new XElement("LedPosition");
                if (led.LedPosition != null)
                {
                    ledPos.Add(new XElement("External", led.LedPosition.External));
                    ledPos.Add(new XElement("X", led.LedPosition.X));
                    ledPos.Add(new XElement("Y", led.LedPosition.Y));
                    ledPos.Add(new XElement("Z", led.LedPosition.Z));
                }
                else
                {
                    ledPos.Add(null);
                }
                leds.Add(ledPos);
            }

            cData.Add(leds);
            return cData;
        }
    }
}
