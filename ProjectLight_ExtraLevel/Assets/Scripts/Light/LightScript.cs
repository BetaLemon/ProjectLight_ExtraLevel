using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour {

    public float life;
    private float hue;
    private Light light;

	// Use this for initialization
	void Start () {
        life = 100.0f;
        light = GetComponent<Light>();
        //print(light.intensity);
        hue = light.intensity / life;
        //print(hue);
	}
	
	// Update is called once per frame
	void Update () {
        light.intensity = hue * life;
	}
}
