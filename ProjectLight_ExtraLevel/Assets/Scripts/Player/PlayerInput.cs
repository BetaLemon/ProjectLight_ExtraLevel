using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    // The idea behind this script is that it should control all the player input, so that other scripts
    // can read the info processed here so they can do whatever they need, but input is isolated here.

    private Dictionary<string, float> input = new Dictionary<string, float>();

    //Double click dictionary
    private Dictionary<string, bool> doubleInput = new Dictionary<string, bool>();
    private float doubleClickDelay = 0.3f;
    //Timers for double clickable keys (Can be converted to dynamic vector if it gets lengthy):
    private float lastClickA;
    private float lastClickS;
    private float lastClickD;
    private float lastClickW;

    public static PlayerInput instance;

    private Vector2 mousePos;

    void Awake() { if (instance == null) instance = this; }

    void Start () {
        //Add INPUTS to input dictionary, current status of which we may be interested in:
        input.Add("Horizontal", 0);
        input.Add("Vertical", 0);
        input.Add("Run", 0);
        input.Add("Jump", 0);
        input.Add("BaseInteraction", 0);
        input.Add("LightMax", 0);
        input.Add("LightSwitch", 0);
        input.Add("Pause", 0);
        input.Add("Submit", 0);

        //Add DOUBLE CLICK INPUTS to doubleInput dictionary, current status of which we may be interested in:

        doubleInput.Add("doubleClickD", false);
        doubleInput.Add("doubleClickA", false);
        doubleInput.Add("doubleClickW", false);
        doubleInput.Add("doubleClickS", false);
    }

    void Update () {
        //Assign input returns to dictionary indexes:
        input["Horizontal"] = Input.GetAxis("Horizontal");
        input["Vertical"] = Input.GetAxis("Vertical");
        input["Run"] = Input.GetAxis("Run");
        //input["Jump"] = Input.GetAxis("Jump");
        input["Jump"] = 0;  // Jump has been disabled.
        input["BaseInteraction"] = Input.GetAxis("BaseInteraction");
        input["LightMax"] = Input.GetAxis("LightMax");
        input["LightSwitch"] = Input.GetAxis("LightSwitch");
        input["Pause"] = Input.GetAxis("Pause");
        input["Submit"] = Input.GetAxis("Submit");

        //Assign double click checkings statuses to dictionary indexes (Double click confirming):
        doubleInput["doubleClickD"] = false;
        //print("D: " + doubleInput["doubleClickD"] + " A: " + doubleInput["doubleClickA"] + " W: " + doubleInput["doubleClickW"] + " S: " + doubleInput["doubleClickS"]);

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastClickD < doubleClickDelay)
            {
                doubleInput["doubleClickD"] = true;
                //print("Double D input");
            }
            lastClickD = Time.time;
        }
        doubleInput["doubleClickA"] = false;
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastClickA < doubleClickDelay)
            {
                doubleInput["doubleClickA"] = true;
                //print("Double A input");
            }
            lastClickA = Time.time;
        }
        doubleInput["doubleClickW"] = false;
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastClickW < doubleClickDelay)
            {
                doubleInput["doubleClickW"] = true;
                //print("Double W input");
            }
            lastClickW = Time.time;
        }
        doubleInput["doubleClickS"] = false;
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastClickS < doubleClickDelay)
            {
                doubleInput["doubleClickS"] = true;
                //print("Double S input");
            }
            lastClickS = Time.time;
        }

        mousePos = Input.mousePosition;
    }

    public bool isPressed(string id)
    {
        return (input[id] > 0 || input[id] < 0);
    }

    public float getInput(string id)
    {
        return input[id];
    }

    public bool wasDoubleClicked(string id)
    {
        return doubleInput[id];
    }

    public Vector2 getMousePos() { return mousePos; }
}
