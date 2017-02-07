using UnityEngine;
using System.Collections;

public class MouseToPosition : MonoBehaviour {
	
	private float _oldMouseX;
	private float _oldMouseY;

    private Transform _me;
	// Use this for initialization
	void Start () 
	{
		_me=GameObject.Find(this.name).transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
        
		float mouseX=Input.mousePosition.x;//Input.GetAxis("Mouse X");
		float mouseY=Input.mousePosition.y;//Input.GetAxis("Mouse Y");
	
		float deltaX=(mouseX-_oldMouseX)/100;
		float deltaY=(mouseY-_oldMouseY)/100;
		
		var pos=_me.position;
		
		pos.z+=deltaX;
		pos.y+=deltaY;
		_me.position=pos;
		
		
		_oldMouseX=mouseX;
		_oldMouseY=mouseY;
	}
}
