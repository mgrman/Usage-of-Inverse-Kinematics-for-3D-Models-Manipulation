using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITranRigElement
{
    Vector3[] UpdateElement();

    Transform[] BoundObjects{ get; }
}


public static class ITranRigElementExtensions
{
    public static T AddTraRigElem<T>(this IRigControler controler, T elem) where T : ITranRigElement
    {
        controler.AddApplicator(new TranslationRigAplicator(elem));
        return elem;
    }

}