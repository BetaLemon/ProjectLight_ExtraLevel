using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This scripts checks if obligatory elements were triggered.
 * REQUIREMENTS:
 *  - Puzzle Object containing all elements of the puzzle needs to have this script.
 *  - All triggers need to have this parent GameObject ("Puzzle X" for example) as triggeredObject.
 */

public class PuzzleCompletionController : MonoBehaviour {

    //[FMODUnity.EventRef]
    //public string puzzleCompleteSound;

    public enum PuzzleState { TRIED, UNTOUCHED, COMPLETED };

    #region Variables
    private PuzzleState state;
    private int completedTriggers;
    private int neededTriggers;
    public bool shouldTriggerSomething = false;
    public GameObject[] something;
    #endregion

    // Use this for initialization
    void Start () {
        state = PuzzleState.UNTOUCHED;
        completedTriggers = 0;

        // We need to get all triggers necessary for completing the level. They will have ourselves as triggeredObject.
        Trigger[] triggers = GetComponentsInChildren<Trigger>();
        neededTriggers = 0;
        foreach(Trigger trigger in triggers)
        {
            if (trigger.HasPuzzleCompletionTrigger()) { neededTriggers++; }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(completedTriggers >= neededTriggers && state != PuzzleState.COMPLETED) {
            state = PuzzleState.COMPLETED;
            if(shouldTriggerSomething) TriggerAllObjects();
        }
	}

    public void getTriggered() {
        completedTriggers++;
        //FMODUnity.RuntimeManager.PlayOneShot(puzzleCompleteSound);
    }

    public PuzzleState getState() { return state; }

    void TriggerAllObjects()
    {
        //sDebug.Log("Triggered All Objects!");
        for (int i = 0; i < something.Length; i++)   // For all the objects in the array that need to be triggered:
        {
            switch (something[i].tag)    // For the type of object that is triggered, we have each of the actions to be done:
            {
                case "MovingPlatform":
                    MovingPlatform platform = something[i].GetComponent<MovingPlatform>();
                    platform.getTriggered();
                    break;
                case "Door":
                    Door door = something[i].GetComponent<Door>();
                    door.getTriggered();
                    break;
            }

            PuzzleCompletionController puzCon = something[i].GetComponent<PuzzleCompletionController>();
            if (puzCon != null && puzCon.getState() != PuzzleCompletionController.PuzzleState.COMPLETED)
            {
                puzCon.getTriggered();
            }
        }
    }
}
