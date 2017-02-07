using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;

public abstract class BaseConnection : MonoBehaviour
{
    private static List<Type> _initializedConnectionTypes = new List<Type>();



    protected abstract string ResourceName { get; }

    protected abstract string ConnectionProgramName { get; }

    protected abstract Dictionary<string, string> ConnectionDictionary { get; }

    public enum ConnectionTypes
    {
        OwnRelay_TCP,
        RemoteRelay_TCP,
        DirectConnection
    }

    public ConnectionTypes _ConnectionType = ConnectionTypes.OwnRelay_TCP;
    
    public OwnRelaySettings _OwnRelaySettings;

    [Serializable]
    public class OwnRelaySettings
    {
        [Port]
        public int RelayPort;

    }

    public RemoteRelaySettings _RemoteRelaySettings;

    [Serializable]
    public class RemoteRelaySettings
    {

        [Port]
        public int RelayPort;

        public string RelayIpAdress = "127.0.0.1";
    }

    public bool _UpdatePosition;
    public bool _UpdateRotation;

    [ReadOnly]
    public Vector3 _LastPosition;
    [ReadOnly]
    public Quaternion _LastRotation;
    [ReadOnly]
    public bool _Connected;


    static ConnectionUtils.Job _job = new ConnectionUtils.Job();
    System.Diagnostics.Process _proc;

    bool _startMessageSent;
    SocketConnection _connection;


    private string AppDataPath
    {
        get
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }

    private string RelayFolder
    {
        get
        {
            return Path.Combine(Path.Combine(AppDataPath, "TrackingRelay"), ResourceName);
        }
    }
    
    protected virtual void Awake()
    {

    }

    // Use this for initialization
    protected virtual void Start()
    {
        InitializeAssets();
        InitializeRelay();
        InitializeConnection();
    }

    protected virtual void Update()
    {
        _Connected = GetConnected();


        if (!_Connected)
            return;


        if (_ConnectionType != ConnectionTypes.DirectConnection)
        {
            if (!_startMessageSent)
            {
                WriteMessage("startClient", ConnectionDictionary);
                _startMessageSent = true;
            }

            RequestNewData();
        }
    }

    protected virtual void OnDestroy()
    {

        if (_proc != null && !_proc.HasExited)
            _proc.Kill();

        if (_connection!=null)
            _connection.Dispose();
    }

    protected virtual bool GetConnected()
    {
        return _connection != null && _connection.Connected;
    }


    void InitializeAssets()
    {
        var type = this.GetType();

        if (_initializedConnectionTypes.Contains(type))
            return;

        _initializedConnectionTypes.Add(type);

        string assetName = ResourceName;
        var dirName = RelayFolder;
        if (Directory.Exists(dirName))
            Directory.Delete(dirName, true);

        var asset = Resources.Load(assetName) as TextAsset;
        var bytes = asset.bytes;

        if (!Directory.Exists(dirName))
            Directory.CreateDirectory(dirName);

        var zipName = dirName + ".zip";
        File.WriteAllBytes(zipName, bytes);
        ZipUtil.Unzip(zipName, dirName);
        File.Delete(zipName);

    }

    void InitializeRelay()
    {
        switch (_ConnectionType)
        {
            case ConnectionTypes.OwnRelay_TCP:
                {
                    var clientExePath = Path.Combine(RelayFolder, ConnectionProgramName).Replace('/', '\\');

                    var args = new List<string>();
                    args.Add("serverRelay");
                    args.Add(_OwnRelaySettings.RelayPort.ToString());
                    args.Add("noconsole");

                    var startInfo = new System.Diagnostics.ProcessStartInfo(clientExePath, string.Join(" ", args.ToArray()));


                    _proc = new System.Diagnostics.Process();
                    _proc.StartInfo = startInfo;
                    _proc.Start();
#if !UNITY_EDITOR 
                    _job.AddProcess(_proc.Handle);
#endif
                }
                break;
            case ConnectionTypes.RemoteRelay_TCP:
                {
                }
                break;
        }
    }

    void InitializeConnection()
    {
        switch (_ConnectionType)
        {
            case ConnectionTypes.OwnRelay_TCP:
                {
                   
                    _connection=new SocketConnection(IPAddress.Loopback, _OwnRelaySettings.RelayPort);

                }
                break;
            case ConnectionTypes.RemoteRelay_TCP:
                {
                    _connection = new SocketConnection(IPAddress.Parse(_RemoteRelaySettings.RelayIpAdress), _RemoteRelaySettings.RelayPort);

                }
                break;
        }


    }

    void RequestNewData()
    {
        try
        {
            if (_UpdatePosition)
            {
                WriteMessage("getPositions");
            }

            if (_UpdateRotation)
            {
                WriteMessage("getRotations");
            }

            _LastPosition = _connection.LastPositionMessage;
            _LastRotation = _connection.LastRotationMessage;
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
    }

    #region Writing

    void WriteMessage(string message, Dictionary<string, string> args)
    {
        var builder = new StringBuilder();
        builder.Append(message);
        foreach (var pair in args ?? new Dictionary<string, string>())
        {
            builder.AppendFormat(" <{0}|{1}>", pair.Key, pair.Value);
        }
        WriteMessage(builder.ToString());
    }

    void WriteMessage(string message)
    {
        _connection.WriteMessage(message);
    }

    #endregion

}
