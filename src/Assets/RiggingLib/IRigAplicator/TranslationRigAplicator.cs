
using System;
using UnityEngine;

public class TranslationRigAplicator:IRigAplicator
{
    public ITranRigElement Element;   

    public TranslationRigAplicator(ITranRigElement elem)
    {
        Element = elem;
    }

    public void Update()
    {

        var translations=Element.UpdateElement();
        for (int i=0; i<Element.BoundObjects.Length; i++)
        {
            Element.BoundObjects[i].position=translations[i];
        }

    }

}


