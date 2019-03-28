using System.Collections;
using UnityEngine;

//This script can keep track of stuff globally.


public class GameStateScript : MonoBehaviour {

    //Mini soundtrack controller with FMOD:
    /*
    [FMODUnity.EventRef]
    public string menuOST;
    [FMODUnity.EventRef]
    public string domainsOST;
    [FMODUnity.EventRef]
    public string seaSound;
    [FMODUnity.EventRef]
    public string plainsOST;
    [FMODUnity.EventRef]
    public string forestOST;
    [FMODUnity.EventRef]
    public string towerOST;

    FMOD.Studio.EventInstance menuSong;
    FMOD.Studio.EventInstance domainsSong;
    FMOD.Studio.EventInstance seaEffect;
    FMOD.Studio.EventInstance plainsSong;
    FMOD.Studio.EventInstance forestSong;
    FMOD.Studio.EventInstance towerSong;
    */
    //--------------------------------------

    private bool side = false; //Did we come from or are we ingame side (true), or otherwise from/in the main menu side (false)?
    public enum SceneState { MAINMENU, OPTIONS, FILESELECT, INGAME };
    SceneState prevFrameState = SceneState.INGAME;
    SceneState state = SceneState.INGAME;

    public int currentIngameOSTIndexPlaying = -1;

    public bool gamePaused;

    public GameObject FileSelectorSpawnRef;
    public GameObject PlayerRef;
    public GameObject PlayerCamerasRef;
    public Cinemachine.CinemachineBrain cinemachineBrain; //The cinemachine brain script reference
    //public GameObject MainMenuRef;
    //public GameObject MainMenuCanvasRef;
    //public GameObject OptionsRef;
    //public GameObject HudRef;

    public static GameStateScript instance;

    private void Awake()
    {
        if(instance == null) { instance = this; }
    }

    //WORLD SCENE START EVENTS
    void Start()
    {
        /*
        domainsSong = FMODUnity.RuntimeManager.CreateInstance(domainsOST);
        seaEffect = FMODUnity.RuntimeManager.CreateInstance(seaSound);
        plainsSong = FMODUnity.RuntimeManager.CreateInstance(plainsOST);
        //forestSong = FMODUnity.RuntimeManager.CreateInstance(forestOST);
        towerSong = FMODUnity.RuntimeManager.CreateInstance(towerOST);
        menuSong = FMODUnity.RuntimeManager.CreateInstance(menuOST);
        */
        //menuSong.start();

        //Reference Initializations:
        //PlayerInteraction.instance.boatOutlinesOff();
        //PlayerRef = GameObject.FindGameObjectWithTag("Player");
        //PlayerRef.SetActive(false);
        PlayerCamerasRef = GameObject.Find("PlayerCameraAngles"); //All of the player camera angles inside the wrapper game object
        //cinemachineBrain = PlayerCamerasRef.GetComponent<Cinemachine.CinemachineBrain>(); //Cinemachine brain script reference
        //MainMenuCanvasRef = MainMenuRef.transform.GetChild(2).gameObject;
        //HudRef = GameObject.Find("HUD");
        FileSelectorSpawnRef = GameObject.Find("FileSelectorSpawn");

        //The state on the previous frame. Used for detecting state changes:
        prevFrameState = SceneState.INGAME;
        //First state considering the first thing we see on scene start is the menu:
        state = SceneState.INGAME;
        //Ingame hud off:
        //HudRef.SetActive(false);
    }

    void Update()
    {
        //print("Current Scene State: " + state);
        prevFrameState = state; //Reference to current state for next frame
    }

    public void StateChanged()
    {
        if (state == SceneState.MAINMENU)
        {
            /*stopIngameMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);

            FMOD.Studio.PLAYBACK_STATE musicState;
            menuSong.getPlaybackState(out musicState);
            if (musicState != FMOD.Studio.PLAYBACK_STATE.PLAYING) //Anti overlap check
            {
                menuSong.start();
            }*/

            side = false; //MainMenu Side (Mark indicating the game was abnandoned or hasn't started yet)

            cinemachineBrain.m_DefaultBlend.m_Time = 0; //This fixes some camera bugs... it can also be used for dinamically changing the blend time for all cameras

            //Player gets disabled
            PlayerRef.SetActive(false);
            //Player gets sent to file selector
            PlayerRef.transform.position = FileSelectorSpawnRef.transform.position;
            PlayerRef.transform.rotation = FileSelectorSpawnRef.transform.rotation;

            /*HudRef.SetActive(false);
            OptionsRef.SetActive(false);
            MainMenuRef.SetActive(true);
            MainMenuCanvasRef.SetActive(true);

            OptionsRef.transform.GetChild(2).gameObject.SetActive(false); //Disactivates the back to game button since we're on the main menu and game is not active
            */
        }
        else if (state == SceneState.OPTIONS)
        {
            /*MainMenuCanvasRef.SetActive(false);
            OptionsRef.SetActive(true);
            if (side) //Stuff that happens only if you come from the game
            {
                PauseGame(true);
                OptionsRef.transform.GetChild(2).gameObject.SetActive(true); //Activates the back to game button
            }*/
        }
        else if (state == SceneState.FILESELECT)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = 2; //Cameras blend time to standard
            PlayerRef.SetActive(true);
            //MainMenuRef.SetActive(false);
        }
        else if (state == SceneState.INGAME)
        {
           // menuSong.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            side = true; //Ingame side (Mark indicating the game has started or is paused)

            //HudRef.SetActive(true);
            //OptionsRef.SetActive(false);
            PauseGame(false);
        }
    }

    public void SetSceneState(SceneState theState)
    {
        state = theState;
        //print("State Set To: " + state);

        if (prevFrameState != state) //Scene change checker
        {
            StateChanged();
        }
    }
    public SceneState GetSceneState()
    {
        return state;
    }
    public bool GetSceneSide()
    {
        return side;
    }
    public void PauseGame(bool to)
    {
        //if (state == SceneState.INGAME) //You can only pause the game ingame
        {
            gamePaused = to;
            if (gamePaused)
            {
                Time.timeScale = 0.5f;
                //print("Game Paused");
            }
            else
            {
                Time.timeScale = 1;
                //print("Game Unpaused");
            }
        }
    }

    public void cameraCoroutine(float time)
    {
        StartCoroutine(CameraBackToNormalTransitionTime(time));
    }
    /*
    public void stopIngameMusic(FMOD.Studio.STOP_MODE mode)
    {
        domainsSong.stop(mode);
        seaEffect.stop(mode);
        plainsSong.stop(mode);
        //forestSong.stop(mode);
        towerSong.stop(mode);
    }

    public void playOST(int index)
    {
        stopIngameMusic(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        switch (index)
        {
            case 0:
                domainsSong.start();
                break;
            case 1:
                seaEffect.start();
                break;
            case 2:
                plainsSong.start();
                break;
            //case 3:
            //    forestSong.start();
            //    break;
            case 4:
                towerSong.start();
                break;
        }

        currentIngameOSTIndexPlaying = index;
    }

    public int getCurrentIndexPlaying()
    {
        return currentIngameOSTIndexPlaying;
    }
    */
    //Corutina que vuelve a poner la transición de camaras bien. Si, es una cutrada pero es muy util joder
    IEnumerator CameraBackToNormalTransitionTime(float time)
    {
        yield return new WaitForSeconds(time);

        cinemachineBrain.m_DefaultBlend.m_Time = 2;
    }
}
