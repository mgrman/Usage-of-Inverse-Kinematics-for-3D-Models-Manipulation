using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeightRotModif : IRotRigModifier
{
    public IRotRigElement ParentNode { get; set; }

    public float Alpha { get; set; } 

    private Quaternion[] _initialRotationsLocal;

    private Quaternion[] _initialRotationsGlobal;

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public WeightRotModif(IRotRigElement element, float alpha)
    {
        ParentNode = element;
        ParentNode.AddModifier(this);

        Alpha = alpha;

        var objs = ParentNode.BoundObjects;
        _initialRotationsLocal = new Quaternion[objs.Length];
        _initialRotationsGlobal = new Quaternion[objs.Length];

        for (int i=0; i<objs.Length; i++)
        {
            _initialRotationsLocal[i] = objs[i].localRotation;
            _initialRotationsGlobal[i] = objs[i].rotation;
        }
    }


    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations,bool useLocal)
    {
        return rotations
            .Select((rot,i) =>
            {

                Quaternion initRot;
                if (useLocal)
                    initRot = _initialRotationsLocal[i];
                else
                    initRot = _initialRotationsGlobal[i];

                return Quaternion.Slerp(initRot, rot, Alpha);

            });

    }
    

}


public static class WeightRotModifExtensions
{
    public static IRotRigElement AddWeightRotModif(this IRotRigElement element, float alpha)
    {
        new WeightRotModif(element, alpha);
        return element;
    }
}

