using UnityEngine;
using System.Collections;

public class BindPosition : MonoBehaviour {
	
	public Transform Target;
	// Use this for initialization
	void Start () {
	
		transform.position=Target.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position=Target.position;
	}
}
