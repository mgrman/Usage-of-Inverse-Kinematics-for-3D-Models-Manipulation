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
    /// Controler which handles send and receive of messages directly using Streams
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public class RawStreamController<CONTYPE> : BaseConnectionController<CONTYPE> where CONTYPE : IConnection, new()
    {
        Stream _inStream;
        Stream _outStream;

        byte[] _sendBuffer = new byte[BufferSize];
        

        public RawStreamController(Stream inStream, Stream outStream)
        {

            _inStream =inStream;
            _outStream = outStream;

        }


        protected override bool CanRead { get { return _inStream.CanRead; } }

        protected override string ReadMessage(byte[] buffer)
        {
            int read = _inStream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer);
        }

        protected override void SendMessage(string message)
        {

            int bytesLength = Encoding.ASCII.GetBytes(message, 0, message.Length, _sendBuffer, 0);

            if (bytesLength > BufferSize)
            {
                throw new Exception("Too large message to send!");
            }

            for (int i = bytesLength; i < BufferSize; i++)
            {
                _sendBuffer[i] = (byte)' ';
            }


            _outStream.Write(_sendBuffer, 0, _sendBuffer.Length);
            _outStream.Flush();
        }


    }
}
