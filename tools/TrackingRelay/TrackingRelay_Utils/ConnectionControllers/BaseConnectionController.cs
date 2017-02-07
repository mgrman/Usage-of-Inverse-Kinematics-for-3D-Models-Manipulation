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

namespace TrackingRelay_Utils
{

    /// <summary>
    /// Base class for handling of messages. Processes incoming commands and sends responses.
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public abstract class BaseConnectionController<CONTYPE> : IServer where CONTYPE : IConnection, new()
    {
        public const int BufferSize = SocketController<CONTYPE>.BufferSize;

        List<CONTYPE> _managers = new List<CONTYPE>();

        Object _messagesToSend_lock = new Object();

        List<string> _messagesToSend = new List<string>();
        List<string> _messagesToSend_shadow = new List<string>();

        public BaseConnectionController()
        {

        }

        protected abstract bool CanRead { get; }
        protected abstract string ReadMessage(byte[] buffer);
        protected abstract void SendMessage(string message);


        public void RunServerLoop()
        {
            ListenerLoop();
        }

        private void ListenerLoop()
        {
            Byte[] bytes = new Byte[BufferSize];
            String incomingMessage = null;

            try
            {
                while (CanRead)
                {
                    incomingMessage = ReadMessage(bytes);

                    Console.WriteLine("Received: {0}", incomingMessage);

                    var response = HandleMessage(incomingMessage);


                    SendMessage(response);
                    Console.WriteLine("Sent: {0}", response);
                }


            }
            catch (IOException exp)
            {

                Console.WriteLine("IOException: {0}", exp.Message);
            }


        }

        private string HandleMessage(string incomingMessage)
        {
            string command;
            Dictionary<string, string> args;
            if (incomingMessage.Contains(' '))
            {
                command = incomingMessage.Substring(0, incomingMessage.IndexOf(' ')).Trim(); ;
                var regex = new Regex("<(.*?)>");

                args = regex
                    .Matches(incomingMessage.Substring(incomingMessage.IndexOf(' ') + 1))
                    .Cast<Match>()
                    .Select(o => o.Groups[1].Value.Split('|'))
                    .ToDictionary(o => o.First().ToLower(), o => o.Last());
            }
            else
            {
                command = incomingMessage.Trim(); ;
                args = new Dictionary<string, string>();
            }
            string response;
            switch (command.ToLower())
            {
                case "startclient":
                    string startErrorMessage;
                    var sucess = StartClient(args, out startErrorMessage);
                    if (sucess)
                    {
                        response = string.Format("CMD OK startclient [{0}]", string.Join(" ", args));
                    }
                    else
                    {
                        response = string.Format("CMD BAD_ARGS startclient [startClient] - ", startErrorMessage);
                    }
                    break;
                case "stopclients":
                    StopClients();
                    response = string.Format("CMD OK stopClient []");
                    break;
                case "getpositions":

                    response = string.Join("|", _managers.Select((o, i) => string.Format("{0}:{1}", i, o.CurrentPosition)));
                    response = string.Format("CMD DATA getpositions [{0}]", response);
                    break;
                case "getrotations":
                    response = string.Join("|", _managers.Select((o, i) => string.Format("{0}:{1}", i, o.CurrentRotation)));
                    response = string.Format("CMD OK getrotations [{0}]", response);
                    break;
                default:
                    response = string.Format("CMD UNKNOWN [{0}]", incomingMessage.ToUpper());
                    break;
            }
            return response;
        }

        private bool StartClient(Dictionary<string, string> args, out string errorMsg)
        {
            errorMsg = "";


            var manager = Activator.CreateInstance<CONTYPE>();

            manager.RunClient(args);

            if (manager.ClientConnected)
            {
                _managers.Add(manager);
            }

            return manager.ClientConnected;
        }

        private void StopClients()
        {
            foreach (var manager in _managers)
            {
                manager.KillClient();
            }
        }

    }
}
