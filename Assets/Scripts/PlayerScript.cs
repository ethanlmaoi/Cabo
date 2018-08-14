using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScript : NetworkBehaviour {

    const int HAND_MAX = 6;
    enum Modes { SPAWN, BEGIN, DRAW, TURN, PEEK, SWAP, WAITING, DOUBLING, REPLACING };
    const int PEEK_SELF_7 = 7;
    const int PEEK_SELF_8 = 8;
    const int PEEK_OTHER_9 = 9;
    const int PEEK_OTHER_10 = 10;
    const int BLIND_SWAP_J = 11;
    const int BLIND_SWAP_Q = 12;
    const int SEE_SWAP_K = 13;

    Controller control;
    Deck deck;
    Discard discard;

    HandCard[] hand;
    Card activeCard;

    HandCard swapSpot1; //first handcard involved in swapping
    HandCard swapSpot2; //second handcard involved in swapping

    HandCard doubleSpot;
    Modes oldMode; //mode player was in before tapping discard pile to double

    bool peekingSelf; //7 and 8 peek self, 9 and 10 peek others
    bool pickingSelfForSwap; //swapping has two phases, picking your own card, then picking somebody else's card
    bool swapIsBlind; //J and Q are blind swap, K is seeing swap

    int chosenCards;
    Modes mode;

	// Use this for initialization
	void Start () {
        control = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
        discard = GameObject.FindGameObjectWithTag("Discard").GetComponent<Discard>();

        hand = new HandCard[HAND_MAX];
        HandCard[] hcScripts = GetComponentsInChildren<HandCard>();
        foreach(HandCard hc in hcScripts)
        {
            int ind = -1;
            switch(hc.transform.name)
            {
                case "Hand Card 1":
                    ind = 0;
                    break;
                case "Hand Card 2":
                    ind = 1;
                    break;
                case "Hand Card 3":
                    ind = 2;
                    break;
                case "Hand Card 4":
                    ind = 3;
                    break;
                case "Extra Card 1":
                    ind = 4;
                    break;
                case "Extra Card 2":
                    ind = 5;
                    break;
            }
            hand[ind] = hc;
        }

        for(int i = 0; i < HAND_MAX; i++)
        {
            hand[i].setOwner(this);

            //test block - delete after testing
            hand[i].setCard(new Card(1, Card.Suit.SPADES));
        }

        activeCard = null;
        chosenCards = 0;
        mode = Modes.SPAWN;
        Debug.Log("begin");
	}
	
	void FixedUpdate () {
        if (this.isLocalPlayer)
        {
            /*Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began) {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);*/
                if(Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    switch (mode)
                    {
                        case Modes.BEGIN:
                            if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getOwner() == this 
                                && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                if (chosenCards == 0)
                                {
                                    //reveal card
                                    chosenCards++;
                                    Debug.Log(chosenCards);
                                }
                                else if (chosenCards == 1)
                                {
                                    //reveal card
                                    chosenCards = 0;
                                    mode = Modes.WAITING;
                                    Debug.Log("waiting");
                                    //unhighlight own cards
                                }
                            }
                            break;
                        case Modes.DRAW:
                            if (hit.transform.tag == "Deck")
                            {
                                //play animation of drawing card
                                activeCard = deck.drawCard();
                                mode = Modes.TURN;
                                Debug.Log("turn");
                            }
                            else if (hit.transform.tag == "Discard")
                            {
                                oldMode = mode;
                                mode = Modes.DOUBLING;
                                Debug.Log("doubling");
                            }
                            break;
                        case Modes.TURN:
                            if (hit.transform.tag == "Discard")
                            {
                                //play animation moving card to discard pile
                                discard.addCard(activeCard);
                                if(activeCard.getNum() == PEEK_SELF_7 || activeCard.getNum() == PEEK_SELF_8)
                                {
                                    mode = Modes.PEEK;
                                    peekingSelf = true;
                                }
                                else if (activeCard.getNum() == PEEK_OTHER_9 || activeCard.getNum() == PEEK_OTHER_10)
                                {
                                    mode = Modes.PEEK;
                                    peekingSelf = false;
                                }
                                else if(activeCard.getNum() == BLIND_SWAP_J || activeCard.getNum() == BLIND_SWAP_Q)
                                {
                                    mode = Modes.SWAP;
                                    pickingSelfForSwap = true;
                                    swapIsBlind = true;
                                }
                                else if(activeCard.getNum() == SEE_SWAP_K)
                                {
                                    mode = Modes.SWAP;
                                    pickingSelfForSwap = true;
                                    swapIsBlind = false;
                                }
                                activeCard = null;
                            }
                            else if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                if (hit.transform.GetComponent<HandCard>().getOwner() == this)
                                {
                                    Card oldCard = hit.transform.GetComponent<HandCard>().replaceCard(activeCard);
                                    discard.addCard(oldCard);
                                    //TODO add animations for replacing card and discarding old card
                                    mode = Modes.WAITING;
                                    Debug.Log("waiting");
                                }
                            }
                            break;
                        case Modes.PEEK:
                            if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                if( (peekingSelf && hit.transform.GetComponent<HandCard>().getOwner() == this) ||
                                    (!peekingSelf && hit.transform.GetComponent<HandCard>().getOwner() != this) )
                                {
                                    Card peekCard = hit.transform.GetComponent<HandCard>().getCard();
                                    //TODO play animation of revealing card
                                    mode = Modes.WAITING;
                                }
                            }
                            else if (hit.transform.tag == "Discard")
                            {
                                oldMode = mode;
                                mode = Modes.DOUBLING;
                            }
                            break;
                        case Modes.SWAP:
                            if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                if(pickingSelfForSwap && hit.transform.GetComponent<HandCard>().getOwner() == this)
                                {
                                    swapSpot1 = hit.transform.GetComponent<HandCard>();
                                    pickingSelfForSwap = false;
                                    //TODO picked card and other cards
                                }
                                else if(!pickingSelfForSwap && hit.transform.GetComponent<HandCard>().getOwner() != this)
                                {
                                    swapSpot2 = hit.transform.GetComponent<HandCard>();
                                    if(!swapIsBlind)
                                    {
                                        Card swap1 = swapSpot1.getCard();
                                        Card swap2 = swapSpot2.getCard();
                                        //TODO reveal the cards and prompt user if they want to swap
                                        //if they do, then do same as in else block
                                        //if not, then play animation for putting cards down and do nothing
                                    }
                                    else
                                    {
                                        Card swapCard = swapSpot2.replaceCard(swapSpot1.getCard());
                                        swapSpot1.setCard(swapCard);
                                        //TODO play animation
                                    }
                                    pickingSelfForSwap = true;
                                    mode = Modes.WAITING;
                                }
                            }
                            else if (hit.transform.tag == "Discard")
                            {
                                oldMode = mode;
                                mode = Modes.DOUBLING;
                            }
                            break;
                        case Modes.WAITING:
                            if (hit.transform.tag == "Discard")
                            {
                                oldMode = mode;
                                mode = Modes.DOUBLING;
                                Debug.Log("doubling");
                            }
                            break;
                        case Modes.DOUBLING:
                            if(hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                doubleSpot = hit.transform.GetComponent<HandCard>(); //need to remember the spot across calls
                                Card doubleCard = doubleSpot.getCard();
                                if (doubleCard.getNum() == discard.checkTop().getNum())
                                {
                                    discard.addCard(doubleCard);
                                    doubleSpot.setCard(null); //remove card from handcard
                                    //TODO play animation for moving card from handcard to discard
                                    if(doubleSpot.getOwner() != this)
                                    {
                                        mode = Modes.REPLACING;
                                        //TODO highlight own cards
                                        Debug.Log("replacing");
                                    }
                                    else
                                    {
                                        mode = oldMode;
                                    }
                                }
                                else
                                {
                                    HandCard moveDest = addCard(discard.drawCard());
                                    //play animation of moving card from discard to moveDest
                                    mode = oldMode;
                                }
                            }
                            else if(hit.transform.tag == "Discard")
                            {
                                mode = oldMode;
                                Debug.Log(oldMode);
                            }
                            break;
                        case Modes.REPLACING:
                            if(hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
                            {
                                HandCard replacingSpot = hit.transform.GetComponent<HandCard>();
                                if (replacingSpot.getOwner() == this)
                                {
                                    doubleSpot.setCard(replacingSpot.getCard());
                                    replacingSpot.setCard(null);
                                    //TODO play animation for moving card from handcard to handcard
                                    mode = oldMode;
                                    //TODO unhighlight own cards
                                }
                            }
                            break;
                    }
                }
            }
        }
	}

    public void begin()
    {
        mode = Modes.BEGIN;
    }

    public bool isWaiting()
    {
        return mode == Modes.WAITING;
    }

    public void startTurn()
    {
        mode = Modes.DRAW;
    }

    public HandCard addCard(Card card)
    {
        for(int i = 0; i < hand.Length; i++)
        {
            if(hand[i].getCard() == null)
            {
                hand[i].setCard(card);
                return hand[i];
            }
        }

        return null;
    }
}
