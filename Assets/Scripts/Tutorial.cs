using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] GameObject cardPrefab;

    ArrayList myCards, hisCards;
    Vector3 target;
    enum Modes { WELCOME, BEGINNING, GO_PEEK, YOU_CHOSE, DRAW, STACK, REPLACE, TECHNIQUES };
    [SerializeField] Modes currentMode;

    [SerializeField] GameObject tut_1_welcome;
    [SerializeField] GameObject tut_2_beginning;
    [SerializeField] GameObject tut_3_goPeek;
    [SerializeField] GameObject tut_4_youChose;
    [SerializeField] GameObject tut_5_draw;
    [SerializeField] GameObject tut_6_stack;
    [SerializeField] GameObject tut_65_stack;
    [SerializeField] GameObject tut_675_stack;
    [SerializeField] GameObject tut_7_replace;
    [SerializeField] GameObject tut_8_techniques;
    [SerializeField] GameObject green_arrow;

    const int ACE = 1;
    const int JACK = 11;
    const int QUEEN = 12;
    const int KING = 13;

    bool firstCardFlipped;
    bool secondCardFlipped;
    bool drewCard;
    bool discarded;
    bool discardMode;

    Vector3 activeCardPos = new Vector3(5.75f, 0, 0);

    void Start ()
    {
        myCards = new ArrayList();
        hisCards = new ArrayList();

        target = new Vector3(-2.55f, -3.5f, 0f); // HANDCARD 1 (MY CARDS)
        cardPrefab.tag = "4HEARTS";
        myCards.Add("4HEARTS");
        cardPrefab.GetComponent<TutCard>().setNum(4);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-0.85f, -3.5f, 0f); // HANDCARD 2
        cardPrefab.tag = "6SPADES";
        myCards.Add("6SPADES");
        cardPrefab.GetComponent<TutCard>().setNum(6);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.SPADES);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(0.85f, -3.5f, 0f); // HANDCARD 3
        cardPrefab.tag = "8DIAMONDS";
        myCards.Add("8DIAMONDS");
        cardPrefab.GetComponent<TutCard>().setNum(8);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(2.55f, -3.5f, 0f); // HANDCARD 4
        cardPrefab.tag = "10CLUBS";
        myCards.Add("10CLUBS");
        cardPrefab.GetComponent<TutCard>().setNum(10);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.CLUBS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-2.55f, 3.5f, 0f); // his cards
        cardPrefab.tag = "1HEARTS";
        hisCards.Add("1HEARTS");
        cardPrefab.GetComponent<TutCard>().setNum(1);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-0.85f, 3.5f, 0f);
        cardPrefab.tag = "3SPADES";
        hisCards.Add("3SPADES");
        cardPrefab.GetComponent<TutCard>().setNum(3);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.SPADES);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(0.85f, 3.5f, 0f);
        cardPrefab.tag = "5DIAMONDS";
        hisCards.Add("5DIAMONDS");
        cardPrefab.GetComponent<TutCard>().setNum(5);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(2.55f, 3.5f, 0f);
        cardPrefab.tag = "7CLUBS";
        hisCards.Add("7CLUBS");
        cardPrefab.GetComponent<TutCard>().setNum(7);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.CLUBS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-1.17f, -0.05f, 0f); // DECK POSITION (LAST)
        cardPrefab.tag = "7HEARTS";
        cardPrefab.GetComponent<TutCard>().setNum(7);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-1.17f, -0.05f, -0.01f); // DECK POSITION
        cardPrefab.tag = "KDIAMONDS";
        cardPrefab.GetComponent<TutCard>().setNum(KING);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-1.17f, -0.05f, -0.02f); // DECK POSITION (FIRST)
        cardPrefab.tag = "4DIAMONDS";
        cardPrefab.GetComponent<TutCard>().setNum(4);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        currentMode = Modes.WELCOME;
        GameObject.Instantiate(tut_1_welcome);
    }
	
	void FixedUpdate ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                switch (currentMode)
                {
                    case Modes.WELCOME:
                        exeWelcome(hit);
                        break;
                    case Modes.BEGINNING:
                        exeBeginning(hit);
                        break;
                    case Modes.GO_PEEK:
                        exePeek(hit);
                        break;
                    case Modes.YOU_CHOSE:
                        exeYouChose(hit);
                        break;
                    case Modes.DRAW:
                        exeDraw(hit);
                        break;
                    case Modes.STACK:
                        exeStack(hit);
                        break;
                    case Modes.REPLACE:
                        exeReplace(hit);
                        break;
                }
            }
        }
	}

    void exeWelcome(RaycastHit hit)
    {
        Destroy(GameObject.Find("tut_1_welcome(Clone)"));
        Instantiate(tut_2_beginning);
        updateMode(Modes.BEGINNING);
    }

    void exeBeginning(RaycastHit hit)
    {
        Destroy(GameObject.Find("tut_2_beginning(Clone)"));
        Instantiate(tut_3_goPeek);
        GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().highlightCard();
        GameObject.FindGameObjectWithTag("6SPADES").GetComponent<TutCard>().highlightCard();
        Instantiate(green_arrow, new Vector3(-2.55f, -1.6f, -2f), Quaternion.identity); // creates green arrow for first card
        Instantiate(green_arrow, new Vector3(-0.87f, -1.6f, -2f), Quaternion.identity); // creates green arrow for second card
        updateMode(Modes.GO_PEEK);
    }

    void exePeek(RaycastHit hit)
    {
        if (hit.transform.tag == "4HEARTS")
        {
            firstCardFlipped = true;
            GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().flipUp();
        }
        if (hit.transform.tag == "6SPADES")
        {
            secondCardFlipped = true;
            GameObject.FindGameObjectWithTag("6SPADES").GetComponent<TutCard>().flipUp();
        }

        if (firstCardFlipped && secondCardFlipped) // checks if both cards are flipped
        {
            StartCoroutine(flipCardsDown());
            GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().removeHighlightCard();
            GameObject.FindGameObjectWithTag("6SPADES").GetComponent<TutCard>().removeHighlightCard();
            GameObject[] greenArrows = GameObject.FindGameObjectsWithTag("greenArrow");
            Destroy(greenArrows[0]);
            Destroy(greenArrows[1]);
            Destroy(GameObject.Find("tut_3_goPeek(Clone)"));
            Instantiate(tut_4_youChose);
            updateMode(Modes.YOU_CHOSE);
        }
    }

    IEnumerator flipCardsDown()
    {
        yield return new WaitForSeconds(2);
        GameObject.FindGameObjectWithTag("6SPADES").GetComponent<TutCard>().flipDown();
        GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().flipDown();
    }

    void exeYouChose(RaycastHit hit)
    {
        Destroy(GameObject.Find("tut_4_youChose(Clone)"));
        Instantiate(tut_5_draw);
        Instantiate(green_arrow, new Vector3(-1.2f, 1.79f, -2f), Quaternion.identity);
        GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().highlightCard();
        updateMode(Modes.DRAW);
    }

    void exeDraw(RaycastHit hit)
    {
        if (hit.transform.tag == "Deck")
        {
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().setMoveTarget(activeCardPos);
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutAssetRenderer>().drawCard();
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().flipUp();
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().removeHighlightCard();
            Destroy(GameObject.FindGameObjectWithTag("greenArrow"));
            Instantiate(green_arrow, new Vector3(1.13f, 1.79f, -2f), Quaternion.identity);
            Destroy(GameObject.Find("tut_5_draw(Clone)"));
            Instantiate(tut_6_stack);
            updateMode(Modes.STACK);
        }
    }

    void exeStack(RaycastHit hit)
    {
        if (hit.transform.tag == "Discard")
        {
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().setMoveTarget(new Vector3(1.155f, -0.05f, 0f));
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutAssetRenderer>().discardCard();
            GameObject.FindGameObjectWithTag("4DIAMONDS").GetComponent<TutCard>().flipDown();
            Destroy(GameObject.Find("tut_6_stack(Clone)"));
            Instantiate(tut_65_stack);
            discarded = true;
        }
        else if (hit.transform.tag == "Discard" && discarded)
        {
            foreach (string tag in myCards)
            {
                GameObject.FindGameObjectWithTag(tag).GetComponent<TutCard>().highlightCard();
            }

            Destroy(GameObject.FindGameObjectWithTag("greenArrow"));
            Instantiate(green_arrow, new Vector3(0f, 0f, -2f), Quaternion.identity);
            Destroy(GameObject.Find("tut_6.5_stack(Clone)"));
            Instantiate(tut_675_stack);
            discardMode = true;
        }

        if (hit.transform.tag == "4HEARTS" && discarded && discardMode)
        {
            GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().setMoveTarget(new Vector3(1.155f, -0.05f, 0f));
            GameObject.FindGameObjectWithTag("4HEARTS").GetComponent<TutCard>().flipUp();
            Destroy(GameObject.Find("tut_6.75_stack(Clone)"));
            Instantiate(tut_7_replace);
            updateMode(Modes.REPLACE);
        }
    }

    void exeReplace(RaycastHit hit)
    {

    }

    void updateMode(Modes newMode)
    {
        currentMode = newMode;
    }
}
