using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpticalFiber_Node : MonoBehaviour {

    public float charge = 0;
    public float prevCharge = 0;
    private float maxCharge = 100;
    public Transform chargeLight;

    public bool receivingLight;
    private float stoppedReceivingDelay = 1.0f; // seconds!
    private float currentDelay = 0;

    public Light light;

	// Use this for initialization
	void Start () {
        Transform[] children = GetComponentsInChildren<Transform>();
        for(int i = 0; i < children.Length; i++)
        {
            if(children[i].name[0] != 'n' && children[i].name[0] != 'N')
            {
                chargeLight = children[i];
                break;
            }
        }

        light = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (charge > maxCharge) { charge = maxCharge; }
        if(charge < 0) { charge = 0; }

        chargeLight.localScale = (new Vector3(charge, charge, charge) / 10);
        if(light != null) { light.range = charge; }

        if ((charge == prevCharge) && receivingLight) { currentDelay += Time.deltaTime; }
        else { currentDelay = 0; }
        if(currentDelay > stoppedReceivingDelay) { receivingLight = false; }
        prevCharge = charge;

        if(light != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, light.range - 5); //(Sphere center, Radius)
            foreach(Collider col in hitColliders)
            {
                if (!col.isTrigger) { continue; }
                if (col.CompareTag("OpticalFiber")) { continue; }
                if (col.CompareTag("LightOrb")) { if(charge != 0) col.GetComponent<LightOrb>().ChargeOrb(Color.white, charge/10); }
            }
        }
	}

    public void AddCharge(float amount)
    {
        receivingLight = true;
        charge += amount;
    }

    public bool isReceivingLight() { return receivingLight; }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("OpticalFiber")) return;
        if (other.CompareTag("Player"))
        {
            if (PlayerInput.instance.isPressed("BaseInteraction")) GetComponentInParent<OpticalFiber>().StartPlayerMode(other.transform);
        }
    }
}
