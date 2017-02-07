using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[ExecuteInEditMode]
public class SetVisibility : MonoBehaviour {

    private float _oldVisibility;
    [Range(0,1)]
    public float _Visibility;

    SelectionManager _selManager;

    void Awake()
    {
        _selManager = FindObjectOfType<SelectionManager>();

    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!(_oldVisibility != _Visibility || (_selManager!=null && _selManager.SelectionChanged)))
            return;
        _oldVisibility = _Visibility;


        foreach (Transform child in transform)
        {
            float alpha=_Visibility;
            if (_selManager != null && _selManager.SelectedObject == child.gameObject)
            {
                alpha = 1;
            }

            var mesh=child.gameObject.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                foreach (var mat in mesh.sharedMaterials)
                {
                    var col = mat.color;
                    col.a = alpha;
                    mat.color = col;
                }
            }

            var particle = child.GetComponent<SingleParticleControler>();
            if (particle != null)
            {
                var col = particle.Color;
                col.a = alpha;
                particle.Color = col;

            }
        }
	}
}
