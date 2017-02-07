using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Timers;
using System.Text.RegularExpressions;
using System.Threading;

namespace TrackingRelay_Utils
{
    /// <summary>
    /// Controler which listens to Socket connections. Messages are handled by RawStreamController.
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public class SocketController<CONTYPE> : IServer where CONTYPE : IConnection, new()
    {
        public const int BufferSize = 100;

        IServer _controler;

        Socket _listener;

        public SocketController(int portNumber)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(new IPEndPoint(IPAddress.Loopback, portNumber));
            _listener.Listen(10);
        }

        public void RunServerLoop()
        {
            ListenerLoop();
        }

        private void ListenerLoop()
        {
            try
            {
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    Socket client = _listener.Accept();
                    client.NoDelay = true;

                    client.ReceiveBufferSize = BufferSize;
                    client.SendBufferSize = BufferSize;

                    Console.WriteLine("Connected!");


                    NetworkStream stream = new NetworkStream(client);

                    _controler = Activator.CreateInstance(typeof(RawStreamController<CONTYPE>), stream, stream) as RawStreamController<CONTYPE>;

                    _controler.RunServerLoop();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                _listener.Close();
            }
        }
    }
}
