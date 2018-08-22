using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkerHUD : MonoBehaviour {

    NetworkManager manager;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();
	}
	
	public void startGame()
    {
        manager.StartHost();
    }

    public void joinGame()
    {
        manager.StartClient();
    }
}
