using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraOrbit : MonoBehaviour
{
    public Transform Target;
    public float _MinHeight;
    public float _MaxHeight;
    public float _MinDistance;
    public float _MaxDistance;
    public float _MinFoV;
    public float _MaxFoV;

    public float _HeightSpeed = 0.00003f;
    public float _DistanceSpeed = 0.0002f;
    public float _RotationSpeed=0.003f;


    private float _alfa = 0.0f;
    private float _beta = 0.0f;

    private float _distParam = 0f;
    private float _heightParam = 0f;

    private float _oldMouseX;
    private float _oldMouseY;
    
    // Use this for initialization
    void Start ()
    {
        var angles = transform.eulerAngles;
        _alfa = angles.y;
        _beta = angles.x;

        _distParam = ((transform.position - Target.position).magnitude - _MinDistance) / (_MaxDistance - _MinDistance);

        _heightParam = ((transform.position.y - Target.position.y) - _MinHeight) / (_MaxHeight - _MinHeight);

        Clamp();
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    
        UpdatePosition ();
        Input.simulateMouseWithTouches = true;
    }
    
    // Update is called once per frame
    void Update ()
    {    
        float mouseX = Input.mousePosition.x;//Input.GetAxis("Mouse X");
        float mouseY = Input.mousePosition.y;//Input.GetAxis("Mouse Y");
        //print ("X:"+mouseX.ToString()+" Y:"+mouseY.ToString());
    
        float deltaX = (mouseX - _oldMouseX);
        float deltaY = (mouseY - _oldMouseY);
        if (Target) {
            if (Input.GetMouseButton (0)) {
                _alfa += deltaX  * _RotationSpeed;
                _beta -= deltaY  * _RotationSpeed;
        
                Clamp ();
                UpdatePosition ();
            }    
            if (Input.GetMouseButton (1)) {
				_distParam -= deltaX  * _DistanceSpeed;

                Clamp ();
                UpdatePosition ();
                //camera.fieldOfView=_fov;
            }    
            if (Input.GetMouseButton (2)) {    
                _heightParam -= deltaY  *_HeightSpeed;
        
                Clamp ();
                UpdatePosition ();
            }
        }
    
        _oldMouseX = mouseX;
        _oldMouseY = mouseY;
    
    }
    
    private void UpdatePosition ()
    {
        float distance = _MinDistance + (_MaxDistance - _MinDistance) * _distParam;
        float fov = _MinFoV + (_MaxFoV - _MinFoV) * _distParam;
        float heightOffset = _MinHeight + (_MaxHeight - _MinHeight) * _heightParam;

        var basePos = Target.position + new Vector3(0f, heightOffset, 0f);
        var dof = GetComponent<DepthOfField>();
        if(dof!=null)
            dof.focalLength = distance;

        var rotation = Quaternion.Euler (_beta, _alfa, 0f);
        var position = rotation * (new Vector3(0.0f, 0.0f, -distance)) + basePos;
    
        position.y = Mathf.Max (position.y, 0.1f);
        transform.rotation = rotation;
        transform.position = position;

		
		GetComponent<Camera>().fieldOfView=fov;
    }
    
    private void Clamp ()
    {
        _beta = Mathf.Clamp(_beta, -90f, 90f);
        _distParam = Mathf.Clamp(_distParam, 0, 1);
        _heightParam = Mathf.Clamp(_heightParam, 0, 1);
    }
    

}

