using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HandCard : NetworkBehaviour {

    public Card card; //the card at this spot in the hand
    PlayerScript owner; //the player to whom this hand card belongs

    public void setOwner(PlayerScript player)
    {
        owner = player;
    }

    public PlayerScript getOwner()
    {
        return owner;
    }

    public Card replaceCard(Card newCard)
    {
        Card oldCard = card;
        card = newCard;
        return oldCard;
    }

    public Card getCard()
    {
        return card;
    }

    public void setCard(Card c)
    {
        card = c;
    }
}
