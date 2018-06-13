using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraController : MonoBehaviour
{
    public float PanSpeed = 20f;
    public float PlaneHeight = 0;

    private Plane plane;
    private Camera cam;

    private bool zoomActive, panActive;
    private int panFingerId;

    private Vector2 lastPanPosition;

    public void Start()
    {
        plane = new Plane(Vector3.up, 0);
        cam = GetComponent<Camera>();
    }

    public void Update()
    {
        HandleTouch();
        HandleMouse();
    }

    private void HandleTouch()
    {
        switch (Input.touchCount)
        {
            case 1: // Panning
                zoomActive = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                    panActive = true;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming
                panActive = false;
                
                break;

            default:
                panActive = false;
                zoomActive = false;
                break;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            panActive = true;
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            panActive = false;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        /*
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomActive = true;
        ZoomCamera(scroll, ZoomSpeedMouse);
        zoomActive = false;
        */
    }

    private void PanCamera(Vector2 newPointerPosition)
    {
        //if (!panActive)
        //    return;

        /*Vector3 first_position = getOnGridWorldPos(lastPanPosition.x, lastPanPosition.y);
        Vector3 current_position = getOnGridWorldPos(newPointerPosition.x, newPointerPosition.y);

        Vector3 diff = first_position - current_position;
        //diff.x *= -1;
        diff.y = 0;
        diff.z *= -1;*/
        Vector2 mouseDiff = lastPanPosition - newPointerPosition;
        Vector3 diff = new Vector3(mouseDiff.x * cam.transform.forward.x * -1, 0, mouseDiff.y * cam.transform.forward.z);
        Debug.Log(cam.transform.forward);
        cam.transform.Translate(diff, Space.World);
        
        lastPanPosition = newPointerPosition;
    }

    public Vector3 getOnGridWorldPos(float screenX, float screenY)
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(screenX, -screenY + cam.pixelHeight, cam.nearClipPlane));

        //set plane distance from origin to current grid y position
        plane.distance = -PlaneHeight;
        plane.normal = cam.transform.forward;
        

        //calc mouse pos in world cords
        float dst = 0f;
        if (plane.Raycast(ray, out dst))
        {
            Vector3 pos = ray.GetPoint(dst);
            return pos;
        }
        return default(Vector3);
    }

    /*public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = .5f;

    //public static readonly float[] BoundsX = new float[] { -10f, 5f };
    //public static readonly float[] BoundsZ = new float[] { -18f, -4f };
    public static readonly float[] ZoomBounds = new float[] { 10f, 85f };

    private Camera cam;

    private bool panActive;
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool zoomActive;
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake()
    {
        cam = GetComponent<Camera>();

#if UNITY_ANDROID || UNITY_IOS
        cam.fieldOfView = 60f;
#endif
    }

    void Update()
    {
        
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch()
    {
        switch (Input.touchCount)
        {

            case 1: // Panning
                zoomActive = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                    panActive = true;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming
                panActive = false;

                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!zoomActive)
                {
                    lastZoomPositions = newPositions;
                    zoomActive = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                panActive = false;
                zoomActive = false;
                break;
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // On mouse up, disable panning.
        // If there is no mouse being pressed, do nothing.
        if (Input.GetMouseButtonDown(0))
        {
            panActive = true;
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            panActive = false;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomActive = true;
        ZoomCamera(scroll, ZoomSpeedMouse);
        zoomActive = false;
    }

    void PanCamera(Vector3 newPanPosition)
    {
        if (!panActive)
        {
            return;
        }

        // Translate the camera position based on the new input position
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);
        transform.Translate(move, Space.World);
        ClampToBounds();

        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (!zoomActive || offset == 0)
        {
            return;
        }

        //cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
        cam.transform.Translate(transform.forward * offset * speed * Time.deltaTime, Space.Self);
    }

    void ClampToBounds()
    {
        Vector3 pos = transform.position;
        //pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        //pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);

        transform.position = pos;
    }*/

}

