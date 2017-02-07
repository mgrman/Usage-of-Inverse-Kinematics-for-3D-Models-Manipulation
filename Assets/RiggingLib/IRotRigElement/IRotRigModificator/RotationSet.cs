using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotationSet : IRotRigModifier
{
    public IRotRigElement ParentNode { get; set; }

    public Vector3 Rotation { get; set; }

    public Transform BoundObject { get; private set; }

    public RotationSet(IRotRigElement parent, Transform elem, Vector3 rotEuler)
        : this(parent,elem)
    {
        Rotation = rotEuler;
    }



    public RotationSet(IRotRigElement parent, Transform elem)
    {
        if (elem == null)
            throw new ArgumentNullException();

        BoundObject = elem;

        ParentNode = parent;
        ParentNode.AddModifier(this);

        Rotation = new Vector3(float.NaN, float.NaN, float.NaN);
    }




    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations, bool useLocal)
    {
        return rotations
            .Select(rot =>
            {
                var euler = rot.eulerAngles;

                if (!float.IsNaN(Rotation.x))
                    euler.x = Rotation.x;

                if (!float.IsNaN(Rotation.y))
                    euler.y = Rotation.y;

                if (!float.IsNaN(Rotation.z))
                    euler.z = Rotation.z;

                return Quaternion.Euler(euler);
            });
    }
}



public static class RotationSetExtensions
{

    public static IRotRigElement AddRotationSet(this IRotRigElement element, Transform elem, Vector3 rotEuler)
    {
        new RotationSet(element, elem,rotEuler);
        return element;
    }
}