using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    private static NetworkManager manager;
    private static List<GameObject> playersAlive;
    private static bool had1Player = false;
    private static bool had2Players = false;
    private static bool restarting = false;

	// Use this for initialization
	void Start () {
        playersAlive = new List<GameObject>();
        manager = GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //print(playersAlive.Count);
        if (!had1Player) {
            return;
        }
        if ((had2Players && playersAlive.Count <= 1) || playersAlive.Count == 0) {
            if (!restarting) {
                restarting = true;
                StartCoroutine(restartScene(2));
            }
        }
	}

    public static void AddNewPlayer(GameObject newPlayer) {
        playersAlive.Add(newPlayer);
        print("Adding player");
        if (!had1Player) {
            had1Player = true;
        }
        if (!had2Players && playersAlive.Count == 2) {
            had2Players = true;
        }
        
    }

    public static void RemovePlayer(GameObject deadPlayer) {
        playersAlive.Remove(deadPlayer);
    }

    public static int ReturnPlayers(){
        return playersAlive.Count;
    }

    private static IEnumerator restartScene(float secondsUntilRestart) {
        yield return new WaitForSeconds(secondsUntilRestart);
        had1Player = false;
        had2Players = false;
        restarting = false;
        manager.ServerChangeScene("JeffsTestScene");
    }
}
