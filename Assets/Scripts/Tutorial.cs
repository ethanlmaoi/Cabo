using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] GameObject cardPrefab;

    Vector3 target;
    enum Modes { WELCOME, BEGINNING, GO_PEEK, YOU_CHOSE, DRAW, STACK, REPLACE };
    [SerializeField] Modes currentMode;

    [SerializeField] GameObject tut_1_welcome;
    [SerializeField] GameObject tut_2_beginning;
    [SerializeField] GameObject tut_3_goPeek;

    void Start ()
    {
        ArrayList myCards = new ArrayList();

        target = new Vector3(-2.55f, -3.5f, -1);
        cardPrefab.tag = "4HEARTS";
        myCards.Add("4HEARTS");
        cardPrefab.GetComponent<TutCard>().setNum(4);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(-0.85f, -3.5f, -1);
        cardPrefab.tag = "6SPADES";
        myCards.Add("6SPADES");
        cardPrefab.GetComponent<TutCard>().setNum(6);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.SPADES);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(0.85f, -3.5f, -1);
        cardPrefab.tag = "8DIAMONDS";
        myCards.Add("8DIAMONDS");
        cardPrefab.GetComponent<TutCard>().setNum(8);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        target = new Vector3(2.55f, -3.5f, -1);
        cardPrefab.tag = "10CLUBS";
        myCards.Add("10CLUBS");
        cardPrefab.GetComponent<TutCard>().setNum(10);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.CLUBS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(target);
        GameObject.Instantiate(cardPrefab);

        foreach (string tag in myCards)
        {
            GameObject.FindGameObjectWithTag(tag).GetComponent<TutCard>().highlightCard();
        }

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
        updateMode(Modes.GO_PEEK);
    }

    void updateMode(Modes newMode)
    {
        currentMode = newMode;
    }
}
