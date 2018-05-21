using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class BombItem : MonoBehaviour {

	bool pickedUp = false;
	Collider[] hits;
	float explosionForce = 1500f;
	float radius = 10f;
	float up = 2f;
	GameObject parent;

	void OnTriggerEnter(Collider collider){
		if (collider.tag == "Player" && !pickedUp){
			GetComponent<MeshRenderer>().enabled = false;
			transform.parent = collider.transform;
			parent = transform.parent.gameObject;
			pickedUp = true;
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.E) && pickedUp){
			transform.position = transform.parent.position;
			GetComponent<MeshRenderer>().enabled = true;
			transform.parent = null;
			Invoke("Explode", 2.5f);
		}
	}

	void Explode(){
		Vector3 pos = transform.position;
		hits = Physics.OverlapSphere(pos, 4f);
		foreach(Collider hit in hits){
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null && rb.gameObject != parent){
				rb.AddExplosionForce(explosionForce, pos, radius, up);
				transform.parent = null;
				Destroy(this.gameObject);
			}
		}
	}
}
