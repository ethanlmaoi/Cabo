using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    float speed = 30.0f;
    Transform target;

    const float HAND_CARD_1_X_POS = -2.55f;
    const float HAND_CARD_2_X_POS = -0.85f;
    const float HAND_CARD_3_X_POS = 0.85f;
    const float HAND_CARD_4_X_POS = 2.55f;
    const float HAND_CARD_Y_POS = -3.5f;
    const float DEFAULT_Z_POS = 0.0f;

    bool cardDrawing;
    bool discardingCard;
    bool replacingCard;

    [SerializeField] GameObject highlightPrefab;
    GameObject highlight;

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
        // gameObject.GetComponent<NetworkAnimator>().animator = GetComponent<Animator>();

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

        if (cardDrawing)
        {
            float step = speed * Time.deltaTime;
            StartCoroutine(scaleOverTime(0.8f));
        }

        if (discardingCard)
        {
            float step = speed * Time.deltaTime;
            StartCoroutine(discardDescaleOverTime(0.5f));
        }
        if (replacingCard)
        {
            float step = speed * Time.deltaTime;
            StartCoroutine(replaceDescaleOverTime(0.5f));
        }

        if(highlight != null && highlight.transform.position != this.transform.position)
        {
            highlight.transform.position = this.transform.position;
        }
    }

    IEnumerator scaleOverTime(float time)
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(2f, 2f, 2f);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        cardDrawing = false;
    }

    IEnumerator discardDescaleOverTime(float time)
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(0.5f, 0.5f, 0.5f);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }

    IEnumerator replaceDescaleOverTime(float time)
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(0.5f, 0.5f, 0.5f);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        replacingCard = true;
    }

    public void highlightCard()
    {
        if (highlight != null) Destroy(highlight);
        highlight = Instantiate(highlightPrefab, transform.position, this.transform.rotation);
    }

    public void removeHighlightCard()
    {
        if(highlight != null) GameObject.Destroy(highlight);
    }

    public void toggleCard()
    {
        gameObject.GetComponent<Card>().toggleCard();
    }

    // play animation that reveals drawn card
    public void drawCard()
    {
        cardDrawing = true;
    }

    public void discardCard()
    {
        discardingCard = true;
    }

    public void replaceCard()
    {
        replacingCard = true;
    }
}
