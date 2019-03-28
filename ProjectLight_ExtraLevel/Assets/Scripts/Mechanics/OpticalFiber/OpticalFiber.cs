using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpticalFiber : MonoBehaviour {

    enum OF_Mode { LIGHT, PLAYER };

    private OF_Mode mode;

    private Transform[] nodes;
    private GameObject[] cables;
    public GameObject cablePrefab;
    private GameObject connectionsContainer;

    private float propagationRate = 0.2f;
    private float dissipateRate = 1.0f;

    public bool reversed;
    private bool wasReversed;

    private int nextNodeIndex;
    private Transform player;
    private float speed = 10;
    private float tolerance = 0.1f;

    private float playerModeDelay = 2f;
    private float currentPlayerModeDelay = 0;

	// Use this for initialization
	void Start () {
        OpticalSetup();
        reversed = false;
        wasReversed = false;
        mode = OF_Mode.LIGHT;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        SetClosestNode(Player.instance.transform);  // This fixes looping Optic Fiber. I don't even know why this wasn't here in the first place...
        currentPlayerModeDelay += Time.deltaTime; if(currentPlayerModeDelay > playerModeDelay) { currentPlayerModeDelay = playerModeDelay+1; }
        switch (mode)
        {
            case OF_Mode.LIGHT: // If we want to be propagating light, not the player (this is most of the time):
                ChargePropagation();
                if (wasReversed != reversed)
                {
                    ReverseNodes();
                    ReverseCables();
                    wasReversed = reversed;
                }
                ChargeDissipation();
                break;
            case OF_Mode.PLAYER:
                ChargePropagation();
                if(Player.instance.health <= 0) { StopPlayerMode(); break; }    // If Player Health is 0 or below, stop transportating player.
                if (nextNodeIndex + 1 >= nodes.Length) { StopPlayerMode(); break; }

                Vector3 direction = nodes[nextNodeIndex+1].position - player.position;
                //direction.y = 0;

                 //Debug.DrawRay(player.position, direction.normalized * 10, Color.red);
                
                player.GetComponent<PlayerController>().Move(direction.normalized*speed);

                //Vector2 nodev = new Vector2(nodes[nextNodeIndex + 1].position.x, nodes[nextNodeIndex + 1].position.z);
                //Vector2 playv = new Vector2(player.position.x, player.position.z);
                // if (Vector2.Distance(nodev, playv) < tolerance) { nextNodeIndex++; }

                if (Vector3.Distance(player.position, nodes[nextNodeIndex+1].position) < tolerance) { nextNodeIndex++; }
                ChargeDissipation();
                break;
        }
	}

    void OpticalSetup()
    {
        nodes = GetComponentsInChildren<Transform>();  // gets all children objects.
        //print(nodes.Length);
        
        // Filters unwanted objects:
        List<Transform> newNodes = new List<Transform>();
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].name[0] == 'n' || nodes[i].name[0] == 'N')
            {
                newNodes.Add(nodes[i]);
            }
            else if(nodes[i].name == "Connections")
            {
                connectionsContainer = nodes[i].gameObject;
            }
        }

        nodes = new Transform[newNodes.Count];
        for(int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = newNodes[i];
        }

        // Setup all connection cables:
        cables = new GameObject[nodes.Length - 1];
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            GameObject cable = Instantiate(cablePrefab) as GameObject;
            cable.GetComponent<CableSetup>().start = nodes[i].gameObject;
            cable.GetComponent<CableSetup>().end = nodes[i+1].gameObject;
            cable.GetComponent<CableSetup>().Setup();
            cable.transform.parent = connectionsContainer.transform;
            cables[i] = cable;
        }
    }

    void ChargePropagation()
    {
        OpticalFiber_Node n = nodes[0].GetComponent<OpticalFiber_Node>();
        CableSetup c = cables[0].GetComponent<CableSetup>();

        c.charge += (n.charge - c.charge) * propagationRate;

        for (int i = 1; i < nodes.Length; i++)
        {
            c = cables[i-1].GetComponent<CableSetup>();
            n = nodes[i].GetComponent<OpticalFiber_Node>();
            
            n.charge += (c.charge - n.charge) * propagationRate;

            if(i < cables.Length)
            {
                c = cables[i].GetComponent<CableSetup>();
                c.charge += (n.charge - c.charge) * propagationRate;
            }
        }


        // PROPAGATION WITHOUT CABLES:
        //for(int i = 0; i < nodes.Length - 1; i++)
        //{
        //    OpticalFiber_Node current = nodes[i].gameObject.GetComponent<OpticalFiber_Node>();
        //    OpticalFiber_Node next = nodes[i+1].gameObject.GetComponent<OpticalFiber_Node>();

        //    next.charge += (current.charge - next.charge) * 0.2f;
        //}
    }

    void ChargeDissipation()
    {
        OpticalFiber_Node node = nodes[0].GetComponent<OpticalFiber_Node>();
        if (!node.isReceivingLight())
        {
            node.charge -= dissipateRate * propagationRate;
            if (node.charge < 0) { node.charge = 0; }
        }
    }

    // Automatically controlls reversibility:
    public void SetClosestNode(Transform player)
    {
        float distance1;
        float distance2;

        if (!reversed)
        {
            distance1 = Vector3.Distance(nodes[0].transform.position, player.position);
            distance2 = Vector3.Distance(nodes[nodes.Length - 1].transform.position, player.position);
        }
        else
        {
            distance2 = Vector3.Distance(nodes[0].transform.position, player.position);
            distance1 = Vector3.Distance(nodes[nodes.Length - 1].transform.position, player.position);
        }

        if(distance1 < distance2) { reversed = false; }
        else { reversed = true; }
    }

    void ReverseNodes()
    {
        Transform[] rev = new Transform[nodes.Length];
        int index = nodes.Length-1;
        for(int i = 0; i < nodes.Length; i++)
        {
            rev[i] = nodes[index];
            index--;
            if(index < 0) { break; }
        }
        nodes = rev;
    }

    void ReverseCables()
    {
        GameObject[] rev = new GameObject[cables.Length];
        int index = cables.Length - 1;
        for (int i = 0; i < cables.Length; i++)
        {
            rev[i] = cables[index];
            index--;
            if(index < 0) { break; }
        }
        cables = rev;
    }

    public void StartPlayerMode(Transform _player)
    {
        if(currentPlayerModeDelay < playerModeDelay) { return; }
        mode = OF_Mode.PLAYER;
        player = _player;
        player.parent = transform;
        player.gameObject.GetComponent<PlayerController>().StopMovement();
        nextNodeIndex = 0;
        player.position = nodes[nextNodeIndex].position;
        //player.position = new Vector3(player.position.x, player.position.y + 1, player.position.z);
        player.localScale = new Vector3(0, 0, 0);
        ToggleColliders(false);
        //nodes[0].GetComponent<OpticalFiber_Node>().AddCharge(4);
    }

    void StopPlayerMode()
    {
        mode = OF_Mode.LIGHT;
        player.parent = null;
        player.gameObject.GetComponent<PlayerController>().AllowMovement();
        player.localScale = new Vector3(1, 1, 1);
        ToggleColliders(true);
        currentPlayerModeDelay = 0;
    }

    void ToggleColliders(bool enable)
    {
        SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>();
        foreach(SphereCollider col in colliders)
        {
            col.enabled = enable;
        }
    }
}
