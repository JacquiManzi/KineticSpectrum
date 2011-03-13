using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineticControl
{
    interface Fixture
    {
        String SendPacket(String packet, String address);
        List<String> SetAddressList();
        List<String> GetAddressList();

    }
}
