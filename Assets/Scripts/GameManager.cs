using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    NetworkManager manager;
    private static List<GameObject> playersAlive;

	// Use this for initialization
	void Start () {
        playersAlive = new List<GameObject>();
        manager = GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //print(playersAlive.Count);
	}

    public static void AddNewPlayer(GameObject newPlayer) {
        playersAlive.Add(newPlayer);
    }

    public static void RemovePlayer(GameObject deadPlayer) {
        playersAlive.Remove(deadPlayer);
    }
}
