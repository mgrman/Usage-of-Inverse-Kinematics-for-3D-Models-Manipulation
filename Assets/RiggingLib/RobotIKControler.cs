using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RobotIKControler : MonoBehaviour ,IRigControler
{
    public Transform _Target;
    public Transform _Pole;
        
    private List<IRigAplicator> _ikControlers = new List<IRigAplicator>();

    public Transform _LastJointTarget;

    public Transform _LastNode;

	void Start () 
    {
        _ikControlers.Clear();

        var lastJoint = _LastNode;

        if (_LastJointTarget != null)
        {

            this.AddIK_Chain(_Target, _Pole,  IK_Chain.ComputationTypes.Fast_Incremental, true)
                .BindChain(transform, lastJoint.parent);

            this.AddLookAtWithPole(lastJoint.parent, _LastJointTarget, _Pole, false)
                .AddFrameDelayModif(10);
        }
        else
        {

            this.AddIK_Chain(_Target, _Pole,  IK_Chain.ComputationTypes.Fast_Incremental, true)
                .BindChain(transform, lastJoint);
        }
	}
	
	void Update () 
    {
        if (!Application.isPlaying)
        {
            Start();
        }

        foreach (var con in _ikControlers)
        {
                con.Update();
        }
    }
	


    public void AddApplicator(IRigAplicator item)
    {
        _ikControlers.Add(item);
    }
}
