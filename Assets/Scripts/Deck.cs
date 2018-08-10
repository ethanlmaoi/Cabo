using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Deck : MonoBehaviour {

    const int ACE = 1;
    const int KING = 13;
    const int NUM_QUEUES = 4;

    Stack<Card> deck;

	// Use this for initialization
	void Start () {
        deck = new Stack<Card>();

        for(int i = ACE; i <= KING; i++)
        {
            deck.Push(new Card(i, Card.Suit.DIAMONDS));
            deck.Push(new Card(i, Card.Suit.CLUBS));
            deck.Push(new Card(i, Card.Suit.HEARTS));
            deck.Push(new Card(i, Card.Suit.SPADES));
        }

        shuffle();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addCard(Card c)
    {
        deck.Push(c);
    }

    public Card drawCard()
    {
        return deck.Pop();
    }

    public void shuffle()
    {
        Queue<Card>[] queues = new Queue<Card>[NUM_QUEUES];
        for(int i = 0; i < NUM_QUEUES; i++)
        {
            queues[i] = new Queue<Card>();
        }

        int currQueue = 0;
        while (deck.Count > 0)
        {
            queues[currQueue].Enqueue(deck.Pop());
            currQueue++;
            if(currQueue == queues.Length)
            {
                currQueue = 0;
            }
        }

        System.Random rand = new System.Random();
        int queuesInUse = queues.Length;
        while (queues[0] != null)
        {
            int ind = rand.Next(queuesInUse);
            deck.Push(queues[ind].Dequeue());
            if (queues[ind].Count == 0)
            {
                queues[ind] = null;
                for(int i = ind + 1; i < queues.Length; i++)
                {
                    queues[i - 1] = queues[i];
                    queues[i] = null;
                }
                queuesInUse--;
            }
        }
    }
}
