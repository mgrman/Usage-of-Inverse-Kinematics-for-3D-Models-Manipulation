using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MeshVariantSelector : MonoBehaviour {


    public List< GameObject> Variants;
    public int Selector;

    private int _oldSelector;

	// Use this for initialization
	void Start () {
        _oldSelector = -1;
	}
	
	// Update is called once per frame
	void Update () {


        if (_oldSelector != Selector)
        {
            for (int i = 0; i < Variants.Count; i++)
            {
                var v = Variants[i];

                RecursiveRendererEnabled(v, i == Selector);
            }

            _oldSelector=Selector;
        }	    
	}

    private void RecursiveRendererEnabled(GameObject root, bool enabled)
    {
        if (root == null)
            return;

        if (root.GetComponent<Renderer>()!=null)
            root.GetComponent<Renderer>().enabled = enabled;

        foreach (Transform child in root.transform)
        {
            RecursiveRendererEnabled(child.gameObject, enabled);

        }
    }
}