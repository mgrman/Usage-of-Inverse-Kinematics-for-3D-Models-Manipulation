using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum InvertedState
{
    Nothing = 0,
    InvertX = 1 << 0,
    InvertY = 1 << 1,
    InvertZ = 1 << 2,
}

public class InvertRotation : IRotRigModifier
{
    public IRotRigElement ParentNode { get; set; }

    private InvertedState _invertedState;
    public InvertedState InvertedState
    {
        get { return _invertedState; }
        set
        {
            _invertedState = value;

            //MonoBehaviour. print(value);

            if ((value & (InvertedState.InvertX | InvertedState.InvertY | InvertedState.InvertZ)) == (InvertedState.InvertX | InvertedState.InvertY))
            {
                _rotOffset = _rotXYZ;
                return;
            }


            if ((value & (InvertedState.InvertX | InvertedState.InvertY)) == (InvertedState.InvertX | InvertedState.InvertY))
            {
                _rotOffset = _rotXY;
                return;
            }

            if ((value & (InvertedState.InvertY | InvertedState.InvertZ)) == (InvertedState.InvertY | InvertedState.InvertZ))
            {
                _rotOffset = _rotYZ;
                return;
            }

            if ((value & (InvertedState.InvertX | InvertedState.InvertZ)) == (InvertedState.InvertX | InvertedState.InvertZ))
            {
                _rotOffset = _rotXZ;
                return;
            }


            if ((value & (InvertedState.InvertX)) == (InvertedState.InvertX))
            {
                _rotOffset = _rotX;
                return;
            }

            if ((value & (InvertedState.InvertY)) == (InvertedState.InvertY))
            {
                _rotOffset = _rotY;
                return;
            }

            if ((value & (InvertedState.InvertZ)) == (InvertedState.InvertZ))
            {
                _rotOffset = _rotZ;
                return;
            }
            _rotOffset = Quaternion.identity;
        }
    }


    private static readonly Quaternion _rotX = Quaternion.Euler(new Vector3(180, 0, 0));
    private static readonly Quaternion _rotY = Quaternion.Euler(new Vector3(0, 180, 0));
    private static readonly Quaternion _rotZ = Quaternion.Euler(new Vector3(0, 0, 180));


    private static readonly Quaternion _rotXY = Quaternion.Euler(new Vector3(180, 180, 0));
    private static readonly Quaternion _rotYZ = Quaternion.Euler(new Vector3(0, 180, 180));
    private static readonly Quaternion _rotXZ = Quaternion.Euler(new Vector3(180, 0, 180));

    private static readonly Quaternion _rotXYZ = Quaternion.Euler(new Vector3(180, 180, 180));

    private Quaternion _rotOffset;

    public InvertRotation(IRotRigElement parent, InvertedState invertedState)
    {
        ParentNode = parent;
        ParentNode.AddModifier(this);

        InvertedState = invertedState;
    }






    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations, bool useLocal)
    {
        return rotations
            .Select(rot =>
            {
                return rot * _rotOffset;
            });
    }
}



public static class InvertRotationExtensions
{

    public static IRotRigElement AddInvertRotation(this IRotRigElement element, InvertedState invertedState)
    {
        new InvertRotation(element, invertedState);
        return element;
    }
}