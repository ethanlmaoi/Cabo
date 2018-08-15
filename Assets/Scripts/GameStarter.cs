using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameStarter : NetworkBehaviour {

    public GameObject deckPrefab;
    GameObject deckPos;
    public GameObject discardPrefab;
    GameObject discardPos;

    Controller control;

    private void Start()
    {
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        deckPos = GameObject.FindGameObjectWithTag("Deck");
        discardPos = GameObject.FindGameObjectWithTag("Discard");
    }

    [Command]
    public void CmdStartGame()
    {
        GameObject deckObj = (GameObject)Instantiate(deckPrefab, deckPos.transform);
        NetworkServer.Spawn(deckObj);
        GameObject discardObj = (GameObject)Instantiate(discardPrefab, discardPos.transform);
        NetworkServer.Spawn(discardObj);

        control.startGame();
    }
}
