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
    float speed = 10.0f;
    Transform target;

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
        if (gameObject.GetComponent<Card>().checkFlipped()) // flips card up if true
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", true);
        }
        else // flips card down if false
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", false);
        }

        // moves the card if player clicks
        if (moveToHandCard1) //&& transform.position != new Vector3(-2.55f, 1.5f, 0f))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(-2.55f, 3.5f, 0f), step);
        }
        else if (moveToHandCard2)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(-0.85f, 3.5f, 0f), step);
        }
        else if (moveToHandCard3)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0.85f, 3.5f, 0f), step);
        }
        else if (moveToHandCard4)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(2.55f, 3.5f, 0f), step);
        }
    }

    // player left click
    void OnMouseDown()
    {
        gameObject.GetComponent<Card>().toggleCard();
        moveCard();
    }

    void moveCard()
    {
        move = true;
    }

    public void moveCard(Card card, HandCard hc)
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
