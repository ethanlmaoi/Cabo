using UnityEngine;
using System.Collections;

public class HandCard : MonoBehaviour {
    
    Card card; //the card at this spot in the hand
    PlayerScript owner; //the player to whom this hand card belongs

    public void setOwner(PlayerScript player)
    {
        owner = player;
    }

    public PlayerScript getOwner()
    {
        return owner;
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
