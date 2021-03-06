﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Repulse : NetworkBehaviour {

    //For debugging the repulse raycasting
    public bool DrawDebugLines = false;

    [Header("Repulsion Power")]
    public float radius = 10f;
    public float force = 1000f;
    private float charge;
    public float upModifier = 2f;

    [Header("Repulsion Details")]
    [Range(1, 180)] public int hitAngles = 45;
    [Range(0, 2)] public float selfRepulseFactor = 0.5f;
    [Range(0, 3)] public float repulseStunTime = 0.5f; //TODO make this used
    [Range(0, 3)] public float selfStunTime = 0.35f;
    [SerializeField] private ParticleSystem particles;
    private Collider[] hits;
    
    private PlayerController controller;
    
	// Use this for initialization
	void Start () {
        controller = GetComponent<PlayerController>();
        
        //Check and disable this script if it's not a local player
        if (!isLocalPlayer) {
            this.enabled = false;
            return;
        }
	}



    //Push whatever objects it hits backwards, as well as push the player back
	void toRepulse()
	{
        CmdSpawnParticles();

        Vector3 pos = transform.position;
		hits = Physics.OverlapSphere (transform.position, 2.5f);

		foreach (Collider hit in hits)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
            //			if (hit.gameObject.CompareTag("Explodeable"))
            //				hit.GetComponent<Rigidbody>().AddExplosionForce(force, pos , radius);
            
            if (rb != null && rb.gameObject != this.gameObject && checkWithinHitRadius(hit)) {
                CmdApplyForces(hit.gameObject);
            }
		}
	}

    [Command]
    private void CmdSpawnParticles() {
        RpcSpawnParticles();
    }

    [ClientRpc]
    private void RpcSpawnParticles() {
        if (particles) {
            ParticleSystem newParticle = Instantiate(particles, frontOfPlayer(), transform.rotation);
            newParticle.Play();
        }
    }

    [Command]
    private void CmdApplyForces(GameObject objToApplyTo) {
        RpcApplyForces(objToApplyTo);
    }
    [ClientRpc]
    private void RpcApplyForces(GameObject objToApplyTo) {
        Rigidbody rbToApplyTo = objToApplyTo.GetComponent<Rigidbody>();
        PlayerController pc = objToApplyTo.GetComponent<PlayerController>();
        if (pc != null) {
            pc.StunPlayer(repulseStunTime);
        }
        Vector3 front = frontOfPlayer();
        float playerRadius = transform.localScale.z / 2;
        Debug.DrawLine(front, rbToApplyTo.transform.position);
        rbToApplyTo.AddExplosionForce(force, front, radius, upModifier);
        Vector3 directionToRB = rbToApplyTo.transform.position - transform.position;
        applyForceOnSelf(rbToApplyTo.transform.position - directionToRB.normalized * playerRadius);
    }
    

    //Apply the force on the player from the direction of where they hit an object, as well as a diminishing factor
    private void applyForceOnSelf(Vector3 hitPosition) {
        //Stun the player for a small amount of time
        //Without this, the players can instantly counter the force of the push with their movement
        controller.StunPlayer(selfStunTime);
        //Don't apply force on self if the selfRepulseFactor is set to 0
        if (selfRepulseFactor == 0) {
            return;
        }
        float newForce = force * selfRepulseFactor;
        float newUpModifier = upModifier * selfRepulseFactor;
        Rigidbody playerRB = GetComponent<Rigidbody>();
        playerRB.velocity = Vector3.zero;
        playerRB.AddExplosionForce(newForce, hitPosition, radius, newUpModifier);
    }

    //Checks if something is within the radius of getting hit
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

    //For debugging the repulse raycasts amount
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

    private Vector3 frontOfPlayer() {
        float playerRadius = transform.localScale.z / 2;
        return transform.position + transform.forward * playerRadius;
    }

}
