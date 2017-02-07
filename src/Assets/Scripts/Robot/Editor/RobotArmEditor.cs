using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[CustomEditor(typeof(RobotArm))]
public class RobotArmEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Edit links");

        var controller = target as RobotArm;

        var links = new List<Transform>();
        var child = controller.transform;
        while (child.childCount != 0 && child.name.StartsWith("joint"))
        {
            links.Add(child);
            child = child.GetChild(0);
        }

        for (int i = 0; i < links.Count - 1; i++)
        {
            var start = links[i];
            var end = links[i + 1];
            var length = Mathf.Abs(end.localPosition.z);

            EditorGUILayout.BeginHorizontal();

            var newLength = EditorGUILayout.FloatField(string.Format("Length of bone {0}", i + 1), length);
            if (i != 0)
            {

                if (GUILayout.Button("Delete"))
                {
                    var localRot = start.localRotation;

                    end.SetParent(start.parent, false);
                    end.SetAsFirstSibling();
                    DestroyImmediate(start.gameObject);
                    end.localRotation = localRot;

                    return;
                }
            }
            start.name = string.Format("joint_{0}", i );

            EditorGUILayout.EndHorizontal();

            if (Mathf.Abs(newLength - length) > 0.0001)
                end.localPosition = new Vector3(0, 0, newLength);

            var visualNode = start.GetChild(Mathf.Max(start.childCount - 1, 1));
            var bone = visualNode.Cast<Transform>().Where(o => o.name.StartsWith("bone")).FirstOrDefault();

            if (bone != null)
            {
                bone.localPosition = new Vector3(0, 0, newLength / 2);
                bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y, newLength);
            }

        }
        links[links.Count - 1].name = string.Format("joint_{0}", links.Count - 1);


        if (GUILayout.Button("Add bone"))
        {
            var last = links[links.Count - 1];

            var newChild = Instantiate(last);
            newChild.parent = last;
            newChild.name = string.Format("joint_{0}", links.Count + 1);
            newChild.SetAsFirstSibling();
            newChild.GetChild(0).name = "visual_" + (links.Count + 1).ToString();
            newChild.localPosition = last.localPosition;
        }

        var ikCtrl = controller.GetComponent<RobotIKControler>();
        if (ikCtrl != null)
        {
            ikCtrl._LastNode = links[links.Count - 1];
        }
    }
}

