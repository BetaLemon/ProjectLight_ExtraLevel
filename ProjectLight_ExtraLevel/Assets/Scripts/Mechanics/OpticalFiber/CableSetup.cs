using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSetup : MonoBehaviour {

    public GameObject start;
    public GameObject end;

    public float charge;
    private float maxCharge = 100;
    public Transform cylinder;

    // Use this for initialization
    public void Setup () {
        // Calculate middle point between nodes for positioning:
        Vector3 pos =  0.5f * (start.transform.position + end.transform.position);
        transform.position = pos;

        // Set size to the distance between nodes:
        float size = Vector3.Distance(start.transform.position, transform.position);
        transform.localScale = new Vector3(transform.localScale.x, size, transform.localScale.z);

        // Rotate:
        transform.LookAt(end.transform.position);
        transform.Rotate(new Vector3(90, 0, 0));
    }

    void Update()
    {
        if (charge > maxCharge) { charge = maxCharge; }
        if (charge < 0) { charge = 0; }

        cylinder.localScale = new Vector3(charge / 10, 1, charge/10);
    }
}
