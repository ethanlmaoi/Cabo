using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutCard : MonoBehaviour {

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    public enum Suit { DIAMONDS, CLUBS, HEARTS, SPADES }
    [SerializeField] int num;
    [SerializeField] Suit suit;
    [SerializeField] bool isFlipped;

    [SerializeField] Vector3 moveTarget;
    float moveSpeed = 20f;

    // Use this for initialization
    void Start ()
    {
        // nothing
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (moveTarget != Vector3.zero)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, moveTarget, moveSpeed * Time.deltaTime);
            if (this.transform.position == moveTarget) moveTarget = Vector3.zero;
        }
    }

    public void setMoveTarget(Vector3 target)
    {
        moveTarget = target;
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
        if ((suit == Suit.DIAMONDS || suit == Suit.HEARTS) && num == KING) //red kings are worth -1
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

    public void flipUp() //flips card up for discard or peeking or knowledgeable swapping
    {
        isFlipped = true;
    }

    public void flipDown() //flips card facedown after peeking or moving
    {
        isFlipped = false;
    }

    public void highlightCard()
    {
        gameObject.GetComponent<AssetRenderer>().highlightCard();
    }

    public void removeHighlightCard()
    {
        gameObject.GetComponent<AssetRenderer>().removeHighlightCard();
    }

    public string toString()
    {
        return num + " of " + suit;
    }
}
