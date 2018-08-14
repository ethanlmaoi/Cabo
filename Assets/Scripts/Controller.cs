using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Controller : NetworkManager {

    const int MAX_PLAYERS = 4;
    const int STARTING_HAND_SIZE = 4;

    int numSpawned = 0;
    PlayerScript[] players;
    int currPlayerInd = 0;

    Deck deck;
    Discard discard;

	// Use this for initialization
	void Start () {
        players = new PlayerScript[MAX_PLAYERS];
        deck = GameObject.FindGameObjectWithTag("DecK").GetComponent<Deck>();
        discard = GameObject.FindGameObjectWithTag("Discard").GetComponent<Discard>();
	}
	
	// FixedUpdate is called independent of frame
	void FixedUpdate () {

	}

    public void startGame()
    {
        foreach (PlayerScript player in players)
        {
            for(int i = 0; i < STARTING_HAND_SIZE; i++)
            {
                HandCard moveDest = player.addCard(deck.drawCard());
            }
            player.begin();
        }
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

    public override void OnStopServer()
    {
        numSpawned = 0;
        base.OnStopServer();
    }
}
