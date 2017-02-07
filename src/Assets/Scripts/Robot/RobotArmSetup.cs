using UnityEngine;
using System.Collections;
using System.Linq;

//[ExecuteInEditMode]
public class RobotArmSetup : MonoBehaviour {

    public Transform _Target;

    public float _Distance;

    ParticleSystem _partSystem;
    RobotIKControler _ikCtrl;

	// Use this for initialization
	void Awake () 
    {
        _partSystem = transform.GetComponentsInChildren<ParticleSystem>().Where(o => o.gameObject.name == "RobotArmParticleSystem").FirstOrDefault();
        if (_partSystem == null)
            throw new UnityException("Problem finding particleSystem");

        _ikCtrl = transform.GetComponentInChildren<RobotIKControler>();
        if (_ikCtrl == null)
            throw new UnityException("Problem finding RobotIKControler");
	}
	
	// Update is called once per frame
	void Update () 
    {
        _ikCtrl._Target = _Target;
        _partSystem.startSpeed = _Distance * 0.2f * _partSystem.startLifetime;
	}
}
