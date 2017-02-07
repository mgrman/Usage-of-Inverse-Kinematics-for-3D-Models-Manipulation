using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FrameDelayModifier : IRotRigModifier
{
    public IRotRigElement ParentNode { get; set; }

    private Queue<IEnumerable<Quaternion>> _history;

    private int _historyLength;

    /// <summary>
    /// Automaticaly adds itself to element's modifiers
    /// </summary>
    public FrameDelayModifier(IRotRigElement element, int frameDelay)
    {
        ParentNode = element;
        ParentNode.AddModifier(this);

        _historyLength = frameDelay;
        _history = new Queue<IEnumerable<Quaternion>>(frameDelay);

    }


    public IEnumerable<Quaternion> UpdateElement(IEnumerable<Quaternion> rotations, bool useLocal)
    {
        IEnumerable<Quaternion> newRotations = null;
        if (_history.Count == _historyLength)
        {
                newRotations = _history.Dequeue();
        }
        else
        {
            newRotations = rotations;
        }

        _history.Enqueue(rotations);

        return newRotations;
    }
    

}


public static class FrameDelayModifExtensions
{
    public static IRotRigElement AddFrameDelayModif(this IRotRigElement element,int frameDelay)
    {
        new FrameDelayModifier(element, frameDelay);
        return element;
    }
}

