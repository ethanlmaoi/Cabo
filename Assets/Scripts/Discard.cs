using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Discard : NetworkBehaviour {

    Stack<Card> discard;

	// Use this for initialization
	void Start () {
        discard = new Stack<Card>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [Command]
    public void CmdAddCard(GameObject card) //allows clients to tell other clients to discard a card
    {
        RpcAddCard(card);
    }

    [ClientRpc]
    public void RpcAddCard(GameObject card) //add card to discard pile
    {
        discard.Push(card.GetComponent<Card>());
    }

    public Card peekTop() //check top card in case of doubles
    {
        return discard.Peek();
    }

    [Command]
    public void CmdPopCard()
    {
        RpcPopCard();
    }

    [ClientRpc]
    public void RpcPopCard()
    {
        discard.Pop();
    }

    [Command]
    public void CmdShuffleIntoDeck(GameObject d) //shuffle discard cards back into deck
    {
        Deck deck = d.GetComponent<Deck>();
        while(discard.Count > 0)
        {
            deck.RpcAddCard(peekTop().gameObject);
            RpcPopCard();
        }
        deck.shuffle(); //TODO shuffle only concerned with beginning of game, this not working yet
    }
}
