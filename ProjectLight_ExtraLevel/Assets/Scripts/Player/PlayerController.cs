using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum PlayerState { STANDING, WALKING, RUNNING, FALLING };

    #region Variables
    // Variables:
    CharacterController controller;       // For controlling the player.
    public GameStateScript gameStateDataScriptRef; //Reference to the Game/Global World Scene State
    PlayerState state;
    // Player movement variables:
    public float rotateSpeed = 10f;
    public float walkSpeed = 3.0f;        // Maximal speed when walking.
    public float runSpeed = 6.0f;         // Maximal speed when running.
    public float gravity = 1.0f;          // Gravity (unfortunately, it's linear for the moment being).
    public float minimumFallDamageDistance = 150;
    // Intern player variables:
    private float speed;                  // Speed applied to player.
    private Vector3 moveDirection;        // The direction the player is gonna move towards.
    private Vector3 forward;
    private float fallDistance;
    private float prevFallDistance;
    // Used for controlling the player:
    private PlayerInput input;
    private bool canMove = true;
    // Used for making the player move where the camera is looking at:
    private Camera camera;
    private Vector3 camForward;
    private Vector3 camRight;
    private Animator animator;
    public static PlayerController instance;
    #endregion

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()    // When the script starts.
    {
        controller = GetComponent<CharacterController>();                                           // We get the player's CharacterController.
        gameStateDataScriptRef = GameObject.Find("GameState").GetComponent<GameStateScript>();      // Game State Script ref
        moveDirection = Vector3.zero;                                                               // We set the player's direction to (0,0,0).
        input = GetComponent<PlayerInput>();                                                        // We get the player's input controller.
        camera = Camera.main;                                                                       // We fetch the main camera.
        state = PlayerState.STANDING;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!canMove || gameStateDataScriptRef.GetSceneState() != GameStateScript.SceneState.INGAME) return;   // If the player can't move or isn't on GameStateScript.cs INGAME mode, then the following code shouldn't be executed.

        if (input.getInput("Horizontal") == 0 && input.getInput("Vertical") == 0) { state = PlayerState.STANDING; }
        else
        {
            state = PlayerState.WALKING;
            if (input.getInput("Run") != 0) { state = PlayerState.RUNNING; }
        }

        //Check for a double input of: W, A, S, D, and if so, apply runModeActive true
        if (input.wasDoubleClicked("doubleClickD")) { state = PlayerState.RUNNING; }
        else if (input.wasDoubleClicked("doubleClickA")) { state = PlayerState.RUNNING; }
        else if (input.wasDoubleClicked("doubleClickW")) { state = PlayerState.RUNNING; }
        else if (input.wasDoubleClicked("doubleClickS")) { state = PlayerState.RUNNING; }

        if (state == PlayerState.RUNNING) //The player should walk, run mode is inactive
        {
            speed += runSpeed * Time.deltaTime * 2f;
            if (speed > runSpeed) { speed = runSpeed; }
        }
        else if (state == PlayerState.WALKING) //Run mode is active, apply run speed
        {
            speed += walkSpeed * Time.deltaTime * 2f;
            if (speed > walkSpeed) { speed = walkSpeed; }
        }
        else if (state == PlayerState.STANDING) { speed = 0; }

        //print("State: " + state + ", Speed: " + speed);

        //Basic movement system:
        moveDirection.x = input.getInput("Horizontal") * speed;  // The player's x movement is the Horizontal Input (0-1) * speed.
        moveDirection.z = input.getInput("Vertical") * speed;    // The player's y movement is the Vertical Input (0-1) * speed.

        AnimatorUpdate();
    }

    void FixedUpdate()  // What the script executes at a fixed framerate. Good for physics calculations. Avoids stuttering.
    {
        if (!canMove) { return; }
        if (!controller.isGrounded) // If the player is not grounded / is in the air.
        {
            moveDirection.y = -1*gravity; // We apply gravity.
            fallDistance += Mathf.Abs(moveDirection.y);
            state = PlayerState.FALLING;
        }
        else    // Else, the player is touching the ground.
        {
            //moveDirection.y = 0;    // His vertical (y) movement is reset.
            fallDistance = 0;
            state = PlayerState.STANDING;
        }

        if(fallDistance == 0 && prevFallDistance != 0)
        {
            //print(prevFallDistance);
            if(prevFallDistance > minimumFallDamageDistance)
            {
                GetComponent<Player>().fallDamage(prevFallDistance);
            }
        }

        prevFallDistance = fallDistance;

        camForward = camera.transform.forward;
        camRight = camera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 move = moveDirection.x * camRight + moveDirection.z * camForward;
        move.y = moveDirection.y;

        controller.Move(move * Time.deltaTime);    // We tell the CharacterController to move the player in the direction, by the Delta for smoothness.

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            forward = move.normalized; forward.y = 0;
            //controller.gameObject.transform.forward = forward;
            //transform.forward = Vector3.RotateTowards(transform.forward, forward, speed, speed);
            float angle = Vector3.Angle(transform.forward, forward);
            if (angle > 170)
            {
                transform.forward = Vector3.Lerp(transform.forward, new Vector3(forward.x, 0, forward.z), Time.deltaTime * 45f);
            }
            else
            {
                transform.forward = Vector3.Lerp(transform.forward, new Vector3(forward.x, 0, forward.z), Time.deltaTime * 40f);
            }
        }
        AnimatorUpdate();
    }

    void OnTriggerStay(Collider other)  // If entering a Trigger Collider.
    {
        if(other.gameObject.tag == "MovingPlatform")    // If the trigger belongs to the MovingPlatform, make that the player's parent.
        {
            transform.parent = other.transform;
        }
    }

    void OnTriggerExit(Collider other)  // If leaving a Trigger Collider.
    {
        if(other.gameObject.tag ==   "MovingPlatform")  // If he's leaving the MovingPlatform, make the player it's own.
        {
            transform.parent = null;
        }
    }

    void AnimatorUpdate()
    {
        switch (state)
        {
            case PlayerState.FALLING:
                animator.SetBool("isWalking", false);
                animator.SetBool("isGrounded", false);
                animator.SetBool("isRunning", false);
                break;
            case PlayerState.RUNNING:
                animator.SetBool("isWalking", true);
                animator.SetBool("isGrounded", true);
                animator.SetBool("isRunning", true);
                break;
            case PlayerState.STANDING:
                animator.SetBool("isWalking", false);
                animator.SetBool("isGrounded", true);
                animator.SetBool("isRunning", false);
                break;
            case PlayerState.WALKING:
                animator.SetBool("isWalking", true);
                animator.SetBool("isGrounded", true);
                animator.SetBool("isRunning", false);
                break;
        }
        //Debug.Log(state);
    }

    public void AllowMovement() { canMove = true; }

    public void Move(Vector3 direction) {
        controller.Move(direction * Time.deltaTime);
    }

    public void StopMovement() {
        canMove = false;
        state = PlayerState.STANDING;
        AnimatorUpdate();
    }

    public PlayerState GetState() { return state; }
}
