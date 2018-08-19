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
    
    [SyncVar]
    Modes mode;

    [SyncVar]
    public string perName; //debug purposes

    Camera cam;

    public void setName(string n)
    {
        perName = n;
    }

    public string getName()
    {
        return perName;
    }

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
                case "Hand Card 1": // setting Hand Card 1 object to be hand[0]
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
        }

        activeCard = null;
        chosenCards = 0;
        mode = Modes.SPAWN;

        if(this.isLocalPlayer)
        {
            cam = GetComponentInChildren<Camera>();
            cam.enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }

        if(!this.isServer)
        {
            GameObject gameStarter = GameObject.FindGameObjectWithTag("GameStarter");
            if(gameStarter != null)
            {
                gameStarter.SetActive(false);
            }
        }
	}
	
	void FixedUpdate () {
        if (this.isLocalPlayer)
        {
            /*Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began) {
                Ray ray = cam.ScreenPointToRay(touch.position);*/
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if(this.isServer && hit.transform.tag == "GameStarter" && deck.isDoneShuffling())
                    {
                        hit.transform.GetComponentInChildren<TextMesh>().text = "Shuffling";
                        //shuffling is already done, just need to transfer cards from shuffle deck to normal deck
                        deck.CmdDeckCards();
                    }
                    Debug.Log(this.getName() + " hit " + hit.transform + " while in " + mode);
                    switch (mode)
                    {
                        case Modes.BEGIN:
                            exeBegin(hit);
                            break;
                        case Modes.DRAW:
                            exeDraw(hit);
                            break;
                        case Modes.TURN:
                            exeTurn(hit);
                            break;
                        case Modes.PEEK:
                            exePeek(hit);
                            break;
                        case Modes.SWAP:
                            exeSwap(hit);
                            break;
                        case Modes.WAITING:
                            exeWaiting(hit);
                            break;
                        case Modes.DOUBLING:
                            exeDoubling(hit);
                            break;
                        case Modes.REPLACING:
                            exeReplacing(hit);
                            break;
                    }
                }
            }
        }
	}

    void exeBegin(RaycastHit hit)
    {
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
                CmdUpdateMode(Modes.WAITING);
                Debug.Log("waiting");
                //unhighlight own cards
            }
        }
    }

    void exeDraw(RaycastHit hit)
    {
        if (hit.transform.tag == "Deck")
        {
            //play animation of drawing card
            activeCard = deck.peekTop();
            this.CmdPopDeck();
            Debug.Log("drew " + activeCard.toString());
            CmdUpdateMode(Modes.TURN);
            Debug.Log("turn");
        }
        else if (hit.transform.tag == "Discard" && discard.peekTop() != null)
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            Debug.Log("doubling");
        }
    }

    void exeTurn(RaycastHit hit)
    {
        if (hit.transform.tag == "Discard")
        {
            Debug.Log("discarded " + activeCard.toString());
            //play animation moving card to discard pile
            this.CmdDiscardCard(activeCard.gameObject);
            if (activeCard.getNum() == PEEK_SELF_7 || activeCard.getNum() == PEEK_SELF_8)
            {
                CmdUpdateMode(Modes.PEEK);
                Debug.Log("peek self");
                peekingSelf = true;
            }
            else if (activeCard.getNum() == PEEK_OTHER_9 || activeCard.getNum() == PEEK_OTHER_10)
            {
                CmdUpdateMode(Modes.PEEK);
                Debug.Log("peek other");
                peekingSelf = false;
            }
            else if (activeCard.getNum() == BLIND_SWAP_J || activeCard.getNum() == BLIND_SWAP_Q)
            {
                CmdUpdateMode(Modes.SWAP);
                Debug.Log("blind swap");
                pickingSelfForSwap = true;
                swapIsBlind = true;
            }
            else if (activeCard.getNum() == SEE_SWAP_K)
            {
                CmdUpdateMode(Modes.SWAP);
                Debug.Log("see swap");
                pickingSelfForSwap = true;
                swapIsBlind = false;
            }
            else
            {
                activeCard = null;
                this.CmdFinishTurn();
                Debug.Log("waiting");
            }
        }
        else if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null &&
            hit.transform.GetComponent<HandCard>().getOwner() == this)
        {
            Debug.Log("replacing " + hit.transform);
            Card oldCard = hit.transform.GetComponent<HandCard>().getCard();

            int handInd = this.findHandCard(hit.transform.GetComponent<HandCard>());
            this.CmdSetCard(handInd, activeCard.gameObject);
            discard.CmdAddCard(oldCard.gameObject);
            //TODO add animations for replacing card and discarding old card
            this.CmdFinishTurn();
            Debug.Log("waiting");
        }
    }

    void exePeek(RaycastHit hit)
    {
        if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
        {
            if ((peekingSelf && hit.transform.GetComponent<HandCard>().getOwner() == this) ||
                (!peekingSelf && hit.transform.GetComponent<HandCard>().getOwner() != this))
            {
                Card peekCard = hit.transform.GetComponent<HandCard>().getCard();
                Debug.Log("peeked at " + peekCard.toString());
                //TODO play animation of revealing card
                this.CmdFinishTurn();
                Debug.Log("waiting");
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            Debug.Log("doubling");
        }
    }

    void exeSwap(RaycastHit hit)
    {
        if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
        {
            if (pickingSelfForSwap && hit.transform.GetComponent<HandCard>().getOwner() == this)
            {
                Debug.Log("picked " + hit.transform + " first");
                swapSpot1 = hit.transform.GetComponent<HandCard>();
                pickingSelfForSwap = false;
                //TODO picked card and other cards
            }
            else if (!pickingSelfForSwap && hit.transform.GetComponent<HandCard>().getOwner() != this)
            {
                swapSpot2 = hit.transform.GetComponent<HandCard>();
                Debug.Log("picked " + hit.transform + " second");

                Card swap1 = swapSpot1.getCard();
                Card swap2 = swapSpot2.getCard();

                if (!swapIsBlind)
                {
                    
                    Debug.Log("maybe swap " + swap1.toString() + " and " + swap2.toString());
                    //TODO reveal the cards and prompt user if they want to swap
                    //if they do, then do same as in else block
                    //if not, then play animation for putting cards down and do nothing
                }
                else
                {
                    int swapInd1 = swapSpot1.getOwner().findHandCard(swapSpot1);
                    int swapInd2 = swapSpot2.getOwner().findHandCard(swapSpot2);
                    swapSpot1.getOwner().CmdSetCard(swapInd1, swap2.gameObject);
                    swapSpot2.getOwner().CmdSetCard(swapInd2, swap1.gameObject);
                    //TODO play animation
                }
                pickingSelfForSwap = true;
                this.CmdFinishTurn();
                Debug.Log("waiting");
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            Debug.Log("doubling");
        }
    }

    void exeWaiting(RaycastHit hit)
    {
        if (hit.transform.tag == "Discard" && discard.peekTop() != null)
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            Debug.Log("doubling");
        }
    }

    void exeDoubling(RaycastHit hit)
    {
        if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
        {
            doubleSpot = hit.transform.GetComponent<HandCard>(); //need to remember the spot across calls
            Card doubleCard = doubleSpot.getCard();
            if (doubleCard.getNum() == discard.peekTop().getNum())
            {
                discard.CmdAddCard(doubleCard.gameObject);
                int handInd = doubleSpot.getOwner().findHandCard(doubleSpot);
                doubleSpot.getOwner().CmdSetCard(handInd, null); //remove card from handcard
                                          //TODO play animation for moving card from handcard to discard
                if (doubleSpot.getOwner() != this)
                {
                    CmdUpdateMode(Modes.REPLACING);
                    //TODO highlight own cards
                    Debug.Log("replacing");
                }
                else
                {
                    CmdUpdateMode(oldMode);
                }
            }
            else //double incorrect, add top of discard to player's empty spot
            {
                int moveDestInd = FindEmptyHandCard();
                if (moveDestInd == -1)
                {
                    //lose
                }
                else
                {
                    this.CmdSetCard(moveDestInd, discard.peekTop().gameObject);
                    discard.CmdPopCard();
                    Debug.Log("moving to hand card " + moveDestInd);
                    //play animation of moving card from discard to moveDest
                    CmdUpdateMode(oldMode);
                }
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            CmdUpdateMode(oldMode);
            Debug.Log(oldMode);
        }
    }

    void exeReplacing(RaycastHit hit)
    {
        if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null)
        {
            HandCard replacingSpot = hit.transform.GetComponent<HandCard>();
            if (replacingSpot.getOwner() == this)
            {
                int doubleHandInd = doubleSpot.getOwner().findHandCard(doubleSpot);
                int replaceHandInd = this.findHandCard(replacingSpot);
                doubleSpot.getOwner().CmdSetCard(doubleHandInd, replacingSpot.getCard().gameObject);
                this.CmdSetCard(replaceHandInd, null);
                //TODO play animation for moving card from handcard to handcard
                CmdUpdateMode(oldMode);
                //TODO unhighlight own cards
            }
        }
    }

    [Command]
    void CmdUpdateMode(Modes m)
    {
        mode = m;
    }

    [ClientRpc]
    public void RpcBegin()
    {
        mode = Modes.BEGIN;
        Debug.Log("begin");
    }

    public bool isWaiting()
    {
        return mode == Modes.WAITING;
    }

    public void startTurn()
    {
        mode = Modes.DRAW;
        Debug.Log("draw");
    }

    [Command]
    public void CmdPopDeck()
    {
        deck.popCard();
    }

    [Command]
    public void CmdDiscardCard(GameObject card)
    {
        discard.addCard(card);
    }

    [Command]
    public void CmdFinishTurn()
    {
        mode = Modes.WAITING;
        control.nextPlayerTurn();
    }

    public int findHandCard(HandCard hc)
    {
        for(int i = 0; i < hand.Length; i++)
        {
            if (hand[i] == hc) return i;
        }
        return -1;
    }

    public int FindEmptyHandCard()
    {
        Debug.Log("finding");
        for (int i = 0; i < hand.Length; i++)
        {
            if (hand[i].getCard() == null)
            {
                return i;
            }
        }
        return -1;
    }
    
    [Command]
    public void CmdSetCard(int handInd, GameObject card)
    {
        RpcSetCard(handInd, card);
    }

    [ClientRpc]
    public void RpcSetCard(int handInd, GameObject card)
    {
        if (card != null)
        {
            hand[handInd].setCard(card.GetComponent<Card>());
        }
        else
        {
            hand[handInd].setCard(null);
        }
    }

    [ClientRpc]
    public void RpcDealCard(int handInd)
    {
        Card card = deck.drawCard();
        hand[handInd].setCard(card);

        // animation to deal cards
        card.GetComponent<AssetRenderer>().moveCard(card.GetComponent<Card>(), hand[handInd]);
        Debug.Log("RHEEEEEEEEEEE: " + hand[handInd].transform.name);
    }
}
