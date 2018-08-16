using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Controller : NetworkBehaviour {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;
    
    PlayerScript[] players;
    int numPlayers = 0;
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
            for(int i = 0; i < numPlayers; i++)
            {
                if(!players[i].isWaiting())
                {
                    break;
                }
                if(i == numPlayers - 1)
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
            currPlayerInd = (currPlayerInd + 1) % numPlayers;
            nextPlayer = false;
        }
	}

    public void addPlayer(PlayerScript p)
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
            player.RpcBegin();
        }
    }

    public void nextPlayerTurn()
    {
        nextPlayer = true;
    }
}
