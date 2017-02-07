using UnityEngine;
using System.Collections;

public class RelativeControlPSR : MonoBehaviour {

    public PSR AttributeToControl;

    public Vector3 StartValue;

    public Vector3 EndValue;

    public float Parameter;

    public Transform Target;

    public bool ClipValues;

    public bool AbsoluteTransformation;

    private Vector3 _startLocalPos;

    private Vector3 _startPos;

    private Vector3 _startLocalRot;

    private Vector3 _startRot;

    private Vector3 _startScale;


	// Use this for initialization
	void Start () {
        _startLocalPos=Target.localPosition;

        _startPos=Target.position;

        _startLocalRot=Target.localRotation.eulerAngles;

        _startRot=Target.rotation.eulerAngles;

        _startScale = Target.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (Target == null)
            return;

	    Vector3 value=StartValue*(1-Parameter)+EndValue*Parameter;

        if (ClipValues)
        {
            if (Parameter < 0)
                value = StartValue;

            if (Parameter > 1)
                value = EndValue;
        }



        switch (AttributeToControl)
        {
            case PSR.LocalPosition:
                if (AbsoluteTransformation)
                    value += _startLocalPos;

                Target.localPosition = value;
                break;

            case PSR.Position:
                if (AbsoluteTransformation)
                    value += _startPos;

                Target.position = value;
                break;

            case PSR.LocalRotation:
                if (AbsoluteTransformation)
                    value += _startLocalRot;

                Target.localRotation = Quaternion.Euler(value);
                break;

            case PSR.Rotation:
                if (AbsoluteTransformation)
                    value += _startRot;

                Target.rotation = Quaternion.Euler(  value);
                break;

            case PSR.Scale:
                if (AbsoluteTransformation)
                    value += _startScale
                        ;
                Target.localScale = value;
                break;
        }
	}
}

public enum PSR { LocalPosition, Position, Scale, LocalRotation, Rotation }
