using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetRenderer : MonoBehaviour {

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    int deckHeight = 0;
    int discardHeight = 0;

    string num;
    string suit;
    string fileName;

    bool move = false;
    float speed = 15.0f;
    Transform target;

    const float HAND_CARD_1_X_POS = -2.55f;
    const float HAND_CARD_2_X_POS = -0.85f;
    const float HAND_CARD_3_X_POS = 0.85f;
    const float HAND_CARD_4_X_POS = 2.55f;
    const float HAND_CARD_Y_POS = -3.5f;
    const float DEFAULT_Z_POS = 0.0f;

    bool moveToHandCard1;
    bool moveToHandCard2;
    bool moveToHandCard3;
    bool moveToHandCard4;

    void Start()
    {
        // initialize number names (append strings to find files more efficiently)
        if (gameObject.GetComponent<Card>().getNum() == ACE)
        {
            num = "ace";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 2)
        {
            num = "2";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 3)
        {
            num = "3";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 4)
        {
            num = "4";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 5)
        {
            num = "5";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 6)
        {
            num = "6";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 7)
        {
            num = "7";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 8)
        {
            num = "8";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 9)
        {
            num = "9";
        }
        else if (gameObject.GetComponent<Card>().getNum() == 10)
        {
            num = "10";
        }
        else if (gameObject.GetComponent<Card>().getNum() == JACK)
        {
            num = "jack";
        }
        else if (gameObject.GetComponent<Card>().getNum() == QUEEN)
        {
            num = "queen";
        }
        else if (gameObject.GetComponent<Card>().getNum() == KING)
        {
            num = "king";
        }

        // initialize suit names
        if (gameObject.GetComponent<Card>().getSuit() == Card.Suit.DIAMONDS)
        {
            suit = "diamonds";
        }
        else if (gameObject.GetComponent<Card>().getSuit() == Card.Suit.SPADES)
        {
            suit = "spades";
        }
        else if (gameObject.GetComponent<Card>().getSuit() == Card.Suit.HEARTS)
        {
            suit = "hearts";
        }
        else if (gameObject.GetComponent<Card>().getSuit() == Card.Suit.CLUBS)
        {
            suit = "clubs";
        }

        // combine
        fileName = num + "_of_" + suit;

        // load the sprite and the animator controller
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + fileName);
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/" + fileName);

        // moveObject stuff
        target = GameObject.FindGameObjectWithTag("Deck").GetComponent<Transform>();
    }

    void Update()
    {
        // toggleCard method (DEBUG PURPOSES)
        if (gameObject.GetComponent<Card>().checkFlipped()) // flips card up if true
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", true);
        }
        else // flips card down if false
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", false);
        }

        // dealHandCard method
        if (moveToHandCard1 && transform.position != new Vector3(HAND_CARD_1_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(HAND_CARD_1_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS), step);
        }
        else if (moveToHandCard2 && transform.position != new Vector3(HAND_CARD_2_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(HAND_CARD_2_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS), step);
        }
        else if (moveToHandCard3 && transform.position != new Vector3(HAND_CARD_3_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(HAND_CARD_3_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS), step);
        }
        else if (moveToHandCard4 && transform.position != new Vector3(HAND_CARD_4_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(HAND_CARD_4_X_POS, HAND_CARD_Y_POS, DEFAULT_Z_POS), step);
        }
        else
        {
            moveToHandCard1 = false;
            moveToHandCard2 = false;
            moveToHandCard3 = false;
            moveToHandCard4 = false;
        }
    }

    public void toggleCard()
    {
        gameObject.GetComponent<Card>().toggleCard();
    }

    // play animation that deals hand cards to the player
    public void dealHandCard(Card card, HandCard hc)
    {
        if (hc.transform.name == "Hand Card 1")
        {
            moveToHandCard1 = true;
        }
        else if (hc.transform.name == "Hand Card 2")
        {
            moveToHandCard2 = true;
        }
        else if (hc.transform.name == "Hand Card 3")
        {
            moveToHandCard3 = true;
        }
        else if (hc.transform.name == "Hand Card 4")
        {
            moveToHandCard4 = true;
        }
    }


}
