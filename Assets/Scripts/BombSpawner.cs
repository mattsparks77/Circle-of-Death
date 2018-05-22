using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class BombSpawner : NetworkBehaviour {

	public GameObject BombPrefab;
	
	void Awake(){
		if (isServer)
			InvokeRepeating("RpcSpawn", 3.5f, 7f);
	}

	[ClientRpc]
	void RpcSpawn(){
		Bounds bd= GetComponent<Collider>().bounds;
		Vector3 spawnPos = new Vector3(Random.Range(bd.min.x, bd.max.x), transform.position.y + 0.5f, Random.Range(bd.min.z, bd.max.z));
		Quaternion rot = new Quaternion(-28.38f, -18.4f, 48.049f, 1f);
		GameObject bombClone = Instantiate(BombPrefab, spawnPos, rot);
		NetworkServer.Spawn(bombClone);
	}

}
