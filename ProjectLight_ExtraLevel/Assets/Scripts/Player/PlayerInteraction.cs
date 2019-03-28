using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    public static PlayerInteraction instance; //Global instance of the player interaction script that can be accessed from other script from calling local functions

    public GameStateScript gameStateDataScriptRef; //Reference to the Game/Global World Scene State

    public LayerMask raycastLayers;

    public GameObject CylindricLight;
    public GameObject LightRayGeometry;

    public GameObject BaseWorldSpawnRef;

    float timer = 0;
    float selectDelay = 20; //Change highlighted file delay in frames
    int highlightedFilenum; //File 1 to 4 index
    private bool[] highlightedFile = new bool[2]; //The file selector is treated as a very simple 2x2 boolean slot matrix: Left slot = false means left column, Right slot = false means higher row
    /*public GameObject SelectorEnvironment;
    public GameObject FileSelectorBoat1;
    public GameObject FileSelectorBoat2;
    public GameObject FileSelectorBoat3;
    public GameObject FileSelectorBoat4;*/

    private RaycastHit rayHit;
    bool prevBaseInteraction;
    bool pressedBaseInteraction;

    private PlayerInput input;
    private PlayerLight light;

    private float amount;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start () {

        //Reference Initializations:
        gameStateDataScriptRef = GameObject.Find("GameState").GetComponent<GameStateScript>();
        BaseWorldSpawnRef = GameObject.Find("BaseWorldSpawn");

        //File selector: Default upper left boat selected
        highlightedFilenum = 1;
        highlightedFile[0] = false;
        highlightedFile[1] = false;
        /*
        SelectorEnvironment = GameObject.Find("MENU_PORT");
        FileSelectorBoat1 = SelectorEnvironment.transform.GetChild(0).gameObject;
        FileSelectorBoat2 = SelectorEnvironment.transform.GetChild(1).gameObject;
        FileSelectorBoat3 = SelectorEnvironment.transform.GetChild(2).gameObject;
        FileSelectorBoat4 = SelectorEnvironment.transform.GetChild(3).gameObject;
        */
        input = PlayerInput.instance;
        light = GetComponent<PlayerLight>();

        amount = light.healthDrainAmmount;
    }

    void FixedUpdate()
    {

        if (gameStateDataScriptRef.GetSceneState() == GameStateScript.SceneState.INGAME) //This update section only works if we're INGAME:
        {
            //print(input.getInput("Pause"));

            //Pause input:
            if (input.getInput("Pause") != 0)
            {
                gameStateDataScriptRef.PauseGame(!gameStateDataScriptRef.gamePaused);
                gameStateDataScriptRef.SetSceneState(GameStateScript.SceneState.OPTIONS);
            }

            pressedBaseInteraction = input.isPressed("BaseInteraction");

            /// PASSIVE INTERACTION (Sphere Light)
            Collider[] hitColliders = Physics.OverlapSphere(CylindricLight.transform.position, GetComponent<PlayerLight>().lightSphere.range - 3); //(Sphere center, Radius)
            int tmp = 0;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].isTrigger)
                {
                    if (hitColliders[i].gameObject.CompareTag("PlayerLight")) { continue; }
                    switch (hitColliders[i].gameObject.tag)
                    {
                        case "LightOrb":
                            if (input.isPressed("LightMax")) hitColliders[i].GetComponent<LightOrb>().ChargeOrb(Color.white, amount); //Attempt to charge the light orb if we are expanding the player light sphere radius (Default white from player white ray)
                            else if (input.isPressed("BaseInteraction")) hitColliders[i].GetComponent<LightOrb>().SubtractFromOrb(); //Attempt to subtract energy from the light orb if we press Q
                            break;
                        case "BlackInsect":
                            BlackInsect(hitColliders[i]);
                            break;
                        case "Mirror":
                            //if (pressedBaseInteraction != 0 && prevBaseInteraction == 0) { FindObjectOfType<CameraScript>().setFocus(hitColliders[i].gameObject); }
                            //if (pressedBaseInteraction && !prevBaseInteraction) { hitColliders[i].GetComponentInParent<MirrorRotateController>().Toggle(); }
                            break;
                        case "OpticalFiber":
                            // Make closest node work (switching reverse or not):
                            hitColliders[i].GetComponentInParent<OpticalFiber>().SetClosestNode(transform);

                            // Charge optical fiber:

                            if (input.isPressed("LightMax")) hitColliders[i].GetComponent<OpticalFiber_Node>().AddCharge(amount);
                            //else if (input.isPressed("BaseInteraction")) hitColliders[i].GetComponentInParent<OpticalFiber>().StartPlayerMode(transform);
                            break;
                        default: break;
                    }
                    tmp++;
                }
            }

            prevBaseInteraction = pressedBaseInteraction;
        }
        /*
        else if (gameStateDataScriptRef.GetSceneState() == GameStateScript.SceneState.FILESELECT) //This update section only works if we're in FILESELECT:
        {
            ++timer;

            if (input.getInput("Horizontal") != 0 && timer > selectDelay)
            {
                highlightedFile[0] = !highlightedFile[0];
                timer = 0;
            }
            else if (input.getInput("Vertical") != 0 && timer > selectDelay)
            {
                highlightedFile[1] = !highlightedFile[1];
                timer = 0;
            }

            boatOutlinesOff();

            //FILE SELECTOR:
            if (highlightedFile[0] == false && highlightedFile[1] == false)
            {
                highlightedFilenum = 1;
                FileSelectorBoat1.GetComponentInChildren<cakeslice.Outline>().enabled = true;
            }
            else if (highlightedFile[0] == true && highlightedFile[1] == false)
            {
                highlightedFilenum = 2;
                FileSelectorBoat2.GetComponentInChildren<cakeslice.Outline>().enabled = true;
            }
            else if (highlightedFile[0] == false && highlightedFile[1] == true)
            {
                highlightedFilenum = 3;
                FileSelectorBoat3.GetComponentInChildren<cakeslice.Outline>().enabled = true;
            }
            if (highlightedFile[0] == true && highlightedFile[1] == true)
            {
                highlightedFilenum = 4;
                FileSelectorBoat4.GetComponentInChildren<cakeslice.Outline>().enabled = true;
            }

            //INSERT LOAD SELECTED FILE HERE:
            if (input.getInput("Submit") != 0)
            {
                //Provisional stuff for now:
                boatOutlinesOff();
                gameStateDataScriptRef.SetSceneState(GameStateScript.SceneState.INGAME);

                transform.position = BaseWorldSpawnRef.transform.position;
            }
        }*/
    }

    void Update()
    {
        /// ACTIVE INTERACTION (Cylinder LightRay)
        if (GetComponent<PlayerLight>().getLightMode() == PlayerLight.LightMode.FAR) // If the player uses the Cylinder Light.
        {
            Debug.DrawRay(CylindricLight.transform.position, LightRayGeometry.transform.forward * light.maxLightCylinderScale * 2, Color.red);
            if (Physics.Raycast(CylindricLight.transform.position, LightRayGeometry.transform.forward, out rayHit, light.maxLightCylinderScale * 2))  //(vec3 Origin, vec3direction, vec3 output on intersection) If Raycast hits a collider.
            {
                float distCylPosHitPos = Vector3.Distance(getRayHit().point, CylindricLight.transform.position);

                // Specific game object interactions with light cylinder:
                if (rayHit.collider.gameObject.CompareTag("Mirror")) { Mirror(rayHit); } //Reflect mirror light
                if (rayHit.collider.gameObject.CompareTag("Filter")) { Filter(rayHit); } //Process light ray
                if (rayHit.collider.gameObject.CompareTag("LightOrb")) { rayHit.collider.GetComponentInParent<LightOrb>().ChargeOrb(Color.white, amount); } //Charge the light orb (Default white from player white ray)
                if (rayHit.collider.gameObject.CompareTag("Trigger")) { TriggerTrigger(rayHit); }
                if (rayHit.collider.gameObject.CompareTag("BlackInsect")) { BlackInsect(rayHit.collider); }
                if (rayHit.collider.gameObject.CompareTag("Prism")) { Prism(rayHit); }
            }
        }
    }
    /*
    public void boatOutlinesOff()
    {
        FileSelectorBoat1.GetComponentInChildren<cakeslice.Outline>().enabled = false;
        FileSelectorBoat2.GetComponentInChildren<cakeslice.Outline>().enabled = false;
        FileSelectorBoat3.GetComponentInChildren<cakeslice.Outline>().enabled = false;
        FileSelectorBoat4.GetComponentInChildren<cakeslice.Outline>().enabled = false;
    }
    */
    void BlackInsect(Collider col)
    {
        //col.gameObject.GetComponentInParent<BlackInsect>().Hurt();
    }

    void Mirror(RaycastHit mirrorHit)
    {
        Vector3 inVec = mirrorHit.point - CylindricLight.transform.position;
        mirrorHit.collider.GetComponentInParent<Mirror>().Reflect(inVec, mirrorHit.normal, mirrorHit.point, Color.white);
        LightRayGeometry.transform.localScale = new Vector3(light.cylinderRadius, light.cylinderRadius, Vector3.Distance(mirrorHit.point, LightRayGeometry.transform.position)); // Limit the light ray's length to the object
    }

    void Filter(RaycastHit filterHit)
    {
        Vector3 inVec = filterHit.point - CylindricLight.transform.position;
        filterHit.collider.GetComponentInParent<RayFilter>().Process(inVec, filterHit.point);
        LightRayGeometry.transform.localScale = new Vector3(light.cylinderRadius, light.cylinderRadius, Vector3.Distance(filterHit.point, LightRayGeometry.transform.position)); // Limit the light ray's length to the object
    }

    void TriggerTrigger(RaycastHit rh)
    {
        rh.collider.gameObject.GetComponentInParent<Trigger>().pleaseTrigger();
    }

    void Prism(RaycastHit rh)
    {
        Vector3 inVec = rh.point - CylindricLight.transform.position;
        rh.collider.GetComponentInParent<Prism>().Process(inVec, rh.point, rh.normal);
        LightRayGeometry.transform.localScale = new Vector3(light.cylinderRadius, light.cylinderRadius, Vector3.Distance(rh.point, LightRayGeometry.transform.position));
    }

    public RaycastHit getRayHit()
    {
        return rayHit;
    }
}
