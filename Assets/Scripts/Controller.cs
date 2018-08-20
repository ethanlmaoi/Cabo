using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Controller : NetworkBehaviour {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;
    const int MAX_HAND_VALUE = 74; //two black kings and four queens
    const float MOVE_DELAY = 0.05f;
    
    PlayerScript[] players;

    [SyncVar]
    int numPlayers = 0;
    [SyncVar]
    int currPlayerInd = -1;

    public GameObject deckObj;
    Deck deck;
    public GameObject discardObj;
    Discard discard;

    [SyncVar]
    bool decking;
    [SyncVar]
    bool beginning;

    [SyncVar]
    bool cambrioCalled;
    [SyncVar]
    int cambrioInd;

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
        if(!isServer)
        {
            return;
        }
        if (decking && deck.isReady() && deck.peekTop().transform.position.x == deck.transform.position.x)
        {
            CmdStartGame();
        }
        if(beginning)
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
                    nextPlayerTurn();
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
        decking = false;
        StartCoroutine(dealCards());
        beginning = true;
    }

    IEnumerator dealCards()
    {
        foreach (PlayerScript player in players)
        {
            if (player == null) continue;
            for (int i = 0; i < STARTING_HAND_SIZE; i++)
            {
                player.RpcDealCard(i);
                yield return new WaitForSeconds(MOVE_DELAY); //adds a delay between each card being dealt
            }
            player.RpcBegin();
        }
        
    }

    public void nextPlayerTurn()
    {
        if(!isServer)
        {
            Debug.Log("nonserver controller trying to start next player turn");
            return;
        }
        if (deck.size() == 0)
        {
            Debug.Log("shuffling discard into deck");
            discard.shuffleIntoDeck(deck);
        }
        currPlayerInd = (currPlayerInd + 1) % numPlayers;
        if (cambrioCalled && currPlayerInd == cambrioInd)
        {
            finishGame();
        }
        else
        {
            Debug.Log("setting " + players[currPlayerInd].getName() + " to start turn");
            players[currPlayerInd].startTurn();
        }
        
    }

    public bool cambrioIsCalled()
    {
        return cambrioCalled;
    }

    public void callCambrio()
    {
        cambrioCalled = true;
        cambrioInd = currPlayerInd;
    }

    public void finishGame()
    {
        int minScore = MAX_HAND_VALUE;
        Queue<int> minIndices = new Queue<int>();
        int score;
        for(int i = 0; i < numPlayers; i++)
        {
            score = players[i].getScore();
            if(score < minScore)
            {
                minScore = score;
                minIndices.Clear();
                minIndices.Enqueue(i);
            }
            if(score == minScore)
            {
                minIndices.Enqueue(i);
            }
        }
        if (minIndices.Count > 1)
        {
            while (minIndices.Count > 1)
            {
                Debug.Log(players[minIndices.Dequeue()].getName() + " and ");
            }
            Debug.Log(players[minIndices.Dequeue()].getName() + " tie with " + minScore);
        }
        else
        {
            Debug.Log(players[minIndices.Dequeue()].getName() + " wins with " + minScore);
        }
    }
}
