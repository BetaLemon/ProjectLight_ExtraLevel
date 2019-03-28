using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {

    //TODO ESTO HABRÍA QUE QUITARLO MAS ADELANTE, ES UN APAÑO PARA PODER DECIDIR SI EL PORTAL LLEVA A INGAME (DESDE EL FILE SELECTOR), ES PROVISIONAL
    public GameStateScript gameStateDataScriptRef; //Reference to the Game/Global World Scene State
    public bool sendsIngame;

    void Start()
    {
        //Reference Initializations:
        gameStateDataScriptRef = GameObject.Find("GameState").GetComponent<GameStateScript>();
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = transform.GetChild(0).gameObject.transform.position;

            //TODO ESTO HABRÍA QUE QUITARLO MAS ADELANTE, ES UN APAÑO PARA PODER DECIDIR SI EL PORTAL LLEVA A INGAME (DESDE EL FILE SELECTOR), ES PROVISIONAL
            if (sendsIngame)
            {
                gameStateDataScriptRef.SetSceneState(GameStateScript.SceneState.INGAME);
            }
            //-----------------------------------------------------------------------------------------------------------------------------------------------
        }
    }
}