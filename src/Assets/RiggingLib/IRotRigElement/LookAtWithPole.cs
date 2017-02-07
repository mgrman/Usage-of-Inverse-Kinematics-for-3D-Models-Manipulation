using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAtWithPole : BaseModifiableRotRigElement
{
    public Transform Target{ get; private set; }

    public Transform Pole { get; private set; }
    public Vector3 PoleVectorFallBack { get; private set; }

    public Transform Object { get; private set; }



    private Vector3 _polePosition
    {
        get
        {
            if (Pole != null)
                return Pole.position;
            else
                return Object.position + PoleVectorFallBack;
        }
    }


    public LookAtWithPole(Transform obj, Transform target, Transform pole)
    {
        Object = obj;
        Target = target;
        Pole = pole;
        PoleVectorFallBack = Vector3.up;
    }
    public LookAtWithPole(Transform obj, Transform target, Vector3 poleVector)
    {
        Object = obj;
        Target = target;
        PoleVectorFallBack = poleVector;
    }


    protected override Quaternion[] UpdateElementImpl(bool useLocal)
    {

        if (Object)
        {
            Vector3 pole =  _polePosition - Object.position ;
            var angles = Quaternion.LookRotation(Target.position - Object.position, pole).eulerAngles;
            angles.z = 0;


            Quaternion rot = Quaternion.Euler(angles);



            if (useLocal)
                rot = IKRigging_Utils.GetLocalRotation(Object, rot);

            return new Quaternion[] { rot };

        }
        return new Quaternion[]{ new Quaternion()};

    }

    public override Transform[] BoundObjects
    {
        get
        {
            return new Transform[]{Object};
        }
    }
}

public static class LookAtWithPoleExtensions
{


    public static LookAtWithPole AddLookAtWithPole(this IRigControler controler, Transform obj, Transform target, Transform pole, bool useLocal = true)
    {
        return controler.AddRotRigElem(new LookAtWithPole(obj, target, pole), useLocal);
    }

    public static LookAtWithPole AddLookAtWithPole(this IRigControler controler, Transform obj, Transform target, Vector3 poleVector, bool useLocal = true)
    {
        return controler.AddRotRigElem(new LookAtWithPole(obj, target, poleVector), useLocal);
    }
}