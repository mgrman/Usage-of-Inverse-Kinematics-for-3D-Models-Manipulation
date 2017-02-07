using UnityEngine;
using System.Collections;

public class MoveSelectedObject : MonoBehaviour {
	private GameObject _selectedObject;
	public float Delta=0.005f;

    SelectionManager _selMgr;

    void Awake()
    {
        _selMgr = MonoBehaviour.FindObjectOfType<SelectionManager>();
    }
	
	// Update is called once per frame
	void Update () 
	{
        _selectedObject = _selMgr.SelectedObject;
		if(_selectedObject==null)
			return;

        Vector3 translate = new Vector3();
        Vector3 rotate = new Vector3();

		if(Input.GetKey(KeyCode.W))
			translate.z+=Delta;
		if(Input.GetKey(KeyCode.S))
			translate.z-=Delta;
		if(Input.GetKey(KeyCode.D))
			translate.x+=Delta;
		if(Input.GetKey(KeyCode.A))
			translate.x-=Delta;
		if(Input.GetKey(KeyCode.Q))
			translate.y+=Delta;
		if(Input.GetKey(KeyCode.E))
			translate.y-=Delta;

        if (Input.GetKey(KeyCode.R))
            rotate.x += Delta*100;
        if (Input.GetKey(KeyCode.T))
            rotate.x -= Delta * 100;
        if (Input.GetKey(KeyCode.F))
            rotate.y += Delta * 100;
        if (Input.GetKey(KeyCode.G))
            rotate.y -= Delta * 100;
        if (Input.GetKey(KeyCode.V))
            rotate.z += Delta * 100;
        if (Input.GetKey(KeyCode.B))
            rotate.z -= Delta * 100;


        _selectedObject.transform.Translate(translate);
        _selectedObject.transform.Rotate(rotate);
	}
}
