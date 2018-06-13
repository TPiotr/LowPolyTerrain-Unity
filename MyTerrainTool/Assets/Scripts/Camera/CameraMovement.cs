using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private Vector3 camera_position = Vector3.zero;
    
    private Camera cam;

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        //android input
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                hit_position = Input.mousePosition;
                camera_position = transform.position;
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                current_position = Input.mousePosition;
                dragCamera();
            }
        }

        //pc input
        if (Input.GetMouseButtonDown(0) && Application.isEditor)
        {
            //save mouse & camera pos when touch began
            hit_position = Input.mousePosition;
            camera_position = transform.position;

        }
        if (Input.GetMouseButton(0) && Application.isEditor)
        {
            current_position = Input.mousePosition;
            dragCamera();
        }
    }

    void dragCamera()
    {
        Vector3 hit_pos = cam.ScreenToWorldPoint(hit_position);
        Vector3 current_pos = cam.ScreenToWorldPoint(current_position);
        //Debug.Log("Mouse pos: " + Input.mousePosition + " Hit: " + hit_pos + " Curr: " + current_pos);
        Vector3 direction = current_pos - hit_pos;
        direction = direction * -1;

        Vector3 position = camera_position + direction;
        
        transform.position = position;

    }
}
