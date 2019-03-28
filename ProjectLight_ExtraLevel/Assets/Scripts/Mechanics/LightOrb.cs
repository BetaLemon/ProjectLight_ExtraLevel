using UnityEngine;

public class LightOrb : MonoBehaviour {

    private Player thePlayer; //Reference to the player
    private Trigger orbTrigger; // ! We could make it so that the light orb could contain multiple triggers.
    //private GameObject OrbGeometry;
    public GameObject AbsorbEffect;
    public GameObject ChargeEffect;
    //--------
    public bool isCharging = false;
    private bool wasCharging = true;
    private float chargeAmount = 0.0f;
    private Color lastEnteringColor = Color.white;

    public bool isSubtracting = false;
    private bool wasSubstracting = true;

    public Light glow;

    private float orbIntensity;                //Either: 10(has charge enrgy) or 0(no charge enrgy), defined on startup according to orbCharge.
    public float orbCharge;                    //Current orb charge. Is public in order to be set up on level design
    private float maxOrbCharge = 10f;
    private float minOrbCharge = 0f;
    private float orbGlowRangeFactor = 6f;     //Reduces orb glow range the higher it is
    private float minOrbGlowRange = 1.5f;      //The orb starts to glow directly from this range

    public Color color = Color.white;          //The orb's current color. Is public in order to be set up on level design
    public float autoRefillAmount = 0;
    public float refillDelay = 10;
    private float currentRefillDelay = 0;
    private bool waitingRefill = false;
    public GameObject chargeSphere;

    private float Lerp(float goal, float speed, float currentVal)
    {
        if (currentVal > goal)
        {
            if (currentVal - speed < goal) { return currentVal = goal; }
            return currentVal -= speed;
        }
        else if (currentVal < goal)
        {
            if (currentVal + speed > goal) { return currentVal = goal; }
            return currentVal += speed;
        }
        else return currentVal;
    }

    void Start ()
    {
        //OrbGeometry = transform.GetChild(1).gameObject; // Assign Orb Geometry reference to the second child of this gameObject
        thePlayer = Player.instance;  // Maybe use tags instead?
        orbTrigger = GetComponent<Trigger>();
        AbsorbEffect.SetActive(false);
        ChargeEffect.SetActive(false);
    }

    public void SubtractFromOrb() //Interaction function: Can, and is, used by Wizard interaction in order to absorb energy from the orb towards the wizard
    {
        if (orbCharge > minOrbCharge) //Priority for charging (Stops subtraction and charging at the same time)
        { 
            isSubtracting = true;

            //Interrupt natural refill:
            waitingRefill = false;
            currentRefillDelay = 0.0f;
        }
    }
    public void ChargeOrb(Color theEnteringColor, float theAmount) //Interaction function: Can, and is, used by Wizard interaction in order to charge the orb with the wizard's own mana
    {
        if (theEnteringColor == color && orbCharge != maxOrbCharge || orbCharge == 0 && orbCharge != maxOrbCharge)
        {
            isCharging = true;
            lastEnteringColor = theEnteringColor;
            chargeAmount = theAmount;

            //Interrupt natural refill:
            waitingRefill = false;
            currentRefillDelay = 0.0f;
        }
        //currentRefillDelay = 0;
    }
    public void SetOrbCharge(float amount) // You can directly set the orb's charge with this to a certain amount.
    {
        orbCharge = amount;
    }

    void Update()
    {
        if (isCharging) //Charging check
        {
            if (!wasCharging) { ChargeEffect.SetActive(true); color = lastEnteringColor; }
            orbCharge += chargeAmount; //The orb is filled with the standard ammount, which should/could be the same the wizard loses from his mana (orb deposition) for balance purposes
        }
        else if (!isCharging && wasCharging) //Not charging check
        {
            ChargeEffect.SetActive(false);
        }

        if (isSubtracting)
        {
            float exchange = thePlayer.GetComponent<PlayerLight>().healthDrainAmmount; //Is the same equivalent value for subtracting as for charging (You give what you can take)
            orbCharge -= exchange; //(orb subtraction)
            if (orbCharge > 0) thePlayer.GetComponent<Player>().health += exchange; //Increase player health from orb absortion as long as there's energy (The player isn't dead)
            if(!wasSubstracting) AbsorbEffect.SetActive(true);
        }
        else if (!isSubtracting && wasSubstracting)
        {
            AbsorbEffect.SetActive(false);
        }

        ///This boolean reset section guarantees positive boolean effects whont take place next frame unless explicitly toggled by public interaction functions 
        wasSubstracting = isSubtracting;
        wasCharging = isCharging;
        isSubtracting = false;
        isCharging = false;

        chargeAmount = 0.0f;

        if (chargeSphere != null) { OrbChargeSphere(); }
        //else { OrbGeometry.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", color); }
        else
        {
            chargeSphere.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", color);
            chargeSphere.GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", color);
        }

        //Orb energy charge limits:
        if (orbCharge >= maxOrbCharge) { orbCharge = maxOrbCharge; waitingRefill = false; currentRefillDelay = 0.0f; }
        else if (orbCharge <= minOrbCharge) { orbCharge = minOrbCharge; waitingRefill = true; }

        RefillSystem();

        //Update glow color:
        glow.color = color;
        //OrbGeometry.GetComponent<MeshRenderer>().materials[0].SetColor("_MKGlowColor", color);

        glow.range = minOrbGlowRange + orbCharge / orbGlowRangeFactor; //Orb light extension radius starts at a minimum, and extends the same as the current charge divided by a decreasing factor

        //Adjust glow intensity according to orb energy charge
        if (orbCharge > 0) orbIntensity = Lerp(10, 1, orbIntensity);
        else orbIntensity = Lerp(0, 1, orbIntensity);
        glow.intensity = orbIntensity;

        if(orbTrigger != null) {
            if (orbTrigger.type == Trigger.TriggerType.ON_COLOUR)
            {
                orbTrigger.pleaseTrigger(orbCharge, color);
            }
            else { orbTrigger.pleaseTrigger(orbCharge); }
        }
    }

    void OrbChargeSphere()
    {
        chargeSphere.transform.localScale = new Vector3(orbCharge / maxOrbCharge, orbCharge / maxOrbCharge, orbCharge / maxOrbCharge);
        chargeSphere.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", color);
        chargeSphere.GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", color*2);
    }

    void RefillSystem()
    {
        if (autoRefillAmount <= 0 || refillDelay < 0) return;
        if (waitingRefill) { currentRefillDelay += Time.deltaTime; }
        else { currentRefillDelay = 0; }
        if (waitingRefill && currentRefillDelay > refillDelay) { orbCharge += autoRefillAmount; isCharging = true; }
    }
}
