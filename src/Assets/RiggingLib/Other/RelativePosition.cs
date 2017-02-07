using UnityEngine;
using System.Collections;

public class RelativePosition : MonoBehaviour {

	// Use this for initialization
	public Transform A;
	public Transform B;
	public float Percent;
    public Transform Target;
	void Start () 
	{
	
	}

	// Update is called once per frame
	void Update () 
	{
        if (Target)
            Target.position = A.position + (B.position - A.position) * Percent;
        else
            transform.position = A.position + (B.position - A.position) * Percent;
	}
}
