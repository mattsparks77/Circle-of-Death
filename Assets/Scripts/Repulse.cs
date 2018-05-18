using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulse : MonoBehaviour {
	public float force = 1000f;
	public Collider[] hits;
	public float radius = 10f;

	public float charge;
	public float upModifier = 2f;
	// Use this for initialization
	void Start () {
		
	}
	void toRepulse()
	{
		Vector3 pos = transform.position;
		hits = Physics.OverlapSphere (transform.position, 2.5f);

		foreach (Collider hit in hits)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
//			if (hit.gameObject.CompareTag("Explodeable"))
//				hit.GetComponent<Rigidbody>().AddExplosionForce(force, pos , radius);

			if (rb != null)
				rb.AddExplosionForce(force, pos , radius, upModifier);
		}
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Q))
			{
			Debug.Log ("Q pressed");
			toRepulse();
		}

			
	}

}
