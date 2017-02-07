using UnityEngine;
using System.Collections;

public class DetectHits : MonoBehaviour
{ 
    

    int _score;


    void Start()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {

        if (other.tag == "Player")
        {
            //print("hit:" + other.ToString());
            _score++;
            
        }

    }

    public void Reset()
    {
        _score = 0;
    }


    void OnGUI()
    {


        float height = Screen.height / 20;

        float width = Screen.width / 5;


        var rect=new Rect(Screen.width - width, 0, width, height);
        
        GUI.Box(rect, string.Format("Score : {0}", _score));
    }
}
