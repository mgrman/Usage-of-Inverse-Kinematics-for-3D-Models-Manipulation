using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotationOffsetModif : IRotRigModifier
{
    public Quaternion[] Rotation{ get; set; }
    
    public IRotRigElement ParentNode { get; set; }
     
    public bool UseFirstOnAll { get; set; }

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public RotationOffsetModif(IRotRigElement elem,Quaternion rot)
        :this(elem)
    {
        Rotation = new Quaternion[]{rot};
    }

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public RotationOffsetModif(IRotRigElement elem,Vector3 rotEuler)
        :this(elem)
    {
        Rotation =new Quaternion[]{ Quaternion.Euler(rotEuler)};
    }

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public RotationOffsetModif(IRotRigElement elem, Quaternion[] rot)
        : this(elem)
    {
        Rotation = rot;
        UseFirstOnAll = false;
    }

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public RotationOffsetModif(IRotRigElement elem, Vector3[] rotEuler)
        : this(elem)
    {
        Rotation = rotEuler.Select(o => { return Quaternion.Euler(o); }).ToArray();
        UseFirstOnAll = false;
    }

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public RotationOffsetModif(IRotRigElement element)
    {
        if (element == null)
            throw new ArgumentNullException();

        Rotation = new Quaternion[] { new Quaternion() };

        ParentNode = element;
        ParentNode.AddModifier(this);

        UseFirstOnAll = true;

    }


    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations, bool useLocal)
    {
        return rotations
            .Select((rot, i) =>
            {
                var rotIndex = UseFirstOnAll ? 0 : i;

                if (rotIndex >= Rotation.Length)
                    throw new UnityException(string.Format("There arent enough rotations supplied, need at least {0}, have {1}", rotIndex + 1, Rotation.Length));
                return rot * Rotation[rotIndex];
            });
    }

}



public static class RotationOffsetModifExtensions
{
    public static IRotRigElement AddRotationOffsetModif(this IRotRigElement element, Quaternion rot)
    {
        new RotationOffsetModif(element, rot);
        return element;
    }

    public static IRotRigElement AddRotationOffsetModif(this IRotRigElement element, Vector3 rot)
    {
        new RotationOffsetModif(element, rot);
        return element;
    }
    public static IRotRigElement AddRotationOffsetModif(this IRotRigElement element, Vector3[] rot)
    {
        new RotationOffsetModif(element, rot);
        return element;
    }
    public static IRotRigElement AddRotationOffsetModif(this IRotRigElement element, Quaternion[] rot)
    {
         new RotationOffsetModif(element, rot);
        return element;
    }
    public static IRotRigElement AddRotationOffsetModif(this IRotRigElement element)
    {
       new RotationOffsetModif(element);
        return element;
    }

}