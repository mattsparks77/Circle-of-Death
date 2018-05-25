using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombItem : NetworkBehaviour {

	bool pickedUp = false;
	Collider[] hits;
	float explosionForce = 1500f;
	float radius = 10f;
	float up = 12f;
	GameObject parent;
	public float stunTime = 0.75f;
	PlayerController pc;

	void OnTriggerEnter(Collider collider){
		if (collider.tag == "Player" && !pickedUp){
			pc = collider.GetComponent<PlayerController>();
			if (!pc.hasBomb){
				GetComponent<MeshRenderer>().enabled = false;
				transform.parent = collider.transform;
				parent = transform.parent.gameObject;
				pickedUp = true;
				pc.SetBomb(true);
				Destroy(GetComponent<BoxCollider>());
			}
        }
	}
	
	[ClientRpc]
	public void RpcDropBomb(){
		GetComponent<MeshRenderer>().enabled = true;
		transform.parent = null;
		pc.SetBomb(false);
		Invoke("RpcExplode", 2.5f);
	}

	[ClientRpc]
	void RpcExplode(){
		Vector3 pos = transform.position;
		hits = Physics.OverlapSphere(pos, 4f);
		foreach(Collider hit in hits){
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null && rb.gameObject != parent){
				PlayerController pc = rb.gameObject.GetComponent<PlayerController>();
				if (pc != null)
					pc.StunPlayer(stunTime);
				rb.AddExplosionForce(explosionForce, pos, radius, up);
				
				transform.parent = null;
				
			}
			
		}
		Destroy(this.gameObject);
	}
}
