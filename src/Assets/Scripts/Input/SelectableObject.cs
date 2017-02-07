using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour
{


    SelectionManager _selMgr;

	void Awake () 
	{
        _selMgr = MonoBehaviour.FindObjectOfType<SelectionManager>();
	}

    void OnMouseDown()
    {
        _selMgr.SelectedObject = this.gameObject;
    }

}
