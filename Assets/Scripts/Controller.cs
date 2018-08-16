using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Controller : NetworkManager {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;

    int numSpawned = 0;
    PlayerScript[] players;
    int currPlayerInd = 0;
    bool nextPlayer = false;

    public GameObject deckObj;
    Deck deck;
    public GameObject discardObj;
    Discard discard;

    bool beginning;

	// Use this for initialization
	void Start () {
        players = new PlayerScript[MAX_PLAYERS];
        deck = deckObj.GetComponent<Deck>();
        discard = discardObj.GetComponent<Discard>();
        beginning = true;
    }
	
	// FixedUpdate is called independent of frame
	void FixedUpdate () {
        if(beginning)
        {
            //TODO not working with multiple players
            for(int i = 0; i < numSpawned; i++)
            {
                if(!players[i].isWaiting())
                {
                    break;
                }
                if(i == numSpawned - 1)
                {
                    beginning = false;
                    nextPlayer = true;
                    Debug.Log("beginning game");
                }
            }
        }
        if(nextPlayer)
        {
            if(deck.size() == 0)
            {
                discard.shuffleIntoDeck(deck);
            }
            players[currPlayerInd].startTurn();
            currPlayerInd = (currPlayerInd + 1) % numSpawned;
            nextPlayer = false;
        }
	}

    public void startGame()
    {
        deck.CmdStartGame();

        foreach (PlayerScript player in players)
        {
            if (player == null) continue;
            for (int i = 0; i < STARTING_HAND_SIZE; i++)
            {
                HandCard moveDest = player.addCard(deck.drawCard());
                Debug.Log("moving to " + moveDest);
                //TODO animate moving card from deck to moveDest
            }
            player.begin();
        }
    }

    public void nextPlayerTurn()
    {
        nextPlayer = true;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        numSpawned++;
        Transform[] pos = startPositions.ToArray();
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        for (int i = 0; i < pos.Length; i++) {
            if (pos[i].name.Equals("Spawn Position " + numSpawned))
            {
                spawnPos = pos[i].position;
                spawnRot = pos[i].rotation;
            }
        }
        GameObject player = (GameObject)Instantiate(playerPrefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        players[numSpawned - 1] = player.GetComponent<PlayerScript>();
    }

    public override void OnStartHost()
    {
        numSpawned = 0;
        base.OnStartHost();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        numSpawned--;
        base.OnServerDisconnect(conn);
    }
}
