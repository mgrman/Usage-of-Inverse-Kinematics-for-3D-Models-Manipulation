using System;
using UnityEngine;
using System.Collections.Generic;

public interface IRotRigElement
{

    IEnumerable<Quaternion> UpdateElement(bool useLocal);

    Transform[] BoundObjects{ get; }

    void AddModifier(IRotRigModifier modif);
}

public static class IRotRigElementExtensions
{
    public static T AddRotRigElem<T>(this IRigControler controler, T elem) where T : IRotRigElement
    {
        controler.AddApplicator(new RotationRigAplicator(elem));
        return elem;
    }

    public static T AddRotRigElem<T>(this IRigControler controler, T elem, bool useLocal) where T : IRotRigElement
    {
        controler.AddApplicator(new RotationRigAplicator(elem, useLocal));
        return elem;
    }


}