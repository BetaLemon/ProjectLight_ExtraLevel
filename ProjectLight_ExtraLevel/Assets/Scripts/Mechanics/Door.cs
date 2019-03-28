using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    //[FMODUnity.EventRef]
    //public string doorSound;

    public GameObject LeftHinge;
    public GameObject RightHinge;

    private Animator DoorAnimation;

    public bool doorOpen;
    private bool animate; //Used for first animation skip on first update

    //public float doorSpeed = 0.1f; //Speed at which the door opens and closes

    void Start () {
        DoorAnimation = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        if (animate) { //Stops bug where door animates for the first time without changing state
            if (doorOpen)
            {
                DoorAnimation.SetBool("Close", false);
                DoorAnimation.SetBool("Open", true);
            }
            else if (!doorOpen)
            {
                DoorAnimation.SetBool("Close", true);
                DoorAnimation.SetBool("Open", false);
            }
        }
    }

    public void getTriggered() {
        animate = true;
        if (doorOpen) {
            doorOpen = false;
        }
        else {
            //FMODUnity.RuntimeManager.PlayOneShot(doorSound);
            doorOpen = true;
        }
    }
}
