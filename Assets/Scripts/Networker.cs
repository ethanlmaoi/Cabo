using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Networker : NetworkManager {

    int numSpawned = 0;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        numSpawned++;
        Transform[] pos = startPositions.ToArray();
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        for (int i = 0; i < pos.Length; i++)
        {
            if (pos[i].name.Equals("Spawn Position " + numSpawned))
            {
                spawnPos = pos[i].position;
                spawnRot = pos[i].rotation;
            }
        }
        GameObject player = (GameObject)Instantiate(playerPrefab, spawnPos, spawnRot);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().addPlayer(player.GetComponent<PlayerScript>());
    }

    public override void OnStartHost()
    {
        numSpawned = 0;
        base.OnStartHost();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        numSpawned--;
        base.OnServerDisconnect(conn);
    }
}
