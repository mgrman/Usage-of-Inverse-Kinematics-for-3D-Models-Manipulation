
using System;
using System.Collections;

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class IKRigging_Utils
{
    
    public static Vector3 PiecewiseVectorMultiplication(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Quaternion GetLocalRotation(Transform obj,Quaternion globRot)
    {
        return GetLocalRotations(new Transform[] { obj }, new Quaternion[] { globRot })[0];
    }

    public static Quaternion[] GetLocalRotations(Transform[] obj, Quaternion[] globRot)
    {
        var count = Math.Min(obj.Length, globRot.Length);

        Quaternion[] oldRotations = new Quaternion[count];
        Quaternion[] resultingRotations = new Quaternion[count];



        for (int i = 0; i < count;i++ )
        {
            var o = obj[i];
            oldRotations[i]=o.rotation;
        }

        for (int i = 0; i < count; i++)
        {
            var o = obj[i];
            o.rotation = globRot[i];
        }

        for (int i = 0; i < count; i++)
        {
            var o = obj[i];
            resultingRotations[i] = o.localRotation;
        }

        for (int i = 0; i < count; i++)
        {
            var o = obj[i];
            o.rotation = oldRotations[i];
        }
        //var oldRot = obj.rotation;
        //obj.rotation = globRot;
        //var resRot = obj.localRotation;
        //obj.rotation = oldRot;
        return resultingRotations;
    }
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) { return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); }

    public static Quaternion[] MixRotations(List<Quaternion[]> rotations)
    {
        var length = rotations.Min((o) => { return o.Length; });
        var res = Enumerable.Repeat(Quaternion.identity, length).ToArray();

        var alfa = 1.0f / rotations.Count;

        foreach (var rot in rotations)
        {
            for (int i = 0; i < length; i++)
            {
                res[i] *= rot[i];
            }
        }


        for (int i = 0; i < length; i++)
        {
            res[i] = Quaternion.Slerp(Quaternion.identity, res[i], alfa);
        }
        return res;
    }

}


