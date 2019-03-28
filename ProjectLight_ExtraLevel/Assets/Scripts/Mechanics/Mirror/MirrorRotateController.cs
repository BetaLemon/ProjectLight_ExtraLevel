using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotateController : MonoBehaviour {

    private MirrorRotation rot;
    private bool moving;
    private Vector2 rotation;

    private bool baseInteractionWasAlreadyPressed = false;

    public Transform activeIndicator;

	// Use this for initialization
	void Start () {
        rot = GetComponentInChildren<MirrorRotation>();
        moving = false;
	}

    void Update()
    {
        if (moving)
        {
            activeIndicator.localScale = new Vector3(1, 1, 1);
            rotation.x += Input.GetAxis("CamY");
            rotation.y += Input.GetAxis("CamX");
        }
        else { activeIndicator.localScale = new Vector3(0, 0, 0); }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(PlayerInput.instance.isPressed("BaseInteraction") && !baseInteractionWasAlreadyPressed) { moving = !moving; }
        if (moving)
        {
            float amount = rotation.x;
            rot.RotateHorizontal(amount);
            rotation.x -= amount;

            amount = rotation.y;
            rot.RotateVertical(amount);
            rotation.y -= amount;
        }
        baseInteractionWasAlreadyPressed = PlayerInput.instance.isPressed("BaseInteraction");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        moving = false;
    }

    public void Toggle()
    {
        moving = !moving;
    }
}
