using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class VrpnConnection : BaseConnection
{
    [Serializable]
    public class VRPNSettings
    {

        public string ServerName;

        public string ServerIPAdress;
        public int Sensor;

    }

    public VRPNSettings _VRPNSettings;

    private static VRPNManager _manager;
    private static int _managerIndex = 0;
    private VRPNTracker _tracker;


    protected override bool GetConnected()
    {

        if (_ConnectionType == ConnectionTypes.DirectConnection)
            return _tracker.GetConnected();
        else
            return base.GetConnected();
    }

    protected override void Awake()
    {

        if (_ConnectionType == ConnectionTypes.DirectConnection)
        {
            _tracker = gameObject.AddComponent<VRPNTracker>();
            _tracker.TrackerName = _VRPNSettings.ServerName;
            _tracker.TrackerType = VRPNManager.Tracker_Types.vrpn_Tracker_Liberty;
            _tracker.SensorNumber = _VRPNSettings.Sensor;
        }
        if (_manager == null)
        {
            _manager = gameObject.AddComponent<VRPNManager>();
            _manager.ServerAddress = _VRPNSettings.ServerIPAdress;

        }
        _managerIndex++;

        base.Awake();
    }

    protected override void Update()
    {
        base.Update();


        if (_ConnectionType == ConnectionTypes.DirectConnection)
        {
            if (_UpdatePosition)
            {
                _LastPosition = _tracker.GetPosition();
            }
            if (_UpdateRotation)
            {
                _LastRotation = _tracker.GetRotation();
            }
        }
    }

    protected override void OnDestroy()
    {
        if (_tracker != null)
        {
            Destroy(_tracker);
        }
        _managerIndex--;
        if (_managerIndex == 0)
        {
            Destroy(_manager);
        }
        base.OnDestroy();
    }

    protected override string ResourceName { get { return "TrackingRelay_VRPN"; } }

    protected override string ConnectionProgramName { get { return "TrackingRelay_VRPN.exe"; } }

    protected override Dictionary<string, string> ConnectionDictionary
    {
        get
        {
            var args = new Dictionary<string, string>()
            {
                {"serverName",_VRPNSettings.ServerName},
                {"serverAdress",_VRPNSettings.ServerIPAdress},
                {"sensor",_VRPNSettings.Sensor.ToString()}
            };
            return args;
        }
    }


    public override string ToString()
    {
        return string.Format("{0}@{1} Sensor:{2}", _VRPNSettings.ServerName, _VRPNSettings.ServerIPAdress, _VRPNSettings.Sensor);
    }

}
