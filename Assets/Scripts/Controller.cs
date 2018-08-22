using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Controller : NetworkBehaviour {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;
    const int MAX_HAND_VALUE = 75; //two black kings and four queens + 1
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
    bool cambrioing; //waiting for last cards to be set before calculating score

    [SyncVar]
    bool cambrioCalled;
    [SyncVar]
    int cambrioInd;

    private void Awake()
    {
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
                if(!players[i].isWaiting()) break;
                if(i == numPlayers - 1)
                {
                    beginning = false;
                    nextPlayerTurn();
                    Debug.Log("beginning game");
                }
            }
        }
        if(cambrioing)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                if (players[i].isSettingCard()) break; //wait until all players are done setting cards
                if (i == numPlayers - 1)
                {
                    cambrioing = false;
                    finishGame();
                }
            }
        }
	}
    
    public void addPlayer(PlayerScript p)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                players[i] = p;
                break;
            }
        }
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
        for(int i = 0; i < numPlayers; i++)
        {
            RpcPopulatePlayers(i, players[i].gameObject);
        }
        decking = false;
        StartCoroutine(dealCards());
        beginning = true;
    }

    [ClientRpc]
    public void RpcPopulatePlayers(int pInd, GameObject p)
    {
        players[pInd] = p.GetComponent<PlayerScript>();
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
            cambrioing = true;
        }
        else
        {
            Debug.Log("setting " + players[currPlayerInd].getName() + " to start turn");
            players[currPlayerInd].RpcStartTurn();
        }
        
    }

    public void highlightPlayerCardsExcept(PlayerScript playerToAvoid)
    {
        for(int i = 0; i < numPlayers; i++)
        {
            if (players[i] != null && (playerToAvoid == null || players[i] != playerToAvoid))
            {
                players[i].highlightHand();
            }
        }
    }

    public void unhighlightPlayerCardsExcept(PlayerScript playerToAvoid)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i] != null && (playerToAvoid == null || players[i] != playerToAvoid)) players[i].unhighlightHand();
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

    [ClientRpc]
    public void RpcRevealHands()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null) players[i].revealHand();
        }
    }

    public void finishGame()
    {
        RpcRevealHands();
        int minScore = MAX_HAND_VALUE;
        Queue<int> minIndices = new Queue<int>();
        int score;
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;
            score = players[i].getScore();
            if(score < minScore)
            {
                minScore = score;
                minIndices.Clear();
                minIndices.Enqueue(i);
            }
            else if(score == minScore)
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
