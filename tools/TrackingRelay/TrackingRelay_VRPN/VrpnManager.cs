using TrackingRelay_Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_VRPN
{
    public class VrpnManager : IConnection
    {
        static Vrpn.Connection _connection;
        static int _connectionCounter;

        static Object _connectionLock = new Object();

        Vrpn.TrackerServer _server;
        Vrpn.TrackerRemote _client;
        bool _requestStopServer;
        bool _requestStopClient;

        Task _serverLoopTask;
        Task _clientLoopTask;

        MessageCollection _linesBuffer;

        Object _linesBuffer_lock = new Object();


        Vector3 _currentPosition;
        Vector3 _currentRotation;

        public Vector3? BroadcastPosition { get; set; }
        public Vector3? BroadcastPosition_Noise { get; set; }

        public bool ServerConnected { get { return _server != null; } }

        public bool ClientConnected { get { return _client != null; } }

        public Vector3 CurrentPosition { get { return _currentPosition; } }

        public Vector3 CurrentRotation { get { return _currentRotation; } }

        public VrpnManager()
            : this(false)
        {


        }

        public VrpnManager(bool enableMessages)
        {
            if (enableMessages)
            {
                _linesBuffer = new MessageCollection(2000);
            }
            else
            {
                _linesBuffer = null;
            }
        }

        public event EventHandler ConnectionChanged;
        private void FireConnectionChanged()
        {
            var handler = ConnectionChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public MessageCollection Messages
        {
            get
            {
                return _linesBuffer;
            }
        }

        public void UpdateMessages()
        {
            if (_linesBuffer == null)
                return;
            lock (_linesBuffer_lock)
            {
                _linesBuffer.UpdateContainers();
            }
        }

        public void RunServer(string serverName, int? port)
        {
            lock (_connectionLock)
            {
                if (_connection == null)
                {
                    if (port.HasValue)
                        _connection = Vrpn.Connection.CreateServerConnection(port.Value);
                    else
                        _connection = Vrpn.Connection.CreateServerConnection();
                    _connectionCounter = 0;

                }
                _connectionCounter++;

                _server = new Vrpn.TrackerServer(serverName, _connection, 1);
            }

            FireConnectionChanged();

            _serverLoopTask = Task.Run(() => ServerLoop());
        }

        public void KillServer()
        {
            if (_server == null)
                return;

            _requestStopServer = true;

            _serverLoopTask.Wait();

            FireConnectionChanged();
        }

        public void RunClient(string serverName, string serverAdress, int sensor)
        {
            serverAdress = string.IsNullOrEmpty(serverAdress) ? "localhost" : serverAdress;
            //var connection = Vrpn.Connection.CreateServerConnection (port.Value);
            _client = new Vrpn.TrackerRemote(String.Format("{0}@{1}", serverName, serverAdress));

            _client.MuteWarnings = false;

            _client.PositionChanged += (o1, e1) =>
            {
                if (e1.Sensor != sensor)
                    return;
                Vrpn.Vector3 axis;
                double angle;
                e1.Orientation.ToAxisAngle(out axis, out angle);
                axis.Normalize();

                double x = axis.X;
                double y = axis.Y;
                double z = axis.Z;

                double heading;
                double attitude;
                double bank;


                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);
                double t = 1 - cos;

                if ((x * y * t + z * sin) > 0.998)
                {
                    heading = 2 * Math.Atan2(x * Math.Sin(angle / 2), Math.Cos(angle / 2));
                    attitude = Math.PI / 2;
                    bank = 0;
                }
                else if ((x * y * t + z * sin) < -0.998)
                {

                    heading = -2 * Math.Atan2(x * Math.Sin(angle / 2), Math.Cos(angle / 2));
                    attitude = -Math.PI / 2;
                    bank = 0;
                }
                else
                {

                    heading = Math.Atan2(y * sin - x * z * (1 - cos), 1 - (y * y + z * z) * (1 - cos));
                    attitude = Math.Asin(x * y * (1 - cos) + z * sin);
                    bank = Math.Atan2(x * sin - y * z * (1 - cos), 1 - (x * x + z * z) * (1 - cos));
                }
                _currentPosition = e1.Position.ToMyVector();
                _currentRotation = new Vector3(heading, attitude, bank);

                if (_linesBuffer == null)
                    return;

                AddLine("sensor[{0}] pos[{1}, {2}, {3}] orientation[{4}, {5}, {6}] time[{7}]\r\n",
                    e1.Sensor.ToString().PadLeft(2),
                    e1.Position.X.ToString("F2").PadLeft(10),
                    e1.Position.Y.ToString("F2").PadLeft(10),
                    e1.Position.Z.ToString("F2").PadLeft(10),
                    heading.ToString("F2").PadLeft(10),
                    attitude.ToString("F2").PadLeft(10),
                    bank.ToString("F2").PadLeft(10),
                    e1.Time.ToString("hh:mm:ss"));

            };

            FireConnectionChanged();

            _clientLoopTask = Task.Run(() => ClientLoop());

        }

        public void KillClient()
        {
            if (_client == null)
                return;
            _requestStopClient = true;

            _clientLoopTask.Wait();

            FireConnectionChanged();
        }


        private async Task ServerLoop()
        {
            int counter = 0;

            Random rnd = new Random();

            while (!_requestStopServer)
            {
                await Task.Delay(10);
                lock (_connectionLock)
                {
                    _connection.Update();
                }

                Vector3 pos;
                if (BroadcastPosition.HasValue && BroadcastPosition_Noise.HasValue)
                {
                    double period=3;
                    var val = Math.Sin((double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond / period*Math.PI*2);
                    pos = BroadcastPosition.Value + BroadcastPosition_Noise.Value * val;
                }
                else if (BroadcastPosition.HasValue)
                {
                    pos = BroadcastPosition.Value;
                }
                else
                {
                    pos = new Vector3(counter, rnd.NextDouble(), DateTime.Now.Second);
                }
                _server.ReportPose(0, DateTime.Now, pos.ToVrpn(), new Vrpn.Quaternion());
                counter++;
                _server.Update();
            }
            _requestStopServer = false;
            _server.Dispose();
            _server = null;

            lock (_connectionLock)
            {
                _connectionCounter--;

                if (_connectionCounter <= 0)
                {
                    _connection.Dispose();
                    _connection = null;

                }
            }
        }

        private async Task ClientLoop()
        {
                //_client.UpdateRate = 100;
            while (!_requestStopClient)
            {
                await Task.Delay(10);
                _client.Update();

            }
            _requestStopClient = false;

            _client.Dispose();
            _client = null;
        }


        private void AddLine(string message)
        {
            if (_linesBuffer == null)
                return;

            lock (_linesBuffer_lock)
            {
                _linesBuffer.AddMessage(message);
            }
        }

        private void AddLine(string message, params object[] args)
        {
            AddLine(string.Format(message, args));
        }

        #region IConnection
        void IConnection.RunServer(Dictionary<string, string> connectionData)
        {
            RunServer(connectionData["serverName".ToLower()], null);
        }

        void IConnection.KillServer()
        {
            KillServer();
        }

        bool IConnection.RunClient(Dictionary<string, string> connectionData)
        {
            if (!connectionData.ContainsKey("serverName".ToLower()))
                return false;
            if (!connectionData.ContainsKey("serverAdress".ToLower()))
                return false;
            if (!connectionData.ContainsKey("sensor".ToLower()))
                return false;

            RunClient(connectionData["serverName".ToLower()], connectionData["serverAdress".ToLower()], Int32.Parse(connectionData["sensor".ToLower()]));
            return _client != null;
        }

        void IConnection.KillClient()
        {
            throw new NotImplementedException();
        }

        Vector3 IConnection.CurrentPosition
        {
            get { return CurrentPosition; }
        }

        Vector3 IConnection.CurrentRotation
        {
            get { return CurrentRotation; }
        }

        #endregion

    }


    public static class Vrpn_Vector3Extensions
    {
        public static Vector3 ToMyVector(this Vrpn.Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
        public static Vrpn.Vector3 ToVrpn(this Vector3 vec)
        {
            return new Vrpn.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}
