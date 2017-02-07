using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Globalization;

public class SocketConnection
{
    const int BufferSize = 100;

    static Regex _dataRegex = new Regex(@"\[(.+?):(.+?);(.+?);(.+?)\]");

    Stream _inStream;
    Stream _outStream;
    Socket _client;
    System.Threading.Thread _readThread;

    Vector3 _lastPositionMessage = new Vector3();
    Quaternion _lastRotationMessage = new Quaternion();
    object _messageBuffer_lock = new object();


    byte[] _sendBuffer = new byte[BufferSize];
    byte[] _readBuffer = new byte[BufferSize];

    IPAddress _adress;
    int _port;



    public SocketConnection(IPAddress adress, int port)
    {
        _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _client.NoDelay = true;

        _client.ReceiveBufferSize = 256;
        _client.SendBufferSize = 256;

        _adress=adress;
        _port = port;

        StartReaderLoop();

        TryConnect();
    }

    public IPAddress Adress
    {
        get
        {
            return _adress;
        }
    }

    public int Port
    {
        get
        {
            return _port;
        }
    }

    public Vector3 LastPositionMessage
    {
        get
        {
            return _lastPositionMessage;
        }
    }

    public Quaternion LastRotationMessage
    {
        get
        {
            return _lastRotationMessage;
        }
    }

    public bool Connected
    {
        get
        {
            return _client.Connected;
        }
    }

    public void TryConnect()
    {
        if (Connected)
        {
            Disconnect();
        }

        _client.Connect(_adress, _port);
        var stream = new NetworkStream(_client);
        _inStream = stream;
        _outStream = stream;

    }

    public void Disconnect()
    {

        try
        {
            _client.Disconnect(true);
        }
        catch { }

        if (_inStream != null)
        {
            try
            {
                _inStream.Close();
            }
            catch { }
            try
            {
                _inStream.Dispose();
            }
            catch { }
            _inStream = null;
        }
        if (_outStream != null)
        {
            try
            {
                _outStream.Close();
            }
            catch { }
            try
            {
                _outStream.Dispose();
            }
            catch { }
            _outStream = null;
        }
    }

    public void WriteMessage(string message)
    {
        if (!Connected)
            return;

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




    void StartReaderLoop()
    {

        Action readStream = () =>
        {
                Queue<double> fpsBuffer = new Queue<double>(10);

                long oldTime = DateTime.Now.Ticks;
                while (true)
                {
                    if (_inStream == null || !_inStream.CanRead)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    try
                    {
                        int read = _inStream.Read(_readBuffer, 0, _readBuffer.Length);
                        string line = Encoding.ASCII.GetString(_readBuffer);

                        //print(Encoding.ASCII.GetByteCount(line));
                        if (line.ToLower().Contains("getpositions"))
                        {
                            var msg = MessageToVector(line);


                            //var fps=((double)(DateTime.Now.Ticks - oldTime)) / TimeSpan.TicksPerMillisecond;
                            //if(fpsBuffer.Count==10)
                            //    fpsBuffer.Dequeue();
                            //fpsBuffer.Enqueue(fps);

                            //MonoBehaviour.print(string.Format("position : {0} - {1}", msg,fpsBuffer.Average() ));
                            //oldTime = DateTime.Now.Ticks;

                            if (msg != null)
                            {
                                lock (_messageBuffer_lock)
                                {
                                    _lastPositionMessage = msg.Value;
                                }
                            }
                        }

                        if (line.ToLower().Contains("getrotations"))
                        {
                            var msg = MessageToVector(line);

                            //MonoBehaviour.print(string.Format("rotation : {0} - {1}", msg, ((double)(DateTime.Now.Ticks - oldTime)) / TimeSpan.TicksPerMillisecond));
                            //oldTime = DateTime.Now.Ticks;

                            if (msg != null)
                            {
                                lock (_messageBuffer_lock)
                                {
                                    _lastRotationMessage = Quaternion.Euler(msg.Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MonoBehaviour.print(ex.Message);
                    }
                }
            
        };

        _readThread = new System.Threading.Thread(new System.Threading.ThreadStart(readStream));
        _readThread.IsBackground = true;
        _readThread.Start();
    }




    Vector3? MessageToVector(string message)
    {

        var match = _dataRegex.Match(message);
        if (match == null)
            return null;

        float pars;
        int parsint;
        int client = int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsint) ? parsint : 0;
        float x = float.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out pars) ? pars : float.NaN;
        float y = float.TryParse(match.Groups[3].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out pars) ? pars : float.NaN;
        float z = float.TryParse(match.Groups[4].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out pars) ? pars : float.NaN;

        if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z))
            return null;

        return new Vector3(x, y, z);


    }

    public void Dispose()
    {
        if (_readThread != null)
        {
            _readThread.Abort();
        }
        Disconnect();
        if (_client != null)
        {
            try
            {
                _client.Close();
            }
            catch { }
            _client = null;
        }
    }


}
