using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Discard : NetworkBehaviour {

    Stack<Card> discard;
    Deck deck;

	// Use this for initialization
	void Start () {
        discard = new Stack<Card>();
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addCard(Card c) //add card to discard pile
    {
        discard.Push(c);
    }

    public Card checkTop() //check top card in case of doubles
    {
        return discard.Peek();
    }

    public Card drawCard()
    {
        return discard.Pop();
    }

    public void shuffleIntoDeck() //shuffle discard cards back into deck
    {
        while(discard.Count > 0)
        {
            deck.addCard(discard.Pop());
        }
        deck.shuffle();
    }
}
