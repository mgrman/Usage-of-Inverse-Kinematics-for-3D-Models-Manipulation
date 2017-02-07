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
    /// Controler which handles send and receive of messages using Socket
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public class SocketMessageController<CONTYPE> : BaseConnectionController<CONTYPE> where CONTYPE : IConnection, new()
    {

        Socket _client;

        public SocketMessageController(Socket socket)
        {

            _client = socket;

        }


        protected override bool CanRead { get { return _client.Connected; } }

        protected override string ReadMessage(byte[] buffer)
        {
            int read = _client.Receive(buffer);
            return Encoding.ASCII.GetString(buffer);
        }

        protected override void SendMessage(string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            int bytesLength = bytes.Length;

            if (bytesLength > BufferSize)
            {
                throw new Exception("Too large message to send!");
            }

            Array.Resize(ref bytes, BufferSize);
            for (int i = bytesLength; i < BufferSize; i++)
            {
                bytes[i] = (byte)' ';
            }

            _client.Send(bytes);
        }


    }
}
