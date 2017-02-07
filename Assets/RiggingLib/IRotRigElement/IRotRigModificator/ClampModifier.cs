using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClampModifier : IRotRigModifier
{
    public IRotRigElement ParentNode { get; set; }

    public Vector3 MinValues { get; set; }
    public Vector3 MaxValues { get; set; }


    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public ClampModifier(IRotRigElement element, Vector3 minValues, Vector3 maxValues)
    {
        ParentNode = element;
        ParentNode.AddModifier(this);

        MinValues = minValues;
        MaxValues = maxValues;

    }


    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations, bool useLocal)
    {
        return rotations
            .Select(rot =>
            {
                var angles = rot.eulerAngles;

                if (Math.Abs(angles.x) > 360.0f)
                    angles.x = (Math.Abs(angles.x) % 360.0f) * Mathf.Sign(angles.x);

                if (Math.Abs(angles.y) > 180.0f)
                    angles.y = (Math.Abs(angles.y) % 360.0f) * Mathf.Sign(angles.y);

                if (Math.Abs(angles.z) > 180.0f)
                    angles.z = (Math.Abs(angles.z) % 360.0f) * Mathf.Sign(angles.z);

                if (angles.x > 180.0f)
                    angles.x = angles.x - 360.0f;
                if (angles.y > 180.0f)
                    angles.y = angles.y - 360.0f;
                if (angles.z > 180.0f)
                    angles.z = angles.z - 360.0f;

                if (!float.IsNaN(MinValues.x))
                    angles.x = Math.Max(MinValues.x, angles.x);

                if (!float.IsNaN(MinValues.y))
                    angles.y = Math.Max(MinValues.y, angles.y);

                if (!float.IsNaN(MinValues.z))
                    angles.z = Math.Max(MinValues.z, angles.z);


                if (!float.IsNaN(MaxValues.x))
                    angles.x = Math.Min(MaxValues.x, angles.x);

                if (!float.IsNaN(MaxValues.y))
                    angles.y = Math.Min(MaxValues.y, angles.y);

                if (!float.IsNaN(MaxValues.z))
                    angles.z = Math.Min(MaxValues.z, angles.z);

                return Quaternion.Euler(angles);
            });
    }


}


public static class ClampModifModifExtensions
{
    public static IRotRigElement AddClampModif(this IRotRigElement element, Vector3 minValues, Vector3 maxValues)
    {
        new ClampModifier(element, minValues, maxValues);
        return element;
    }
}

