using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Discard : NetworkBehaviour {

    const float CARD_HEIGHT_DIFF = -0.1f;
    const int OFFSCREEN_OFFSET = 20;

    Stack<Card> discard;

    public GameObject highlightPrefab;
    GameObject highlight;

    // Use this for initialization
    void Start () {
        discard = new Stack<Card>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int size()
    {
        return discard.Count;
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
        card.GetComponent<Card>().flipUp();
    }

    public Card peekTop() //check top card in case of doubles
    {
        return discard.Peek();
    }

    public void popCard()
    {
        discard.Pop();
    }

    [ClientRpc]
    public void RpcPopCard()
    {
        discard.Pop();
    }
    
    [ClientRpc]
    public void RpcShuffleIntoDeck(GameObject d) //shuffle discard cards back into deck
    {
        if(!isServer)
        {
            return;
        }
        Deck deck = d.GetComponent<Deck>();
        while(discard.Count > 0)
        {
            if (isServer)
            {
                deck.addToShuffleDeck(peekTop().gameObject);
            }
            peekTop().setMoveTarget(d.transform.position + new Vector3(OFFSCREEN_OFFSET, 0, 0));
            peekTop().flipDown();
            popCard();
        }
        if (isServer)
        {
            deck.shuffle(); //shouldn't need to check if done shuffling because this call is not asynchronous
            StartCoroutine(deck.deckCards());
        }
    }

    public void highlightDiscard()
    {
        if (highlight != null) Destroy(highlight);
        highlight = Instantiate(highlightPrefab, this.transform.position, this.transform.rotation);
    }

    public void unhighlightDiscard()
    {
        if(highlight != null) Destroy(highlight);
    }
}
