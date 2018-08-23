using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkerHUD : MonoBehaviour {

    const int MATCH_SIZE = 2;

    NetworkManager manager;

    public InputField onlineCreateRoomField;
    public InputField onlineJoinRoomField;
    public InputField localCreateRoomField;
    public InputField localJoinRoomField;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();
        manager.StartMatchMaker();
	}
	
	public void createOnlineGame()
    {
        if(onlineCreateRoomField.text != "")
        {
            manager.matchMaker.CreateMatch(onlineCreateRoomField.text, MATCH_SIZE, true, "", "", "", 0, 0, manager.OnMatchCreate);
        }
    }

    public void joinOnlineGame()
    {
        if (onlineJoinRoomField.text != "")
        {
            manager.matchMaker.ListMatches(0, 20, onlineJoinRoomField.text, false, 0, 0, manager.OnMatchList);
            foreach(MatchInfoSnapshot match in manager.matches)
            {
                if(onlineJoinRoomField.text.Equals(match.name))
                {
                    manager.matchName = match.name;
                    manager.matchSize = (uint) match.currentSize;
                    manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                }
            }
        }
    }

    public void createLocalGame()
    {
        manager.StartHost();
    }

    public void joinLocalGame()
    {
        manager.StartClient();
    }
}
