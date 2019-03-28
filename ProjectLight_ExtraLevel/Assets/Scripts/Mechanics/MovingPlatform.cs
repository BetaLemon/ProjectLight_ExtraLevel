using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    // The MovingPlatform script controlls a model that behaves like a moving platform. It moves between
    // the different nodes that are set in Unity's editor.

    public GameObject[] positions;         // Array with all the different positions it moves to. Set in Editor.
    public GameObject platform;            // Model that will be moved. Needs to have a Trigger Collider to work properly.

    public int nextNode = 0;               // The current node index inside the positions array, the platform is or would move towards. Can be used to choose where to start moving towards, else will choose closest
    public bool isRunning;                 // Controls if the platform is moving or not.
    public int runs = 0;                  // How many times the platform has been triggered
    [Tooltip("-1 for infinite runs.")]
    public int maxRuns = -1;                // How many times the platform can be retriggered (Infinite by default, -1)
    [Tooltip("-1 for infinite nodes.")]
    public int maxNodes = -1;               // How many nodes the platform will do. If exhausted, is running goes to false, the platform stops (Infinite by default, -1)
    public int nodesSinceRunStart = 0;    // How many nodes the platform has visited since triggered as running
    public bool runnable = true;           // If the platform can be triggered (Used for more than once avoidance)

    public float speed;                    // Speed at which the platform moves.


    void Start () {

    }
	
	void FixedUpdate () {
        if (isRunning)  // If the platform is running:
        {
            // Change the position of the platform to the one of the current node, at speed:
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, positions[nextNode].transform.position, speed);
            // If the platform has reached currentNode, then go to the next node (modular arithmetics):
            if (Vector3.Equals(platform.transform.position, positions[nextNode].transform.position)) {
                nodesSinceRunStart++;
                nextNode = (nextNode + 1) % positions.Length;
                if (nodesSinceRunStart >= maxNodes && maxNodes != -1) { //Nodes to visit limiter
                    nodesSinceRunStart = 0;
                    isRunning = false; //Stop the platform
                }
            }
        }
    }

    // If a Trigger Object is set to activate this moving platform, the following function will be called:
    public void getTriggered() {
        if (runnable)
        { //Start the platform movement only if runnable
            isRunning = true;
            runs++;
            if (runs >= maxRuns && maxRuns != -1) runnable = false; //Will stop further running
        }
    }
}
