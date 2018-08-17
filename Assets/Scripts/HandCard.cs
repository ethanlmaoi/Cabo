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

    public Card getCard()
    {
        return card;
    }

    [Command]
    public void CmdSetCard(GameObject c)
    {
        RpcSetCard(c);
    }
    
    [ClientRpc]
    public void RpcSetCard(GameObject c)
    {
        Debug.Log(owner.getName() + ": " + this + " set card to " + c.GetComponent<Card>().toString());
        card = c.GetComponent<Card>();
    }
}
