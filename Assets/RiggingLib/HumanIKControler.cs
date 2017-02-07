using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Animator))]
//[ExecuteInEditMode]
public class HumanIKControler : MonoBehaviour, IRigControler
{

    protected Animator avatar;
    public Transform CogObj = null;
    public Transform SpineObj = null;
    public Transform ChestObj = null;
    public Transform LookAtObj = null;
    public Transform LeftFootObj = null;
    public Transform LeftKneeObj = null;
    public Transform RightFootObj = null;
    public Transform RightKneeObj = null;
    public Transform LeftHandObj = null;
    public Transform LeftElbowObj = null;
    public Transform RightHandObj = null;
    public Transform RightElbowObj = null;

    private List<IRigAplicator> _ikControlers = new List<IRigAplicator>();


    #region Convenience properties
    private Transform _head;
    private Transform _hips;
    private Transform _chest;
    private Transform _jaw;
    private Transform _lastBone;
    private Transform _leftEye;
    private Transform _leftFoot;
    private Transform _leftHand;
    private Transform _leftLowerArm;
    private Transform _leftLowerLeg;
    private Transform _leftShoulder;
    private Transform _leftToes;
    private Transform _leftUpperArm;
    private Transform _leftUpperLeg;
    private Transform _neck;
    private Transform _rightEye;
    private Transform _rightFoot;
    private Transform _rightHand;
    private Transform _rightLowerArm;
    private Transform _rightLowerLeg;
    private Transform _rightShoulder;
    private Transform _rightToes;
    private Transform _rightUpperArm;
    private Transform _rightUpperLeg;
    private Transform _spine;
    #endregion

    public bool _JointsInZAxisPositiveDirection;



    void Start()
    {
        avatar = GetComponent<Animator>();

        #region Convenience properties
        _head = avatar.GetBoneTransform(HumanBodyBones.Head);
        _hips = avatar.GetBoneTransform(HumanBodyBones.Hips);
        _chest = avatar.GetBoneTransform(HumanBodyBones.Chest);
        _jaw = avatar.GetBoneTransform(HumanBodyBones.Jaw);
        _lastBone = avatar.GetBoneTransform(HumanBodyBones.LastBone);
        _leftEye = avatar.GetBoneTransform(HumanBodyBones.LeftEye);
        _leftFoot = avatar.GetBoneTransform(HumanBodyBones.LeftFoot);
        _leftHand = avatar.GetBoneTransform(HumanBodyBones.LeftHand);
        _leftLowerArm = avatar.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        _leftLowerLeg = avatar.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        _leftShoulder = avatar.GetBoneTransform(HumanBodyBones.LeftShoulder);
        _leftToes = avatar.GetBoneTransform(HumanBodyBones.LeftToes);
        _leftUpperArm = avatar.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        _leftUpperLeg = avatar.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        _neck = avatar.GetBoneTransform(HumanBodyBones.Neck);
        _rightEye = avatar.GetBoneTransform(HumanBodyBones.RightEye);
        _rightFoot = avatar.GetBoneTransform(HumanBodyBones.RightFoot);
        _rightHand = avatar.GetBoneTransform(HumanBodyBones.RightHand);
        _rightLowerArm = avatar.GetBoneTransform(HumanBodyBones.RightLowerArm);
        _rightLowerLeg = avatar.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        _rightShoulder = avatar.GetBoneTransform(HumanBodyBones.RightShoulder);
        _rightToes = avatar.GetBoneTransform(HumanBodyBones.RightToes);
        _rightUpperArm = avatar.GetBoneTransform(HumanBodyBones.RightUpperArm);
        _rightUpperLeg = avatar.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        _spine = avatar.GetBoneTransform(HumanBodyBones.Spine);

        #endregion

        var baseInvert = _JointsInZAxisPositiveDirection ? InvertedState.Nothing : InvertedState.InvertX;


        //cog
        this.AddPositionConstrain(_hips, CogObj);

        //spine
        if (_chest != null)
            this.AddIK_Chain(ChestObj, SpineObj, IK_Chain.ComputationTypes.Convex_Spline, false)
                .BindChain(_hips, _chest)
                .AddInvertRotation(baseInvert);
        else
            this.AddIK_Chain(ChestObj, SpineObj, IK_Chain.ComputationTypes.Convex_Spline, false)
                .BindChain(_hips, _spine)
                .AddInvertRotation(baseInvert);

        //legs
        this.AddIK_Chain(LeftFootObj, LeftKneeObj, IK_Chain.ComputationTypes.Convex_Spline, false)
            .BindChain(_leftUpperLeg, _leftFoot)
            .AddInvertRotation(baseInvert);
        this.AddIK_Chain(RightFootObj, RightKneeObj, IK_Chain.ComputationTypes.Convex_Spline, false)
            .BindChain(_rightUpperLeg, _rightFoot)
            .AddInvertRotation(baseInvert);

        //arms
        this.AddIK_Chain(LeftHandObj, LeftElbowObj, IK_Chain.ComputationTypes.Convex_Spline, false)
            .BindChain(_leftUpperArm, _leftHand)
            .AddInvertRotation(InvertedState.InvertZ | baseInvert);

        this.AddIK_Chain(RightHandObj, RightElbowObj, IK_Chain.ComputationTypes.Convex_Spline, false)
            .BindChain(_rightUpperArm, _rightHand)
            .AddInvertRotation(InvertedState.InvertZ | baseInvert);

        //neck
        this.AddLookAtWithPole(_neck, LookAtObj, SpineObj)
            .AddRotationOffsetModif(new Vector3(0, 0, 180))
            .AddRotationOffsetModif(new Vector3(-90, 0, 0))
            .AddRotationOffsetModif(new Vector3(-20, 0, 0))
            .AddWeightRotModif(0.4f)
            .AddClampModif(new Vector3(0, 0, -30), new Vector3(0, 0, 30));

        //head
        this.AddLookAtWithPole(_head, LookAtObj, SpineObj)
            .AddRotationOffsetModif(new Vector3(0, 0, 180))
            .AddRotationOffsetModif(new Vector3(-90, 0, 0))
            .AddRotationOffsetModif(new Vector3(-15, 0, 0))
            .AddWeightRotModif(0.6f)
            .AddClampModif(new Vector3(-20, 0, -30), new Vector3(40, 0, 30));

        //eyes
        if (_leftEye != null)
        {
            this.AddLookAtWithPole(_leftEye, LookAtObj, _neck)
                .AddRotationOffsetModif(new Vector3(180, 0, 0));
        }

        if (_rightEye != null)
        {
            this.AddLookAtWithPole(_rightEye, LookAtObj, _neck)
                .AddRotationOffsetModif(new Vector3(180, 0, 0));
        }

        var initAngle = _leftFoot.rotation.eulerAngles.x;

        this.AddLookAtWithPole(_leftFoot, LeftKneeObj, new Vector3(0, 1, 0), false)
            .AddInvertRotation(InvertedState.InvertY)
            .AddRotationSet(_leftFoot, new Vector3(initAngle, float.NaN, 0));

        this.AddLookAtWithPole(_rightFoot, RightKneeObj, new Vector3(0, 1, 0), false)
           .AddInvertRotation(InvertedState.InvertY)
             .AddRotationSet(_rightFoot, new Vector3(initAngle, float.NaN, 0));
    }

    void Update()
    {
        foreach (var con in _ikControlers)
        {
            //try
            //{
            con.Update();
            //}
            //catch (Exception ex)
            //{
            //    print(ex);
            //}
        }
    }


    public void AddApplicator(IRigAplicator item)
    {
        _ikControlers.Add(item);
    }



}


