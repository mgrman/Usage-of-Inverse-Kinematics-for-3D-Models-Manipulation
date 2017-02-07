using UnityEngine;
using System.Collections;

public class EnableDisableMesh : MonoBehaviour {
	
	public KeyCode Key;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		var spacebar = Input.GetKeyDown(Key);
		if(spacebar)
		{
			GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
		}
	}
}
