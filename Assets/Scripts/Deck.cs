using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class Deck : NetworkBehaviour {
    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;
    const int NUM_QUEUES = 5;

    Stack<Card> deck;
    public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
        deck = new Stack<Card>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [Command]
    public void CmdStartGame()
    {
        for (int i = ACE; i <= KING; i++) //create cards and put them in the deck
        {
            GameObject card = (GameObject)Instantiate(cardPrefab, this.transform);
            card.GetComponent<Card>().setNum(i);
            card.GetComponent<Card>().setSuit(Card.Suit.DIAMONDS);
            NetworkServer.Spawn(card);
            deck.Push(card.GetComponent<Card>());

            card = (GameObject)Instantiate(cardPrefab, this.transform);
            card.GetComponent<Card>().setNum(i);
            card.GetComponent<Card>().setSuit(Card.Suit.CLUBS);
            NetworkServer.Spawn(card);
            deck.Push(card.GetComponent<Card>());

            card = (GameObject)Instantiate(cardPrefab, this.transform);
            card.GetComponent<Card>().setNum(i);
            card.GetComponent<Card>().setSuit(Card.Suit.HEARTS);
            NetworkServer.Spawn(card);
            deck.Push(card.GetComponent<Card>());

            card = (GameObject)Instantiate(cardPrefab, this.transform);
            card.GetComponent<Card>().setNum(i);
            card.GetComponent<Card>().setSuit(Card.Suit.SPADES);
            NetworkServer.Spawn(card);
            deck.Push(card.GetComponent<Card>());
        }

        shuffle();
        shuffle(); //shuffle twice for randomness
    }

    public void addCard(Card c) //add a card to the deck
    {
        deck.Push(c);
    }

    public Card drawCard() //draw a card from the deck
    {
        return deck.Pop();
    }

    public int size()
    {
        return deck.Count;
    }

    public void shuffle()
    {
        Queue<Card>[] queues = new Queue<Card>[NUM_QUEUES]; //queues hold cards while shuffling
        for(int i = 0; i < NUM_QUEUES; i++)
        {
            queues[i] = new Queue<Card>();
        }

        System.Random rand = new System.Random();
        while (deck.Count > 0) //randomly distribute deck into 5 queues
        {
            int ind = rand.Next(queues.Length);
            queues[ind].Enqueue(deck.Pop());
        }

        int queuesInUse = queues.Length; //tracks number of nonempty queues
        while (queues[0] != null) //while at least one queue is nonempty
        {
            int ind = rand.Next(queuesInUse); //randomly pick a queue from which to pull
            if (queues[ind].Count > 0) deck.Push(queues[ind].Dequeue());
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
    }
}
