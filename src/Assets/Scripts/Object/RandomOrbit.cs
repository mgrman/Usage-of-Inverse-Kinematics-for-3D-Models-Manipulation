using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomOrbit : MonoBehaviour {
	public int Speed;
	public Transform Target;

    public float _Distance = 2;
	
	private float _alfa = 0.0f;
    private float _beta = 0.0f;


	
	// Use this for initialization
	void Start () 
	{
	    var angles = transform.eulerAngles;
	    _alfa = angles.y;
	    _beta = angles.x;
		// Make the rigid body not change rotation
	   	if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
        float changeAlfa = Mathf.PerlinNoise(Time.time, 0) - 0.5f;
        float changeBeta = Mathf.PerlinNoise(0, Time.time) - 0.5f;


        changeAlfa *= Speed;
        changeBeta *= Speed;

        changeAlfa *= Time.deltaTime;
        changeBeta *= Time.deltaTime;



        _alfa += changeAlfa;
        _beta += changeBeta;


        _alfa = Mathf.Clamp(_alfa, 0, 180);


        var rotation = Quaternion.Euler(_alfa, _beta, 0f);
        var position = rotation * (new Vector3(0.0f, 0.0f, -_Distance)) + Target.position;
        transform.rotation = rotation;
        transform.position = position;
		
	}
	
	
}
