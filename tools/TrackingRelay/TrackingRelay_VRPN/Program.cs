using TrackingRelay_Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_VRPN
{
    class Program
    {
        static void Main(string[] args)
        {
            var relay = new RelayController<VrpnManager>(args);
            relay.RunServerLoop();
        }
    }
}
