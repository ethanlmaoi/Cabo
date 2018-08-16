using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Card : NetworkBehaviour {

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    public enum Suit { DIAMONDS, CLUBS, HEARTS, SPADES }

    int num;
    Suit suit;
    bool isFlipped;

    public Card(int number, Suit s) // why remove?
    {
        num = number;
        suit = s;
    }

    public void setNum(int n)
    {
        num = n;
    }

    public void setSuit(Suit s)
    {
        suit = s;
    }

    public int getNum() //return number, not value, of this card
    {
        return num;
    }

    public Suit getSuit() //return suit of this card
    {
        return suit;
    }

    public int getValue() //return value, not number, of this cardy
    {
        if((suit == Suit.DIAMONDS || suit == Suit.HEARTS) && num == KING) //red kings are worth -1
        {
            return -1;
        }
        else
        {
            return num; //otherwise, normal value
        }
    }

    public bool checkFlipped() // true if flipped up, false if flipped down
    {
        return isFlipped;
    }

    public void toggleCard() // flips up if down, flips down if up
    {
        isFlipped = !isFlipped;
    }

    public string toString()
    {
        return num + " of " + suit;
    }
}
