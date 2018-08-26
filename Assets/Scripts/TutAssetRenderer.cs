using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutAssetRenderer : MonoBehaviour
{

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    string num;
    string suit;
    string fileName;

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
        if (gameObject.GetComponent<TutCard>().getNum() == ACE)
        {
            num = "ace";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 2)
        {
            num = "2";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 3)
        {
            num = "3";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 4)
        {
            num = "4";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 5)
        {
            num = "5";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 6)
        {
            num = "6";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 7)
        {
            num = "7";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 8)
        {
            num = "8";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 9)
        {
            num = "9";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == 10)
        {
            num = "10";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == JACK)
        {
            num = "jack";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == QUEEN)
        {
            num = "queen";
        }
        else if (gameObject.GetComponent<TutCard>().getNum() == KING)
        {
            num = "king";
        }

        // initialize suit names
        if (gameObject.GetComponent<TutCard>().getSuit() == TutCard.Suit.DIAMONDS)
        {
            suit = "diamonds";
        }
        else if (gameObject.GetComponent<TutCard>().getSuit() == TutCard.Suit.SPADES)
        {
            suit = "spades";
        }
        else if (gameObject.GetComponent<TutCard>().getSuit() == TutCard.Suit.HEARTS)
        {
            suit = "hearts";
        }
        else if (gameObject.GetComponent<TutCard>().getSuit() == TutCard.Suit.CLUBS)
        {
            suit = "clubs";
        }

        // combine
        fileName = num + "_of_" + suit;

        // load the sprite and the animator controller
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + fileName);
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/" + fileName);
        // gameObject.GetComponent<NetworkAnimator>().animator = GetComponent<Animator>();
    }

    void Update()
    {
        // toggleCard method (DEBUG PURPOSES)
        if (gameObject.GetComponent<TutCard>().checkFlipped()) // flips card up if true
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", true);
        }
        else // flips card down if false
        {
            gameObject.GetComponent<Animator>().SetBool("flippedUp", false);
        }

        if (cardDrawing)
        {
            StartCoroutine(scaleOverTime(0.8f));
        }

        if (discardingCard)
        {
            StartCoroutine(discardDescaleOverTime(0.5f));
        }
        if (replacingCard)
        {
            StartCoroutine(replaceDescaleOverTime(0.5f));
        }

        if (highlight != null && highlight.transform.position != this.transform.position)
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
        discardingCard = false;
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
        replacingCard = false;
    }

    public void highlightCard()
    {
        if (highlight != null) Destroy(highlight);
        highlight = Instantiate(highlightPrefab, transform.position, this.transform.rotation);
    }

    public void removeHighlightCard()
    {
        if (highlight != null) GameObject.Destroy(highlight);
    }

    public void toggleCard()
    {
        gameObject.GetComponent<TutCard>().toggleCard();
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
