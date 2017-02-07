using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_Utils
{
    public interface IConnection
    {

        void RunServer(Dictionary<string,string> connectionData);
        void KillServer();

        bool RunClient(Dictionary<string, string> connectionData);
        void KillClient();

        Vector3 CurrentPosition { get; } 
        Vector3 CurrentRotation { get;}

        bool ServerConnected { get; }
        bool ClientConnected { get ; }
    }
}
