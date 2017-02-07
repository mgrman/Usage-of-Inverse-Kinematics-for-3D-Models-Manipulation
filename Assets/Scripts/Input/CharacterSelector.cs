using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CharacterSelector : MonoBehaviour {

    private Transform[] _characters;

    private Transform _selectedCharacter;


	void Start ()
    {
        _characters = Enumerable.Range(0, this.transform.childCount).Select(i => this.transform.GetChild(i)).ToArray();
        _selectedCharacter = _characters.FirstOrDefault();

        UpdateSelection();
	}
	



    void OnGUI()
    {
        Transform newSelection = null;
        float height = Screen.height / 20;

        float width = Screen.width / 20; ;

        foreach (var character in _characters)
        {
            var size=GUI.skin.button.CalcSize(new GUIContent(character.gameObject.name));
            width = Mathf.Max(width, size.x);
            height = Mathf.Max(height, size.y);
        }

        float offsetY = height * 0.5f;
        foreach (var character in _characters)
        {
            if (GUI.Button(new Rect(0, offsetY, width, height), character.gameObject.name))
            {
                newSelection = character;
            }


            offsetY += height*1.5f;

        }



        if (newSelection != null)
        {
            _selectedCharacter = newSelection;
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {

        foreach (var c in _characters)
            c.gameObject.SetActive(c == _selectedCharacter);

        foreach (var bind in MonoBehaviour.FindObjectsOfType<MonoBehaviour>().OfType<IBindToHumanRig>())
        {
            bind.HumanControler = _selectedCharacter.GetComponentInChildren<HumanIKControler>();
        }
    }
}

