using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayFilter : MonoBehaviour
{
    // Filters process light rays and convert them to another colour

    public GameObject LightRayGeometry;   // Stores the cylinder that represents the player's light ray. Internally called LightRay.
    public LayerMask raycastLayer;

    //-------- COLOR RESTRICTIONS (6) ---------
    //Red: Color.red
    //Yellow: Color.red + Color.green
    //Green: Color.green
    //Blue: Color.blue
    //Purple: Color.red + Color.blue
    //Pink: Color.red + Color.white
    //-----------------------------------------

    public Color color;    //The color the filter will filter.
    private bool processing;
    public float filterWidth = 0.5f;
    public float cylinderRadius = 2.0f;

    // Vectors that store: the incoming light, the normal vector of the mirror, the position at which the light enters and leaves, the direction at which it leaves:
    private Vector3 incomingVec, hitPoint;

    private RaycastHit rayHit;    // Saves the hit when the raycast intersects with a collider.

    // Function that is called when a raycast has hit our filter and a colour change is expected:
    public void Process(Vector3 inVec, Vector3 point)   // Parameters actually come from the Raycast.
    {
        // We update our vector to the values of the raycastHit:
        incomingVec = inVec;
        hitPoint = point;
        processing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (processing)
        {
            hitPoint = hitPoint + (incomingVec.normalized) * filterWidth;

            //print("LightRay being processed");
            LightRayGeometry.GetComponentInParent<LightRay>().color = color;    // Assigns the color to the outputting LightRay
            LightRayGeometry.transform.position = hitPoint;                     // We set the LightRay's position to where the light hit.
            LightRayGeometry.transform.forward = incomingVec;                   // We make the LightRay look in the direction of the vector coming in vector.
            processing = false;                                                 // After this execution we won't be processing anymore.

            float amount = FindObjectOfType<PlayerLight>().healthDrainAmmount;

            Debug.DrawRay(hitPoint, incomingVec * 1000, Color.cyan);     // For debugging reasons, we display the ray.
            if (Physics.Raycast(hitPoint, incomingVec, out rayHit, Mathf.Infinity))      // If our casted ray hits something:
            {
                if (rayHit.collider.gameObject.CompareTag("Mirror")) { Mirror(rayHit); } // If we have hit a Mirror -> Mirror(). Hit mirror!
                if (rayHit.collider.gameObject.CompareTag("Filter")) { Filter(rayHit); } // Process light ray
                if (rayHit.collider.gameObject.CompareTag("Trigger")) { TriggerTrigger(rayHit); }   // If we hit a Trigger, then we trigger it -> TriggerTrigger().
                if (rayHit.collider.gameObject.CompareTag("LightOrb")) { rayHit.collider.GetComponentInParent<LightOrb>().ChargeOrb(color,amount); } //Charge the light orb
                if (rayHit.collider.gameObject.CompareTag("BlackInsect")) { BlackInsect(rayHit.collider); }

                //print(rayHit.collider.gameObject.name);
                LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(hitPoint, rayHit.point) / 2);    // The length is the distance between the point of entering light
                                                                                                                            // and where the raycast hits on the other object.
            }

            else   // If our ray didn't hit shit...
            {
                // ... then, well, nothing was hit:
                LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, 20);  // Set to max length.
            }
        }
        else    // If we're not reflecting:
        {
            LightRayGeometry.transform.localScale = new Vector3(0, 0, 0); // We make the LightRay suuuuuuper tiny.
        }
    }

    void BlackInsect(Collider col)
    {
        //col.gameObject.GetComponent<BlackInsect>().Hurt();
    }

    // Function that is called when a mirror is hit:
    void Mirror(RaycastHit mirrorHit)
    {
        print("Mirror was hit by filtered ray");
        Vector3 inVec = mirrorHit.point - hitPoint; // The incoming vector for the receiving mirror is the point where we were hit minus the point where it was hit.
        mirrorHit.collider.GetComponentInParent<Mirror>().Reflect(inVec, mirrorHit.normal, mirrorHit.point, color);    // We tell that mirror to reflect.
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(mirrorHit.point, LightRayGeometry.transform.position) / 2);    // We make LightRay the length of the distance.
    }
    // Function that is called when a trigger is hit:
    void TriggerTrigger(RaycastHit rh)
    {
        rh.collider.gameObject.GetComponentInParent<Trigger>().pleaseTrigger(); // Tell the trigger to please trigger. Thanks.
    }

    void Filter(RaycastHit filterHit)
    {
        Vector3 inVec = filterHit.point - LightRayGeometry.transform.position;
        filterHit.collider.GetComponentInParent<RayFilter>().Process(inVec, filterHit.point);
        LightRayGeometry.transform.localScale = new Vector3(cylinderRadius, cylinderRadius, Vector3.Distance(filterHit.point, LightRayGeometry.transform.position) / 2); // Limit the light ray's length to the object
    }
}
