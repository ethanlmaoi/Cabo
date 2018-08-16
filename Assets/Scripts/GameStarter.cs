using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameStarter : NetworkBehaviour {
    
    Controller control;

    private void Start()
    {
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
    }
    
    public void startGame()
    {
        control.CmdStartGame();
    }
}
