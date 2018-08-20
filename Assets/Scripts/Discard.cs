using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Discard : NetworkBehaviour {

    const float CARD_HEIGHT_DIFF = -0.1f;

    Stack<Card> discard;

	// Use this for initialization
	void Start () {
        discard = new Stack<Card>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addCard(GameObject card)
    {
        if(isServer) RpcAddCard(card);
    }

    [ClientRpc]
    public void RpcAddCard(GameObject card) //add card to discard pile
    {
        discard.Push(card.GetComponent<Card>());
        card.GetComponent<Card>().setMoveTarget(this.transform.position + new Vector3(0, 0, discard.Count * CARD_HEIGHT_DIFF));
    }

    public Card peekTop() //check top card in case of doubles
    {
        return discard.Peek();
    }

    public void popCard()
    {
        if(isServer) RpcPopCard();
    }

    [ClientRpc]
    public void RpcPopCard()
    {
        discard.Pop();
    }
    
    public void shuffleIntoDeck(Deck deck) //shuffle discard cards back into deck
    {
        if(!isServer)
        {
            Debug.Log("nonserver discard trying to shuffle into deck");
            return;
        }
        while(discard.Count > 0)
        {
            deck.addToShuffleDeck(peekTop().gameObject);
            RpcPopCard();
        }
        deck.shuffle(); //shouldn't need to check if done shuffling because this call is not asynchronous
        deck.deckCards();
    }
}
