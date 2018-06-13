using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    private Text text;

    private int frame_count = 0;
    private double dt = 0.0f;
    private double fps = 0.0;
    private double update_rate = 1.0;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();

        text.text = "60.00 fps";
	}
	
	// Update is called once per frame
	void Update () {
        frame_count++;
        dt += Time.deltaTime;
        if (dt > 1.0 / update_rate)
        {
            fps = frame_count / dt;
            frame_count = 0;
            dt -= 1.0 / update_rate;

            text.text = fps.ToString("N2") + " fps";
        }
	}
}
