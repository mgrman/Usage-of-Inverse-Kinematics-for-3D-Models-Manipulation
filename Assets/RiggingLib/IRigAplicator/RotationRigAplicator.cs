
using System;
using UnityEngine;

public class RotationRigAplicator:IRigAplicator
{
    public IRotRigElement Element { get; set; }

    public bool UseLocal { get; set; }

    public RotationRigAplicator(IRotRigElement elem,bool useLocal=true)
    {
        Element = elem;
        UseLocal = useLocal;
    }

    public void Update()
    {
        var rotations = Element.UpdateElement(UseLocal);
        var enumerator = rotations.GetEnumerator();

        for (int i=0; i<Element.BoundObjects.Length; i++)
        {
            if (!enumerator.MoveNext())
                break;

            var rot = enumerator.Current;

            var obj = Element.BoundObjects[i];
            if (UseLocal)
                obj.localRotation = rot;
            else
                obj.rotation = rot;

            //var e = rotations[i].eulerAngles;
            //MonoBehaviour.print(e);
            //e = new Vector3(0,e.y+90, e.x+180);
           // obj.rotation = Quaternion.Euler(e);
        }

    }


}


