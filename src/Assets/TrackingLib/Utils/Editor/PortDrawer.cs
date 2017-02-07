using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net.Sockets;
using System.Net;

[CustomPropertyDrawer(typeof(PortAttribute))]
public class PortDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {

        EditorGUI.PropertyField(new Rect(position.xMin,position.yMin,position.width/2,position.height), property, label, true);


        var res = EditorGUI.Toggle(new Rect(position.xMin + position.width / 2, position.yMin, position.width / 2, position.height), "Find free port", false);
        if (res)
        {
            property.intValue = FreeTcpPort();

        }
    }

    static int FreeTcpPort()
    {
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
}