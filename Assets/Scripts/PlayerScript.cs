using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScript : NetworkBehaviour {

    const int HAND_MAX = 6;
    enum Modes { SPAWN, BEGIN, DRAW, TURN, PEEK, SWAP, WAITING, DOUBLING, REPLACING, CAMBRIO };
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
    Vector3 activeCardPos;

    HandCard swapSpot1; //first handcard involved in swapping
    HandCard swapSpot2; //second handcard involved in swapping

    // ETHAN: added these variables to keep track of the GameObject cards we flipped/highlighted in exeBegin(RaycastHit) method
    private HandCard hc1;
    private HandCard hc2;

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
        activeCardPos = new Vector3(6, 0, 0);

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
                        StartCoroutine(deck.deckCards());
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
                // ETHAN: added animation that flips the selected FIRST card
                hc1 = hit.transform.GetComponent<HandCard>();
                hc1.getCard().toggleCard();
                Debug.Log("flipping first card: " + hc1.getCard().toString());

                chosenCards++;
                Debug.Log(chosenCards);
            }
            else if (chosenCards == 1 && hit.transform.GetComponent<HandCard>() != hc1)
            {
                // ETHAN: added animation that flips the selected SECOND card
                hc2 = hit.transform.GetComponent<HandCard>();
                hc2.getCard().toggleCard();
                Debug.Log("flipping second card: " + hc2.getCard().toString());

                // ETHAN: added animation that flips back the selected two cards (after flipBack() delay)
                StartCoroutine(flipBack());
                Debug.Log("flipped back: " + hc1.getCard().toString() + " and " + hc2.getCard().toString());

                chosenCards = 0;
                this.unhighlightHand();
                CmdUpdateMode(Modes.WAITING);
                Debug.Log("waiting");
            }
        }
    }

    // ETHAN: function that helps flip back the selected two cards with a delay
    IEnumerator flipBack()
    {
        yield return new WaitForSeconds(3);
        if (hc1 != null && hc1.getCard() != null) hc1.getCard().flipDown(); //guarantees it always flip down and not up
        if (hc2 != null && hc2.getCard() != null) hc2.getCard().flipDown();

        hc1 = null;
        hc2 = null;
    }

    void exeDraw(RaycastHit hit)
    {
        if (hit.transform.tag == "Deck")
        {
            activeCard = deck.peekTop();
            this.CmdMoveToActiveCard(activeCard.gameObject);
            this.CmdPopDeck();

            // ETHAN: added animation that draws card and flips it to reveal to player
            activeCard.GetComponent<AssetRenderer>().drawCard();
            activeCard.toggleCard();

            Debug.Log("drew " + activeCard.toString());
            CmdUpdateMode(Modes.TURN);
            deck.unhighlightDeck();
            discard.highlightDiscard();
            this.highlightHand();
            Debug.Log("turn");
        }
        else if (hit.transform.tag == "Discard" && discard.peekTop() != null)
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            deck.unhighlightDeck();
            control.highlightAllPlayerCards();
            Debug.Log("doubling");
        }
        else if (hit.transform.tag == "Cambrio" && !control.cambrioIsCalled())
        {
            this.CmdCallCambrio();
            Debug.Log("cambrio");
        }
    }

    void exeTurn(RaycastHit hit)
    {
        if (hit.transform.tag == "Discard")
        {
            Debug.Log("discarded " + activeCard.toString());
            this.CmdDiscardCard(activeCard.gameObject);
            discard.unhighlightDiscard();
            this.unhighlightHand();

            // ETHAN: added animation puts card into discard pile
            activeCard.GetComponent<AssetRenderer>().discardCard();

            if (activeCard.getNum() == PEEK_SELF_7 || activeCard.getNum() == PEEK_SELF_8)
            {
                CmdUpdateMode(Modes.PEEK);
                this.highlightHand();
                Debug.Log("peek self");
                peekingSelf = true;

            }
            else if (activeCard.getNum() == PEEK_OTHER_9 || activeCard.getNum() == PEEK_OTHER_10)
            {
                CmdUpdateMode(Modes.PEEK);
                control.highlightOtherPlayerCards();
                Debug.Log("peek other");
                peekingSelf = false;
            }
            else if (activeCard.getNum() == BLIND_SWAP_J || activeCard.getNum() == BLIND_SWAP_Q)
            {
                CmdUpdateMode(Modes.SWAP);
                this.highlightHand();
                Debug.Log("blind swap");
                pickingSelfForSwap = true;
                swapIsBlind = true;
            }
            else if (activeCard.getNum() == SEE_SWAP_K)
            {
                CmdUpdateMode(Modes.SWAP);
                this.highlightHand();
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
            activeCard.flipDown();

            this.CmdDiscardCard(oldCard.gameObject);
            this.CmdFinishTurn();
            discard.unhighlightDiscard();
            this.unhighlightHand();
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
                hc1 = hit.transform.GetComponent<HandCard>();
                hc1.getCard().flipUp();
                StartCoroutine(flipBack());

                this.CmdFinishTurn();
                if (peekingSelf) this.unhighlightHand();
                else control.unhighlightOtherPlayerCards();
                Debug.Log("waiting");
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            control.highlightAllPlayerCards();
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
                this.unhighlightHand();
                control.highlightOtherPlayerCards();
            }
            else if (!pickingSelfForSwap && hit.transform.GetComponent<HandCard>().getOwner() != this &&
                !hit.transform.GetComponent<HandCard>().getOwner().isCambrio())
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
                control.unhighlightOtherPlayerCards();
                this.CmdFinishTurn();
                Debug.Log("waiting");
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            control.highlightAllPlayerCards();
            Debug.Log("doubling");
        }
    }

    void exeWaiting(RaycastHit hit)
    {
        if (hit.transform.tag == "Discard" && discard.peekTop() != null)
        {
            oldMode = mode;
            CmdUpdateMode(Modes.DOUBLING);
            control.highlightAllPlayerCards();
            Debug.Log("doubling");
        }
    }

    void exeDoubling(RaycastHit hit)
    {
        if (hit.transform.tag == "HandCard" && hit.transform.GetComponent<HandCard>().getCard() != null &&
            !hit.transform.GetComponent<HandCard>().getOwner().isCambrio())
        {
            control.unhighlightAllPlayerCards();
            doubleSpot = hit.transform.GetComponent<HandCard>(); //need to remember the spot across calls
            Card doubleCard = doubleSpot.getCard();
            if (doubleCard.getNum() == discard.peekTop().getNum())
            {
                this.CmdDiscardCard(doubleCard.gameObject);
                int handInd = doubleSpot.getOwner().findHandCard(doubleSpot);
                doubleSpot.getOwner().CmdSetCard(handInd, null); //remove card from handcard
                if (doubleSpot.getOwner() != this)
                {
                    CmdUpdateMode(Modes.REPLACING);
                    this.highlightHand();
                    Debug.Log("replacing");
                }
                else
                {
                    CmdUpdateMode(oldMode);
                    revertHighlight();
                }
            }
            else //double incorrect, add top of discard to player's empty spot
            {
                hc1 = doubleSpot;
                doubleCard.flipUp();
                StartCoroutine(flipBack());
                int moveDestInd = FindEmptyHandCard();
                if (moveDestInd == -1)
                {
                    //lose
                }
                else
                {
                    discard.peekTop().flipDown();
                    this.CmdSetCard(moveDestInd, discard.peekTop().gameObject);
                    this.CmdPopDiscard();
                    CmdUpdateMode(oldMode);
                    revertHighlight();
                }
            }
        }
        else if (hit.transform.tag == "Discard")
        {
            CmdUpdateMode(oldMode);
            control.unhighlightAllPlayerCards();
            revertHighlight();
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
                this.unhighlightHand();
                revertHighlight();
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
        if(this.isLocalPlayer) this.highlightHand();
        Debug.Log("begin");
    }

    public bool isWaiting()
    {
        return mode == Modes.WAITING;
    }

    public bool isCambrio()
    {
        return mode == Modes.CAMBRIO;
    }

    [ClientRpc]
    public void RpcStartTurn()
    {
        mode = Modes.DRAW;
        if (this.isLocalPlayer) deck.highlightDeck();
        Debug.Log("draw");
    }

    [Command]
    public void CmdPopDeck()
    {
        deck.popCard();
    }

    [Command]
    public void CmdMoveToActiveCard(GameObject card)
    {
        RpcMoveToActiveCard(card);
    }

    [ClientRpc]
    public void RpcMoveToActiveCard(GameObject card)
    {
        card.GetComponent<Card>().setMoveTarget(activeCardPos);
    }

    [Command]
    public void CmdDiscardCard(GameObject card)
    {
        discard.addCard(card);
    }

    [Command]
    public void CmdPopDiscard()
    {
        discard.popCard();
    }

    [Command]
    public void CmdFinishTurn()
    {
        mode = Modes.WAITING;
        control.nextPlayerTurn();
    }

    [Command]
    public void CmdCallCambrio()
    {
        control.callCambrio();
        mode = Modes.CAMBRIO;
        control.nextPlayerTurn();
    }

    public int getScore()
    {
        int score = 0;
        for(int i = 0; i < hand.Length; i++)
        {
            if (hand[i].getCard() != null) score += hand[i].getCard().getValue();
        }
        return score;
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
            card.GetComponent<Card>().setMoveTarget(hand[handInd].transform.position);
            card.GetComponent<AssetRenderer>().replaceCard(); // plays animation to shrink back card
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
        card.setMoveTarget(hand[handInd].transform.position);
    }

    public void highlightHand()
    {
        for(int i = 0; i < hand.Length; i++)
        {
            if (hand[i].getCard() != null) hand[i].getCard().highlightCard();
        }
    }

    public void unhighlightHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            if (hand[i].getCard() != null) hand[i].getCard().removeHighlightCard();
        }
    }

    public void revertHighlight()
    {
        switch (oldMode)
        {
            case Modes.DRAW:
                deck.highlightDeck();
                break;
            case Modes.PEEK:
                if (peekingSelf) this.highlightHand();
                else control.highlightOtherPlayerCards();
                break;
            case Modes.SWAP:
                if (pickingSelfForSwap) this.highlightHand();
                else control.highlightOtherPlayerCards();
                break;
            case Modes.WAITING:
                //nothing is highlighted when waiting
                break;
        }
    }
}
