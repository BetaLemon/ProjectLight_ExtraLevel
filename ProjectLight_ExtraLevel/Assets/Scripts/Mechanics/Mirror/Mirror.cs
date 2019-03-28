using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    // Mirror is a script that enables light reflection for mirrors and other objects. To differentiate the
    // light source from others, it is internally called Kamehameha. It owns such a Kamehameha and points it
    // in the reflected angles direction. Mirrors can reflect with each other.

    public GameObject LightRayGeometry;   // Stores the cylinder that represents the player's light ray. Internally called Kamehameha.

    //-------- COLOR RESTRICTIONS (6) ---------
    //Red: Color.red
    //Yellow: Color.red + Color.green
    //Green: Color.green
    //Blue: Color.blue
    //Purple: Color.red + Color.blue
    //Pink: Color.red + Color.white
    //-----------------------------------------

    private Color color;    //The color the mirror will reflect. Depends on what it receives
    private bool reflecting;    // Controls whether it needs to make calculations and show the Kamehameha.
    // Vectors that store: the incoming light, the normal vector of the mirror, the position at which the light enters and leaves, the direction at which it leaves:
    private Vector3 incomingVec, normalVec, hitPoint, reflectVec;

    public LayerMask raycastLayer;
    
    private RaycastHit rayHit;               // Saves the hit when the raycast intersects with a collider.
    private bool hitOtherMirror = false;     // Stores specifically if a mirror has been hit.
    
    // Function that is called when a raycast has hit our mirror and a reflection is expected:
    public void Reflect(Vector3 inVec, Vector3 normal, Vector3 point, Color inColor)   // Parameters actually come from the Raycast.
    {
        // We update our vector to the values of the raycastHit:
        incomingVec = inVec;
        normalVec = normal;
        hitPoint = point;
        color = inColor;
        reflecting = true;  // We are now reflecting!
    }
	
	// Update is called once per frame
	void Update () {
        if (reflecting)
        {
            LightRayGeometry.GetComponentInParent<LightRay>().color = color;      // Assigns the color to the reflecting LightRay
            LightRayGeometry.transform.position = hitPoint;                          // We set the Kamehameha's position to where the light hit.
            reflectVec= Vector3.Reflect(incomingVec, normalVec);               // We calculate the reflection vector, using our incoming and normal vectors.
            LightRayGeometry.transform.forward = reflectVec;                         // We make the Kamehameha look in the direction of the reflected vector.
            reflecting = false;                                                // After this execution we won't be reflecting anymore.

            float amount = FindObjectOfType<PlayerLight>().healthDrainAmmount;

            //Debug.DrawRay(hitPoint, reflectVec * 1000, Color.blue);            // For debugging reasons, we display the ray.
            if (Physics.Raycast(hitPoint, reflectVec, out rayHit))      // If our casted ray hits something:
            {   
                if (rayHit.collider.gameObject.CompareTag("Mirror")) { OtherMirror(rayHit); hitOtherMirror = true; }    // If we have hit a Mirror -> OtherMirror(). Hit mirror!
                if (rayHit.collider.gameObject.CompareTag("Filter")) { Filter(rayHit); } //Process light ray
                if (rayHit.collider.gameObject.CompareTag("Trigger")) { TriggerTrigger(rayHit); }   // If we hit a Trigger, then we trigger it -> TriggerTrigger().
                if (rayHit.collider.gameObject.CompareTag("LightOrb")) { rayHit.collider.GetComponentInParent<LightOrb>().ChargeOrb(color,amount); } //Charge the light orb
                if (rayHit.collider.gameObject.CompareTag("BlackInsect")) { BlackInsect(rayHit.collider); }

                Debug.DrawRay(hitPoint, reflectVec.normalized * rayHit.distance, Color.green);
                Debug.Log(rayHit.distance);
                LightRayGeometry.transform.localScale = new Vector3(8, 8, rayHit.distance/2);      // The length is the distance between the point of entering light
                                                                                                                        // and where the raycast hits on the other object.
            }
            else   // If our ray didn't hit shit...
            {
                // ... then, well, nothing was hit:
                LightRayGeometry.transform.localScale = new Vector3(8, 8, 15);  // Set to max length.
                hitOtherMirror = false;
            }
        }
        else    // If we're not reflecting:
        {
            LightRayGeometry.transform.localScale = new Vector3(0, 0, 0); // We make the Kamehameha suuuuuuper tiny.
        }
        //transform.Rotate(new Vector3(0,1,0));
	}

    // Function that is called when another mirror is hit:
    void OtherMirror(RaycastHit mirrorHit)
    {
        Vector3 inVec = mirrorHit.point - hitPoint; // The incoming vector for the receiving mirror is the point where we were hit minus the point where it was hit.
        mirrorHit.collider.GetComponentInParent<Mirror>().Reflect(inVec, mirrorHit.normal, mirrorHit.point, color);    // We tell that mirror to reflect.
        //LightRayGeometry.transform.localScale = new Vector3(8, 8, Vector3.Distance(mirrorHit.point, LightRayGeometry.transform.position));    // We make Kamehameha the length of the distance.
    }
    // Function that is called when a trigger is hit:
    void TriggerTrigger(RaycastHit rh)
    {
        rh.collider.gameObject.GetComponentInParent<Trigger>().pleaseTrigger(); // Tell the trigger to please trigger. Thanks.
    }

    void BlackInsect(Collider col)
    {
        //col.gameObject.GetComponent<BlackInsect>().Hurt();
    }

    void Filter(RaycastHit filterHit)
    {
        Vector3 inVec = filterHit.point - LightRayGeometry.transform.position;
        filterHit.collider.GetComponentInParent<RayFilter>().Process(inVec, filterHit.point);
        //LightRayGeometry.transform.localScale = new Vector3(8, 8, Vector3.Distance(filterHit.point, LightRayGeometry.transform.position)); // Limit the light ray's length to the object
    }
}