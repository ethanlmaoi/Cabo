using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] GameObject cardPrefab;

    Vector3 myCard1;
    Vector3 myCard2;
    Vector3 myCard3;
    Vector3 myCard4;

    // Use this for initialization
    void Start ()
    {
        myCard1 = new Vector3(-2.55f, -3.5f, -1);
        cardPrefab.tag = "4HEARTS";
        cardPrefab.GetComponent<TutCard>().setNum(4);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(myCard1);
        GameObject.Instantiate(cardPrefab);

        myCard2 = new Vector3(-0.85f, -3.5f, -1);
        cardPrefab.tag = "6SPADES";
        cardPrefab.GetComponent<TutCard>().setNum(6);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.SPADES);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(myCard2);
        GameObject.Instantiate(cardPrefab);

        myCard2 = new Vector3(0.85f, -3.5f, -1);
        cardPrefab.tag = "8DIAMONDS";
        cardPrefab.GetComponent<TutCard>().setNum(8);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.DIAMONDS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(myCard2);
        GameObject.Instantiate(cardPrefab);

        myCard2 = new Vector3(2.55f, -3.5f, -1);
        cardPrefab.tag = "10CLUBS";
        cardPrefab.GetComponent<TutCard>().setNum(10);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.CLUBS);
        cardPrefab.GetComponent<TutCard>().setMoveTarget(myCard2);
        GameObject.Instantiate(cardPrefab);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
