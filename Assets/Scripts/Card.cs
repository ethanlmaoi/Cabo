using System.Collections;

public class Card {

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    public enum Suit { DIAMONDS, CLUBS, HEARTS, SPADES }

    int num;
    Suit suit;

	public Card(int number, Suit s)
    {
        num = number;
        suit = s;
    }

    public int getNum()
    {
        return num;
    }

    public Suit getSuit()
    {
        return suit;
    }

    public int getValue()
    {
        if((suit == Suit.DIAMONDS || suit == Suit.HEARTS) && num == KING)
        {
            return -1;
        }
        else
        {
            return num;
        }
    }
}
