using UnityEngine;
using System.Collections;
using System.Linq;
using GeoComputer_IK;
using System.Collections.Generic;

public class IK_Chain : BaseModifiableRotRigElement
{
    //Transform _begin;
    //Transform _middle;
    //Transform _end;

    public enum ComputationTypes
    {
        Convex_Spline,
        Fast_Incremental
    }

    List<Transform> _points = new List<Transform>();
    Transform[] _boundPoints=new Transform[0];
    Transform _target;
    Transform _pole;

    //InvertedState      _inverted;
    private BaseIK _IK;

    Vector3 _lastTargetPos;
    Vector3 _lastPolePos;
    Vector3 _lastStartPos;
    Quaternion[] _resultCache;

    Vector3[] _positionsSpaceCache;

    private bool _wasUpdated;

    public IK_Chain(IEnumerable<Transform> points, Transform target, Transform pole, ComputationTypes type)
        : this(target, pole, type)
    {
        if (points == null)
            return;

        _points.AddRange(points);
        _boundPoints=_points.Take(_points.Count - 1).ToArray();
    }


    public IK_Chain(Transform target, Transform pole,  ComputationTypes type)
    {

        _target = target;
        _pole = pole;
        //_inverted = inverted;


        //_IK = new NgonIK_Incremental();

        switch (type)
        {
            case ComputationTypes.Convex_Spline:
                _IK = new SplineConvexIK() { ConvexityCorrections = true, TargetCorrections = true };
                break;
            case ComputationTypes.Fast_Incremental:
                _IK = new NgonIK_Incremental() { MixType = NgonIK_Incremental.MixTypes.Forward };
                break;
        }
    }


    protected override Quaternion[] UpdateElementImpl(bool useLocal)
    {
        if (_points.Count < 2)
            throw new UnityException("Points must be at least two trasforms");

        _wasUpdated = true;

        var target = _target.position;
        var begin = _points[0].position;
        var pole = _pole != null ? _pole.position : begin + new Vector3(0, 1, 0);


        if (target == _lastTargetPos && pole == _lastPolePos && _lastStartPos==begin)
            return _resultCache;
        _lastTargetPos = target;
        _lastPolePos = pole;
        _lastStartPos = begin;


        var planeToSpaceMat = Matrix4x4.TRS(begin, Quaternion.LookRotation(target - begin, pole - begin), new Vector3(1, 1, 1));
        var spaceToPlaneMat = planeToSpaceMat.inverse;


        var targetPlane = spaceToPlaneMat.MultiplyPoint(target);


        double[] lengths = null;
        Vector[] positionsPlane;
        if (_points.Count >= 3)
        {

            _IK.Target = new Vector(targetPlane.z, targetPlane.y);

            lengths = new double[_points.Count - 1];
            for (int i = 1; i < _points.Count; i++)
                lengths[i - 1] = (_points[i - 1].position - _points[i].position).magnitude;
            _IK.Lengths = lengths;

            //Profiler.BeginSample("InnerIK");
            positionsPlane = _IK.GetPositions();
            //Profiler.EndSample();

        }
        else //must be 2 then
        {
            lengths = new double[] { (_points[1].position - _points[0].position).magnitude };
            positionsPlane = new Vector[] { new Vector(), new Vector(lengths[0], 0) };
        }

        if (positionsPlane == null)
        {
            throw new UnityException(string.Format("IK - acquired points were NULL\r\n legths:{0} \r\n target:{1}", string.Join(", ", lengths.Select(o => o.ToString()).ToArray()), targetPlane.z));
        }


        if (_positionsSpaceCache == null)
            _positionsSpaceCache = new Vector3[positionsPlane.Length];
        Vector3[] positionsSpace = _positionsSpaceCache;
        for (int i = 0; i < positionsSpace.Length; i++)
        {
            var planePoint = positionsPlane[i];

            positionsSpace[i] = planeToSpaceMat.MultiplyPoint(new Vector3(0, (float)planePoint.Y, (float)planePoint.X));
        }

        if(_resultCache==null)
            _resultCache= new Quaternion[positionsSpace.Length - 1];
        Quaternion[] rotations = _resultCache;

        for (int i = 0; i < positionsSpace.Length - 1; i++)
        {
            var localPole = positionsSpace[(i + 2) % positionsSpace.Length] - positionsSpace[i + 1];
            localPole.Normalize();
            var localDir = positionsSpace[i + 1] - positionsSpace[i];
            localDir.Normalize();

            var localPoleQuality = Mathf.Min((localPole - localDir).sqrMagnitude, (localPole + localDir).sqrMagnitude);
            if (localPoleQuality < 0.01)
                localPole =-( pole - positionsSpace[i]);

            rotations[i] = Quaternion.LookRotation(localDir, localPole);
        }



        //MonoBehaviour.print(string.Join("<->", _boundPoints.Select(o => o.gameObject.name).ToArray()));

        //return rotations;

        //MonoBehaviour.print("global:"+string.Join(";", rotations.Select(o => o.eulerAngles.ToString()).ToArray()));
        if (useLocal)
        {
            rotations = IKRigging_Utils.GetLocalRotations(BoundObjects, rotations);

            //MonoBehaviour.print("local:" + string.Join(";", rotations.Select(o => o.eulerAngles.ToString()).ToArray()));
        }
        _resultCache = rotations;
        return rotations;
    }

    public override Transform[] BoundObjects
    {
        get
        {
            return _boundPoints;
        }
    }

    public IK_Chain BindPoint(Transform point)
    {
        if (_wasUpdated)
            throw new UnityException("Can only bind points before UpdateElement was first called!");


        _points.Add(point);
        _boundPoints = _points.Take(_points.Count - 1).ToArray();
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start">Start of IK Chain</param>
    /// <param name="end">End of IK chain, must be in child of end</param>
    /// <returns></returns>
    public IK_Chain BindChain(Transform start,Transform end)
    {
        if (_wasUpdated)
            throw new UnityException("Can only bind points before UpdateElement was first called!");


        bool startEncountered = false;
        var chain = GetParents(end)
            .TakeWhile(o => {
                var lastWasStart=startEncountered;
                startEncountered = o == start;
                return !lastWasStart;
            })
            .Reverse()
            .ToArray();

        if (chain[0] != start)
            throw new UnityException("End must be in hierarchy od Start");

        _points.AddRange(chain);
        _boundPoints = _points.Take(_points.Count - 1).ToArray();
        return this;
    }

    private IEnumerable<Transform> GetParents(Transform elem)
    {
        while (elem != null)
        {
            yield return elem;
            elem=elem.parent;
        }
    }
}

public static class IK_ChainExtensions
{
    public static IK_Chain AddIK_Chain(this IRigControler controler, Transform[] points, Transform target, Transform pole,IK_Chain.ComputationTypes type,bool useLocal=true)
    {
        return controler.AddRotRigElem(new IK_Chain(points, target, pole, type), useLocal);
    }
    public static IK_Chain AddIK_Chain(this IRigControler controler, Transform target, Transform pole,  IK_Chain.ComputationTypes type, bool useLocal = true)
    {
        return controler.AddRotRigElem(new IK_Chain(target, pole, type), useLocal);
    }
}
