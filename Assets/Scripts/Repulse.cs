﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Repulse : NetworkBehaviour{

    public bool DrawDebugLines = false;
    [Range(1, 180)] public int hitAngles = 45;

	public float force = 1000f;
	public Collider[] hits;
	public float radius = 10f;

	public float charge;
	public float upModifier = 2f;

    private PlayerController controller;

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer) {
            this.enabled = false;
            return;
        }
        controller = GetComponent<PlayerController>();	
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
            
            if (rb != null && rb.gameObject != this.gameObject && checkWithinHitRadius(hit)) {
                rb.AddExplosionForce(force, pos, radius, upModifier);
                applyForceOnSelf(rb.transform.position);
            }
		}
	}

    //TODO make this work; it doesn't right now
    private void applyForceOnSelf(Vector3 hitPosition) {
        controller.StunPlayer(.5f);
        float diminishFactor = 2;
        float newForce = force / diminishFactor;
        float newUpModifier = upModifier / diminishFactor;
        Rigidbody playerRB = GetComponent<Rigidbody>();
        playerRB.velocity = Vector3.zero;
        playerRB.AddExplosionForce(newForce, hitPosition, radius, newUpModifier);
    }

    bool checkWithinHitRadius(Collider hit) {
        // TODO may need change this, since hit.transform.position is not accurate: 
        // It needs to account for where the player was hit exactly
        Vector3 vectorToTarget = hit.transform.position - transform.position;
        float degAngleOfHit = Vector3.Angle(vectorToTarget, transform.forward);
        print(degAngleOfHit);
        return degAngleOfHit < hitAngles;
    }
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Q))
			{
			Debug.Log ("Q pressed");
			toRepulse();
		}
	}

    private void OnDrawGizmos() {
        if (!DrawDebugLines) {
            return;
        }
        int numSweeps = 10;
        float increments = 2 * hitAngles / (float)numSweeps; // 2*angle, since its numsweeps from 0->hitAngles
        for (float i = -hitAngles; i < hitAngles + increments / 2; i += increments) {
            Vector3 lineVector = Quaternion.Euler(0, i, 0) * transform.forward * radius;
            Gizmos.DrawLine(transform.position, transform.position + lineVector);
        }
    }

}
