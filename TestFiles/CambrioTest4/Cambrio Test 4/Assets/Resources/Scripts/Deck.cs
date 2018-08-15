using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Deck : MonoBehaviour {
    const int ACE = 1;
    const int KING = 13;
    const int NUM_QUEUES = 5;

    Stack<Card> deck;
    GameObject card;

	// Use this for initialization
	void Start () {
        deck = new Stack<Card>();

        for(int i = ACE; i <= KING; i++) //create cards and put them in the deck
        {
            deck.Push(new Card(i, Card.Suit.DIAMONDS));
            deck.Push(new Card(i, Card.Suit.CLUBS));
            deck.Push(new Card(i, Card.Suit.HEARTS));
            deck.Push(new Card(i, Card.Suit.SPADES));
        }

        shuffle();
        shuffle(); //shuffle twice for randomness
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addCard(Card c) //add a card to the deck
    {
        deck.Push(c);
    }

    public Card drawCard() //draw a card from the deck
    {
        return deck.Pop();
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
            deck.Push(queues[ind].Dequeue());
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
