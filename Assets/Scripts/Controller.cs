using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Controller : NetworkBehaviour {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;
    
    PlayerScript[] players;

    [SyncVar]
    int numPlayers = 0;
    [SyncVar]
    int currPlayerInd = 0;

    public GameObject deckObj;
    Deck deck;
    public GameObject discardObj;
    Discard discard;

    [SyncVar]
    bool decking;
    [SyncVar]
    bool beginning;

	// Use this for initialization
	void Start () {
        players = new PlayerScript[MAX_PLAYERS];
        deck = deckObj.GetComponent<Deck>();
        discard = discardObj.GetComponent<Discard>();
        decking = true;
        beginning = false;
    }
	
	// FixedUpdate is called independent of frame
	void FixedUpdate () {
        if(decking)
        {
            if(deck.isReady())
            {
                CmdStartGame();
                decking = false;
            }
        }
        if(isServer && beginning)
        {
            for(int i = 0; i < numPlayers; i++)
            {
                if(!players[i].isWaiting())
                {
                    break;
                }
                if(i == numPlayers - 1)
                {
                    beginning = false;
                    CmdNextPlayerTurn();
                    Debug.Log("beginning game");
                }
            }
        }
	}
    
    public void addPlayer(PlayerScript p) //only the server needs to have a populated players array
    {
        players[numPlayers] = p;
        numPlayers++;

        if (numPlayers == 1) //DEBUG
        {
            p.setName("Charles");
        }
        else if (numPlayers == 2)
        {
            p.setName("Ethan");
        }
    }

    [Command]
    public void CmdStartGame()
    {
        foreach (PlayerScript player in players)
        {
            if (player == null) continue;
            for (int i = 0; i < STARTING_HAND_SIZE; i++)
            {
                HandCard moveDest = player.FindEmptyHandCard();
                player.addCard(moveDest, deck.peekTop());
                deck.RpcPopCard();
                //TODO animate moving card from deck to moveDest
            }
            player.RpcBegin();
        }
        beginning = true;
    }

    [Command]
    public void CmdNextPlayerTurn()
    {
        if (deck.size() == 0)
        {
            discard.CmdShuffleIntoDeck(deck.gameObject);
        }
        players[currPlayerInd].startTurn();
        currPlayerInd = (currPlayerInd + 1) % numPlayers;
    }
}
