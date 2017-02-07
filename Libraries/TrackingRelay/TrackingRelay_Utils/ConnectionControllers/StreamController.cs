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
    /// Controler which handles send and receive of messages directly using StreamReader and StreamWriter
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public class StreamController<CONTYPE> : BaseConnectionController<CONTYPE> where CONTYPE : IConnection, new()
    {

        StreamReader _inStream;
        StreamWriter _outStream;

        public StreamController(Stream inStream,Stream outStream)
        {

            _inStream =new StreamReader( inStream);
            _outStream =new StreamWriter( outStream);

        }



        protected override bool CanRead { get { return _inStream.BaseStream.CanRead; } }

        protected override string ReadMessage(byte[] buffer)
        {
            return _inStream.ReadLine() ?? "";
        }

        protected override void SendMessage(string message)
        {
            _outStream.WriteLine(message);
            _outStream.Flush();
        }


    }
}
