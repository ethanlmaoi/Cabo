using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Controller : NetworkBehaviour {

    const int MAX_PLAYERS = 2;
    const int STARTING_HAND_SIZE = 4;
    const int MAX_HAND_VALUE = 75; //two black kings and four queens + 1
    const float MOVE_DELAY = 0.05f;
    
    public GameObject quitGame;

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
        quitGame.SetActive(false);
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
            decking = false;
        }
        if(beginning)
        {
            for(int i = 0; i < numPlayers; i++)
            {
                if (players[i] == null) continue;
                if (!players[i].isWaiting()) break;
                if(i == numPlayers - 1)
                {
                    beginning = false;
                    nextPlayerTurn();
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
    
    [Command]
    public void CmdStartGame()
    {
        RpcPopulatePlayers(GameObject.FindGameObjectWithTag("Networker").GetComponent<Networker>().getPlayers());
    }

    [ClientRpc]
    public void RpcPopulatePlayers(GameObject[] p)
    {
        for (int pInd = 0; pInd < p.Length; pInd++)
        {
            if (p[pInd] != null)
            {
                players[pInd] = p[pInd].GetComponent<PlayerScript>();

                if (isServer) {
                    numPlayers++;
                    if (numPlayers == GameObject.FindGameObjectWithTag("Networker").GetComponent<Networker>().getNumSpawned())
                    {
                        StartCoroutine(dealCards());
                    }
                }
            }
        }
    }

    IEnumerator dealCards()
    {
        beginning = true;
        Debug.Log("beginning game");
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
            return;
        }
        currPlayerInd = (currPlayerInd + 1) % numPlayers;
        if (cambrioCalled && currPlayerInd == cambrioInd)
        {
            cambrioing = true;
        }
        else
        {
            if (deck.size() == 0) {
                deck.setDeckNotReady();
                discard.RpcShuffleIntoDeck(deck.gameObject);
            }

            if (players[currPlayerInd].isOut()) //skip players who are out
            {
                int i = (currPlayerInd + 1) % numPlayers;
                while(i != currPlayerInd)
                {
                    if(!players[i].isOut())
                    {
                        currPlayerInd = i - 1;
                        nextPlayerTurn();
                        return;
                    }
                    i = (i + 1) & numPlayers;
                }
                cambrioing = true;
            }
            else
            {
                players[currPlayerInd].RpcStartTurn();
            }
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
        
        quitGame.SetActive(true);
    }
}
