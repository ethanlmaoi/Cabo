using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class Deck : NetworkBehaviour {
    const int ACE = 1;
    const int KING = 13;
    const int NUM_QUEUES = 5;
    const int OFFSCREEN_OFFSET = 20;
    const int TIMES_TO_SHUFFLE = 3;
    const float CARD_HEIGHT_DIFF = -0.1f;
    const float MOVE_DELAY = 0.05f;
    
    Stack<Card> deck;
    public GameObject cardPrefab;

    Stack<GameObject> shuffleDeck; //shuffling looks bad onscreen so do it offscreen
    Vector3 offscreenPosition;

    public GameObject highlightPrefab;
    GameObject highlight;

    [SyncVar]
    bool doneShuffling;
    [SyncVar]
    bool deckIsReady;

	// Use this for initialization
	void Start () {
        deck = new Stack<Card>();
        shuffleDeck = new Stack<GameObject>();
        offscreenPosition = this.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0);

        if (isServer)
        {
            deckIsReady = false;
            for (int i = ACE; i <= KING; i++) //create cards and put them in the deck
            {
                GameObject card = (GameObject)Instantiate(cardPrefab, offscreenPosition, this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.DIAMONDS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab, offscreenPosition, this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.CLUBS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab, offscreenPosition, this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.HEARTS);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);

                card = (GameObject)Instantiate(cardPrefab, offscreenPosition, this.transform.rotation);
                card.GetComponent<Card>().setNum(i);
                card.GetComponent<Card>().setSuit(Card.Suit.SPADES);
                NetworkServer.Spawn(card);
                shuffleDeck.Push(card);
            }

            shuffle();
            GameObject.FindGameObjectWithTag("GameStarter").GetComponentInChildren<TextMesh>().text = "Start Game";
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
    public void setDeckNotReady()
    {
        deckIsReady = false;
    }
    
    public IEnumerator deckCards()
    {
        if(!isServer)
        {
            yield break;
        }
        if (!doneShuffling)
        {
            yield return new WaitUntil(() => doneShuffling);
        }
        while(shuffleDeck.Count > 0)
        {
            RpcAddCard(shuffleDeck.Pop());
            yield return new WaitForSeconds(MOVE_DELAY); //adds a delay between each card being decked
        }
    }

    [ClientRpc]
    public void RpcAddCard(GameObject card) //add a card to the deck
    {
        deck.Push(card.GetComponent<Card>());
        card.GetComponent<Card>().setMoveTarget(this.transform.position + new Vector3(0, 0, deck.Count * CARD_HEIGHT_DIFF));
        if(isServer && shuffleDeck.Count == 0)
        {
            deckIsReady = true;
            GameObject gameStarter = GameObject.FindGameObjectWithTag("GameStarter");
            if(gameStarter != null) gameStarter.SetActive(false);
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
        System.Random rand = new System.Random();

        for (int i = 0; i < TIMES_TO_SHUFFLE; i++)
        {
            for (int q = 0; q < NUM_QUEUES; q++)
            {
                queues[q] = new Queue<GameObject>();
            }

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
                    for (int j = ind + 1; j < queues.Length; j++)
                    {
                        queues[j - 1] = queues[j];
                        queues[j] = null;
                    }
                    queuesInUse--; //decrement nonempty queue tracker
                }
            }
        }

        doneShuffling = true;
    }

    public void addToShuffleDeck(GameObject card)
    {
        shuffleDeck.Push(card);
    }

    public void highlightDeck()
    {
        if (highlight != null) Destroy(highlight);
        highlight = Instantiate(highlightPrefab, this.transform.position, this.transform.rotation);
    }

    public void unhighlightDeck()
    {
        if(highlight != null) Destroy(highlight);
    }
}
