using UnityEngine;
using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

public class PositionConstraint : ITranRigElement
{
    //public bool Parent;
    public Transform Object;
    public Transform Target;
    //Matrix4x4 _oldMatrix;

    public PositionConstraint(Transform obj, Transform target)//, bool parentConstrain)
    {
        Object = obj;
        Target = target;
        //Parent = parentConstrain;

        //_oldMatrix = Matrix4x4.TRS(Target.position, Target.rotation, Target.localScale);
    }

    public Vector3[] UpdateElement()
    {
        //TODO
        //use local coordinates

        /*
        if (Object == null && Target == null)
            return;

        if (Parent)
        {
            var p = _oldMatrix.inverse.MultiplyPoint(Object.position);

            var mat = Matrix4x4.TRS(Target.position, Target.rotation, Target.localScale);
            Object.position = mat.MultiplyPoint(p);


            _oldMatrix = mat;
        }
        else
        {*/
        return new Vector3[] { Target.position };
        //}

    }

    public Transform[] BoundObjects
    {
        get
        {

            return new Transform[] { Object };
        }
    }

}

public static class PositionConstrainExtensions
{
    public static PositionConstraint AddPositionConstrain(this IRigControler controler, Transform obj, Transform target)
    {
        return controler.AddTraRigElem(new PositionConstraint(obj, target));
    }
}