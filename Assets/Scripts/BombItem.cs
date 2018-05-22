using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombItem : NetworkBehaviour {

	bool pickedUp = false;
	Collider[] hits;
	float explosionForce = 1500f;
	float radius = 10f;
	float up = 10f;
	GameObject parent;
	public float stunTime = 0.75f;

	void OnTriggerEnter(Collider collider){
		if (collider.tag == "Player" && !pickedUp){
			GetComponent<MeshRenderer>().enabled = false;
			transform.parent = collider.transform;
			parent = transform.parent.gameObject;
			pickedUp = true;
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.E) && pickedUp && parent.GetComponent<PlayerController>().enabled){
			CmdDropBomb();
		}
	}
	[Command]
	void CmdDropBomb(){
		RpcDropBomb();
	}
	[ClientRpc]
	void RpcDropBomb(){
		transform.position = transform.parent.position;
		GetComponent<MeshRenderer>().enabled = true;
		transform.parent = null;
		Invoke("CmdExplode", 2.5f);
	}

	[Command]
	void CmdExplode(){
		RpcExplode();
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
				Destroy(this.gameObject);
			}
		}
	}
}
