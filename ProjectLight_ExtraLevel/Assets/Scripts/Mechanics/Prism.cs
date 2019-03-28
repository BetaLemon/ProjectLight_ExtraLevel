using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour {

    public struct ColorRay { public GameObject gameObject; public Color color; };

    #region ColorDefinition
    public Color red;
    public Color orange;
    public Color yellow;
    public Color green;
    public Color blue;
    public Color violet;
    public Color pink;
    #endregion Colors are set through the editor using the saved color preset.

    private Dictionary<string, ColorRay> colorRays = new Dictionary<string, ColorRay>();
    public GameObject rayPrefab;

    private Vector3 incomingVec;
    private Vector3 hitPoint;
    private Vector3 normal;

    private bool processing;
    private float spreadAngle = 15f;
    private float rayMaxLength = 15f;
    private float rayRadius = 5f;

	// Use this for initialization
	void Start () {
        
        // Color Ray Init:
        colorRays.Add("Red", InitRay(red));
        colorRays.Add("Orange", InitRay(orange));
        colorRays.Add("Yellow", InitRay(yellow));
        colorRays.Add("Green", InitRay(green));
        colorRays.Add("Blue", InitRay(blue));
        colorRays.Add("Violet", InitRay(violet));
        colorRays.Add("Pink", InitRay(pink));

        processing = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (processing)
        {
            colorRays["Red"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Red"].gameObject.transform.position = hitPoint;
            colorRays["Red"].gameObject.transform.forward = incomingVec;
            colorRays["Red"].gameObject.transform.Rotate(0, -spreadAngle * 3, 0);
            colorRays["Red"].gameObject.GetComponent<LightRay>().SetActive(true);

            colorRays["Orange"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Orange"].gameObject.transform.position = hitPoint;
            colorRays["Orange"].gameObject.transform.forward = incomingVec;
            colorRays["Orange"].gameObject.transform.Rotate(0, -spreadAngle * 2, 0);
            colorRays["Orange"].gameObject.GetComponent<LightRay>().SetActive(true);

            colorRays["Yellow"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Yellow"].gameObject.transform.position = hitPoint;
            colorRays["Yellow"].gameObject.transform.forward = incomingVec;
            colorRays["Yellow"].gameObject.transform.Rotate(0, -spreadAngle * 1, 0);
            colorRays["Yellow"].gameObject.GetComponent<LightRay>().SetActive(true);

            colorRays["Green"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Green"].gameObject.transform.position = hitPoint;
            colorRays["Green"].gameObject.transform.forward = incomingVec;
            colorRays["Green"].gameObject.GetComponent<LightRay>().SetActive(true);
            //colorRays["Green"].gameObject.transform.Rotate(0, 0, 0);

            colorRays["Blue"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Blue"].gameObject.transform.position = hitPoint;
            colorRays["Blue"].gameObject.transform.forward = incomingVec;
            colorRays["Blue"].gameObject.transform.Rotate(0, spreadAngle * 1, 0);
            colorRays["Blue"].gameObject.GetComponent<LightRay>().SetActive(true);

            colorRays["Violet"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Violet"].gameObject.transform.position = hitPoint;
            colorRays["Violet"].gameObject.transform.forward = incomingVec;
            colorRays["Violet"].gameObject.transform.Rotate(0, spreadAngle * 2, 0);
            colorRays["Violet"].gameObject.GetComponent<LightRay>().SetActive(true);

            colorRays["Pink"].gameObject.GetComponent<LightRay>().SetRayScale(new Vector3(rayRadius, rayRadius, rayMaxLength));
            colorRays["Pink"].gameObject.transform.position = hitPoint;
            colorRays["Pink"].gameObject.transform.forward = incomingVec;
            colorRays["Pink"].gameObject.transform.Rotate(0, spreadAngle * 3, 0);
            colorRays["Pink"].gameObject.GetComponent<LightRay>().SetActive(true);

            processing = false;
        }
        else
        {
            colorRays["Red"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Orange"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Yellow"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Green"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Blue"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Violet"].gameObject.GetComponent<LightRay>().SetActive(false);
            colorRays["Pink"].gameObject.GetComponent<LightRay>().SetActive(false);

            incomingVec = Vector3.zero;
            hitPoint = Vector3.zero;
            normal = Vector3.zero;
        }
	}

    private ColorRay InitRay(Color color)
    {
        ColorRay tmp;
        tmp.gameObject = Instantiate(rayPrefab) as GameObject;
        tmp.gameObject.transform.parent = transform;
        tmp.gameObject.GetComponent<LightRay>().SetRayScale(Vector3.zero);
        tmp.gameObject.transform.position = transform.position;
        tmp.color = color;

        tmp.gameObject.GetComponent<LightRay>().color = tmp.color;
        return tmp;
    }

    public void Process(Vector3 inVec, Vector3 point, Vector3 norm)   // Parameters actually come from the Raycast.
    {
        // We update our vector to the values of the raycastHit:
        incomingVec = inVec;
        hitPoint = point;
        normal = norm;
        processing = true;
    }
}
