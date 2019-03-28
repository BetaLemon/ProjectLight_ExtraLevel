using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    /*
    [FMODUnity.EventRef]
    public string playerFalldamageSound;
    [FMODUnity.EventRef]
    public string playerDamagedSound;
    [FMODUnity.EventRef]
    public string smallGemCollectSound;
    [FMODUnity.EventRef]
    public string largeGemCollectSound;
    [FMODUnity.EventRef]
    public string manaOrbCollectSound;
    */
    float timeForAnotherComplaint = 0.0f;
    float timeToComplain;


    public Transform healEffect;
    Transform healInstance;
    public Transform damageEffect;
    Transform damageInstance;

    public static Player instance;

    // Player Health system:
    public float health; //Current mana/health of the player. Magic and health are the same. If all health is lost, the player dies
    public float maxHealth = 100f; //Maximum playerHealth the player can reach
    private float minHealth = 0f; //Health at which the player dies
    public float respawnHealth = 25; //Health with which the player respawns after death

    //Player Economy:
    public int gemstones = 0;
    public int smallGemstones = 0;
    public Text LargeGemstones;
    public Text SmallGemstones;

    private float prevAxisMoveDir; //Previous MoveDirection.y value for variance checking
    public Transform lifeBar;

    private CharacterController controllerRef; //Own character controller reference
    private PlayerController playerControllerRef; //Own player controller script reference

    //Area control:
    [Tooltip("Sets where the player would respawn if not assigned to any area")]
    public GameObject baseWorldSpawn;
    private GameObject currentArea = null; //Last area where the player has entered

    void Awake() { if (instance == null) instance = this; }

	void Start () {
        timeToComplain = Time.deltaTime * 15;
        controllerRef = FindObjectOfType<CharacterController>();
        playerControllerRef = GetComponent<PlayerController>();
    }
	
	void Update () {

        //Health limiters:
        if (health > maxHealth) { health = maxHealth; }
        if (health < minHealth) { health = minHealth; Die(); }

        LifeBarAdjustment();

        if (Input.GetKeyDown(KeyCode.F1)) health = maxHealth;
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Area")) //Player enters area, we save the area
        {
            currentArea = other.gameObject;
        } 
        else if (other.gameObject.CompareTag("BlackInsect")) //Black insect's dark areas damage the player
        {
            if (damageInstance == null)
            {
                damageInstance = Instantiate(damageEffect, transform) as Transform;
                damageInstance.transform.position = transform.position;
            }
            timeForAnotherComplaint += Time.deltaTime;
            Debug.Log(timeForAnotherComplaint + "/" + timeToComplain);
            if (timeForAnotherComplaint >= timeToComplain)
            {
                timeForAnotherComplaint = 0.0f;
                //FMODUnity.RuntimeManager.PlayOneShot(playerDamagedSound);
            }

            //health -= other.gameObject.GetComponentInParent<BlackInsect>().getDamageDealt() * Time.deltaTime;
        }
        else if (other.gameObject.CompareTag("Lethal")) //Player enters lethal area, dies and respawns
        {
            Die();
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SmallGemstone"))
        {
            //FMODUnity.RuntimeManager.PlayOneShot(smallGemCollectSound);
            Destroy(other.gameObject); smallGemstones += 1;
            SmallGemstones.text = "x" + smallGemstones.ToString(); //Update GUI
        }
        else if (other.gameObject.CompareTag("Gemstone"))
        {
            //FMODUnity.RuntimeManager.PlayOneShot(largeGemCollectSound);
            Destroy(other.gameObject); gemstones += 1;
            LargeGemstones.text = "x" + gemstones.ToString(); //Update GUI
        }
        else if (other.gameObject.CompareTag("ManaCharge"))
        {
            //FMODUnity.RuntimeManager.PlayOneShot(manaOrbCollectSound);
            healInstance = Instantiate(healEffect, transform) as Transform;
            healInstance.transform.position = transform.position;


            Destroy(other.gameObject);
            health += 5;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BlackInsect"))
        {
            Destroy(damageInstance.gameObject);
        }
    }
    
    public void DestroyDamageInstance()
    {
        if(damageInstance != null) Destroy(damageInstance.gameObject);
    }

    void LifeBarAdjustment()
    {
        if(lifeBar == null) { Debug.Log("Please add lifebar object to Player Script."); }
        //lifeBar.localScale = new Vector3(health / maxHealth, health / maxHealth, health / maxHealth);
    }    

    private void Die() {
        print("Player died");
        //Send to spawn point:
        if (currentArea != null) {
            //transform.position = currentArea.GetComponentInChildren<GameObject>().transform.position;
            transform.position = currentArea.transform.GetChild(0).transform.position;
        }
        else {
            transform.position = baseWorldSpawn.transform.position; //Send to central world spawn
        }
        //Reset Health:
        health = respawnHealth;
    }

    public void fallDamage(float damage) {
        //FMODUnity.RuntimeManager.PlayOneShot(playerFalldamageSound);
        health -= damage;
    }
}
