using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class Deck : NetworkBehaviour {
    const int ACE = 1;
    const int KING = 13;
    const int FULL_DECK = 52;
    const int NUM_QUEUES = 5;
    const int OFFSCREEN_OFFSET = 30;
    const int TIMES_TO_SHUFFLE = 2;
    
    Stack<Card> deck;
    public GameObject cardPrefab;

    Stack<GameObject> shuffleDeck; //shuffling looks bad onscreen so do it offscreen

    [SyncVar]
    bool doneShuffling;
    [SyncVar]
    bool deckIsReady;
    int shuffleCount;

	// Use this for initialization
	void Start () {
        deck = new Stack<Card>();

        shuffleDeck = new Stack<GameObject>();
        
        if(isServer)
        {
            deckIsReady = false;
            shuffleCount = 0;
            for (int i = ACE; i <= KING; i++) //create cards and put them in the deck
            {
                GameObject card = (GameObject)Instantiate(cardPrefab,
                    this.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0), this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.DIAMONDS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab,
                    this.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0), this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.CLUBS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab,
                    this.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0), this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.HEARTS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab,
                    this.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0), this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.SPADES);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);
            }

            for (int i = 0; i < TIMES_TO_SHUFFLE; i++)
            {
                shuffle();
            }
        }
	}

    public bool isDoneShuffling()
    {
        return doneShuffling;
    }

    public bool isReady()
    {
        return deckIsReady;
    }

    [Command]
    public void CmdSetIsReady(bool b)
    {
        deckIsReady = b;
    }

    [Command]
    public void CmdDeckCards()
    {
        while(shuffleDeck.Count > 0)
        {
            RpcAddCard(shuffleDeck.Pop());
        }
    }

    [ClientRpc]
    public void RpcAddCard(GameObject card) //add a card to the deck
    {
        Debug.Log("pushing " + card.GetComponent<Card>().toString() + " to the deck");
        deck.Push(card.GetComponent<Card>());
        card.transform.position = this.transform.position;
        if(deck.Count == FULL_DECK && isServer)
        {
            deckIsReady = true;
            GameObject.FindGameObjectWithTag("GameStarter").SetActive(false);
            
        }
    }
    
    public Card peekTop() //return card on top
    {
        return deck.Peek(); //will only be called on server
    }

    public void popCard() //allows clients to tell the server to pop a card across clients
    {
        if (isServer) RpcPopCard();
    }

    [ClientRpc]
    public void RpcPopCard()//draw a card from the deck
    {
        deck.Pop();
    }

    public Card drawCard() //only used in dealing cards at the beginning
    {
        return deck.Pop();
    }

    public int size()
    {
        return deck.Count;
    }
    
    public void shuffle()
    {
        Queue<GameObject>[] queues = new Queue<GameObject>[NUM_QUEUES]; //queues hold cards while shuffling
        for(int i = 0; i < NUM_QUEUES; i++)
        {
            queues[i] = new Queue<GameObject>();
        }

        System.Random rand = new System.Random();
        while (shuffleDeck.Count > 0) //randomly distribute deck into 5 queues
        {
            int ind = rand.Next(queues.Length);
            queues[ind].Enqueue(shuffleDeck.Pop());
        }

        int queuesInUse = queues.Length; //tracks number of nonempty queues
        while (queues[0] != null) //while at least one queue is nonempty
        {
            int ind = rand.Next(queuesInUse); //randomly pick a queue from which to pull
            if (queues[ind].Count > 0) shuffleDeck.Push(queues[ind].Dequeue());
            if (queues[ind].Count == 0) //if the queue is now empty, null it out and move everything down
            {
                queues[ind] = null;
                for(int i = ind + 1; i < queues.Length; i++)
                {
                    queues[i - 1] = queues[i];
                    queues[i] = null;
                }
                queuesInUse--; //decrement nonempty queue tracker
            }
        }

        shuffleCount++;
        if (shuffleCount == TIMES_TO_SHUFFLE)
        {
            doneShuffling = true;
            GameObject.FindGameObjectWithTag("GameStarter").GetComponentInChildren<TextMesh>().text = "Start Game";
        }
    }
}
