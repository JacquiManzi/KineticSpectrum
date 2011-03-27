using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KineticControl
{
    public class Fixture
    {
        public int Address { get; private set; }
        public Color Color { get; set; }

        public Fixture(int address)
        {
            Address = address;
            Color = Colors.Black;
        }
    }
}
