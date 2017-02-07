using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomInCube : MonoBehaviour
{

	public int Speed;

    public Vector3 _Size;

    private Vector3? _centerPos;


	
	// Use this for initialization
	void Start () 
	{
        _centerPos = transform.position;
		// Make the rigid body not change rotation
	   	if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

        var pos= new Vector3();

       pos.x = Mathf.PerlinNoise(Time.time * Speed, 0) - 0.5f;
       pos.y = Mathf.PerlinNoise(0, Time.time * Speed) - 0.5f;
       pos.z = Mathf.PerlinNoise(-Time.time * Speed, -Time.time * Speed) - 0.5f;


        pos.Scale(_Size);

        transform.position = _centerPos.Value + pos;
		
	}


    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawCube(_centerPos ?? transform.position, _Size);
    }
	
}
