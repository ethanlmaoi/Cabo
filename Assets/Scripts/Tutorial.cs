using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] GameObject cardPrefab;

	// Use this for initialization
	void Start ()
    {
        cardPrefab.GetComponent<TutCard>().setNum(4);
        cardPrefab.GetComponent<TutCard>().setSuit(TutCard.Suit.HEARTS);
        GameObject.Instantiate(cardPrefab);

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
