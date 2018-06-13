using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPlacer : MonoBehaviour {

    public LayerMask TerrainMask;

    public Transform prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0) && Application.isEditor)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000, TerrainMask))
            {
                Instantiate(prefab, hit.point, Quaternion.Euler(hit.normal));
            }
        }

        if(Input.touchCount > 0)
        {
            if(Input.GetTouch(0).tapCount == 2)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000, TerrainMask))
                {
                    Instantiate(prefab, hit.point, Quaternion.Euler(hit.normal));
                }
            }
        }
	}
}
