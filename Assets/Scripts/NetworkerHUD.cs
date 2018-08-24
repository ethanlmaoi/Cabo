using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkerHUD : MonoBehaviour {

    const int MATCH_SIZE = 2;
    const int MESSAGE_TIME = 2;

    NetworkManager manager;

    public InputField onlineCreateRoomField;
    public InputField onlineJoinRoomField;
    public InputField localJoinIPField;

    public Text onlineEnterRoomText;
    public Text onlineNoRoomText;
    public Text localEnterIPText;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();
        manager.StartMatchMaker();

        onlineEnterRoomText.enabled = false;
        onlineNoRoomText.enabled = false;
        localEnterIPText.enabled = false;
	}
	
	public void createOnlineGame()
    {
        if(onlineCreateRoomField.text != "")
        {
            manager.matchMaker.CreateMatch(onlineCreateRoomField.text, MATCH_SIZE, true, "", "", "", 0, 0, manager.OnMatchCreate);
        }
        else
        {
            onlineEnterRoomText.enabled = true;
            StartCoroutine(disableText(onlineEnterRoomText));
        }
    }

    public void joinOnlineGame()
    {
        if (onlineJoinRoomField.text != "")
        {
            bool matchJoined = false;
            manager.matchMaker.ListMatches(0, 20, onlineJoinRoomField.text, false, 0, 0, manager.OnMatchList);
            foreach(MatchInfoSnapshot match in manager.matches)
            {
                if(onlineJoinRoomField.text.Equals(match.name))
                {
                    matchJoined = true;
                    manager.matchName = match.name;
                    manager.matchSize = (uint) match.currentSize;
                    manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                }
            }
            if(!matchJoined)
            {
                onlineNoRoomText.enabled = true;
                StartCoroutine(disableText(onlineNoRoomText));
            }
        }
        else
        {
            onlineEnterRoomText.enabled = true;
            StartCoroutine(disableText(onlineEnterRoomText));
        }
    }

    public void createLocalGame()
    {
        manager.StartHost();
    }

    public void joinLocalGame()
    {
        if (localJoinIPField.text != "")
        {
            manager.networkAddress = localJoinIPField.text;
            manager.StartClient();
        }
        else
        {
            localEnterIPText.enabled = true;
            StartCoroutine(disableText(localEnterIPText));
        }
    }

    IEnumerator disableText(Text t)
    {
        yield return new WaitForSeconds(MESSAGE_TIME);
        t.enabled = false;
    }
}
