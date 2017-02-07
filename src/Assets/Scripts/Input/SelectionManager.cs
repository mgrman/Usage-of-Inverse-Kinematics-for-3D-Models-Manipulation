using UnityEngine;
using System.Collections;

public class SelectionManager : MonoBehaviour 
{

    private GameObject _selectedObject;
    public GameObject SelectedObject
    {
        get
        {
            return _selectedObject;
        }
        set
        {
            _selectedObject = value;
            SelectionChanged = true;
            _selectionJustChanged = true;
        }
    }

    private bool _selectionJustChanged;

    public bool SelectionChanged { get; set; }
    void Awake()
    {
        if (MonoBehaviour.FindObjectsOfType<SelectionManager>().Length != 1)
            throw new UnityException("The are multiple Selection Managers");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !SelectionChanged)
            SelectedObject = null;

        SelectionChanged = _selectionJustChanged;
        _selectionJustChanged = false;
    }


//#if UNITY_EDITOR
//    void OnGUI()
//    {
//        var style = new GUIStyle();
//        style.alignment = TextAnchor.UpperLeft;

//        GUILayout.Label("Selected object:", style);
//        GUILayout.Label(SelectedObject.ToString(), style);
//    }
//#endif
}
