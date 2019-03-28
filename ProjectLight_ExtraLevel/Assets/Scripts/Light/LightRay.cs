using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour {

    public GameObject LightRayGeometry;
    public Color color = Color.white; //Color of the light ray, is white by default
    public bool Interactive = false;
    public bool active = false;
    public float cylinderRadius = 2f;

    public LayerMask raycastLayer;

    private RaycastHit rayHit;
    private Color prevColor;

    float amount;
    float length;
    float maxLength = 15f;
    MeshRenderer mr;

    void Start()
    {
        amount = 0.05f; //GetComponent<PlayerLight>().healthDrainAmmount;
        mr = LightRayGeometry.GetComponent<MeshRenderer>();
        ChangeColor();
    }

    void Update () {
        //renderer.material.SetFloat("_Blend", someFloatValue);
        if (prevColor != color) ChangeColor();

        if (!Interactive) return;
        if (!active)
        {
            LightRayGeometry.transform.localScale = Vector3.zero;
            return;
        }

        // INTERACTION:
        Vector3 rayOrigin = LightRayGeometry.transform.position;

        Debug.DrawRay(LightRayGeometry.transform.position, LightRayGeometry.transform.forward * LightRayGeometry.transform.localScale.z * 2, Color.red);
        if (transform.parent.CompareTag("Prism"))
        {
            //RaycastHit tmp;
            //if (Physics.Raycast(LightRayGeometry.transform.position, LightRayGeometry.transform.forward, out tmp, 4))
            //{
            //    LightRayGeometry.transform.position = tmp.point;
            //}
            //Debug.DrawRay(LightRayGeometry.transform.position, LightRayGeometry.transform.forward * Vector3.Distance(LightRayGeometry.transform.position, tmp.point), Color.cyan);
            rayOrigin = LightRayGeometry.transform.position + LightRayGeometry.transform.forward * 1;
        }

        if (Physics.Raycast(rayOrigin, LightRayGeometry.transform.forward, out rayHit, LightRayGeometry.transform.localScale.z * 2))  //(vec3 Origin, vec3direction, vec3 output on intersection) If Raycast hits a collider.
        {
            // Specific game object interactions with light cylinder:
            if (rayHit.collider.gameObject.CompareTag("Mirror")) { Mirror(rayHit); } //Reflect mirror light
            if (rayHit.collider.gameObject.CompareTag("Filter")) { Filter(rayHit); } //Process light ray
            if (rayHit.collider.gameObject.CompareTag("LightOrb")) { rayHit.collider.GetComponentInParent<LightOrb>().ChargeOrb(color, amount); } //Charge the light orb (Default white from player white ray)
            if (rayHit.collider.gameObject.CompareTag("Trigger")) { TriggerTrigger(rayHit); }
            if (rayHit.collider.gameObject.CompareTag("BlackInsect")) { BlackInsect(rayHit.collider); }
            if (rayHit.collider.gameObject.CompareTag("Prism")) { Prism(rayHit); }

            length = rayHit.distance;
            
            /*
            float distCylPosHitPos = Vector3.Distance(rayHit.point, LightRayGeometry.transform.position);
            if (distCylPosHitPos / 2 > maxLength)
            {
                LightRayGeometry.transform.localScale = new Vector3(8, 8, maxLength);
            }
            else
            {
                LightRayGeometry.transform.localScale = new Vector3(8, 8, distCylPosHitPos / 2);
            }*/
        }
        else length = maxLength;
        if (length > maxLength) { length = maxLength; }
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, length);
	}

    void ChangeColor()
    {
        mr.materials[0].SetColor("_EmissionColor", color*1.9f);
        mr.materials[0].SetColor("_Color", color);
        prevColor = color;
    }

    public void SetColor(Color col)
    {
        color = col;
        ChangeColor();
    }

    public void SetRayScale(Vector3 newScale)
    {
        LightRayGeometry.transform.localScale = newScale;
    }

    public void SetActive(bool isIt) { active = isIt; }

    #region InteractionFunctions
    void BlackInsect(Collider col)
    {
        //col.gameObject.GetComponent<BlackInsect>().Hurt();
    }

    void Mirror(RaycastHit mirrorHit)
    {
        Vector3 inVec = mirrorHit.point - LightRayGeometry.transform.position;
        mirrorHit.collider.GetComponentInParent<Mirror>().Reflect(inVec, mirrorHit.normal, mirrorHit.point, color);
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(mirrorHit.point, LightRayGeometry.transform.position)); // Limit the light ray's length to the object
    }

    void Filter(RaycastHit filterHit)
    {
        Vector3 inVec = filterHit.point - LightRayGeometry.transform.position;
        filterHit.collider.GetComponentInParent<RayFilter>().Process(inVec, filterHit.point);
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(filterHit.point, LightRayGeometry.transform.position)); // Limit the light ray's length to the object
    }

    void TriggerTrigger(RaycastHit rh)
    {
        rh.collider.gameObject.GetComponentInParent<Trigger>().pleaseTrigger();
    }

    void Prism(RaycastHit rh)
    {
        if (rh.collider.gameObject.CompareTag("Prism")) { length = Vector3.Distance(rh.point, LightRayGeometry.transform.position); return; }
        //if(rh.collider.gameObject.transform.parent == transform.parent) { length = Vector3.Distance(rh.point, LightRayGeometry.transform.position); return;}
        Vector3 inVec = rh.point - LightRayGeometry.transform.position;
        rh.collider.GetComponentInParent<Prism>().Process(inVec, rh.point, rh.normal);
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(rh.point, LightRayGeometry.transform.position));
    }
    #endregion
}
