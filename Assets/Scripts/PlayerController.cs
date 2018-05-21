﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    Rigidbody rb;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public float chargeTimer = 0;
	private Vector3 moveDirection = Vector3.zero;

    private float stunTimer = 0;

    //private CharacterController controller;
    private Rigidbody playerRB;

    void Start() {
        //controller = GetComponent<CharacterController>();
        playerRB = GetComponent<Rigidbody>();

        //Check if this player is the client's local player
        if (isLocalPlayer) {
            //Set the camera to follow this player if it's local player
            CameraThirdPerson.SetPlayerTarget(transform);
        }
        else {
            //Don't let other clients control this script if it's not a local player
            this.enabled = false;
        }
    }

    //For dealing with code that are not physics related
    void Update() {
        decrementStunTimer();
    }

    //For dealing with code that is physics-rigidbody related
    void FixedUpdate() {
        //Turn the y-axis to face where the camera is looking at
        faceDirectionOfCamera();

        //If the player is stunned, only take in account the gravity
        if (isStunned()) {
            Vector3 velocityWGravity = playerRB.velocity;
            velocityWGravity.y -= gravity * Time.fixedDeltaTime;
            playerRB.velocity = velocityWGravity;
            return;
        }

        //Take into account gravity amount here
        moveDirection.y -= gravity * Time.fixedDeltaTime;

        if (IsGrounded) {
            //Handle movement if the player is grounded and not stunned
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
            if (isStunned()) {
                moveDirection = Vector3.zero;
            } else if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;   
		}
        
        //Left click to charge up movement dash 
        if (Input.GetKey (KeyCode.Mouse0)) {
			chargeTimer += Time.deltaTime;
			Debug.Log (chargeTimer.ToString ());
		}

        //If player has movement dash charged, put moveDirection to take this into account
		if (Input.GetKeyUp (KeyCode.Mouse0) && chargeTimer > 2) {
			Debug.Log ("MOVE: " +  moveDirection.ToString ());
            //			if (moveDirection.x <= 0) {
            //				transform.forward *= 2;
            //			}
            //				controller.Move(new Vector3(10f, -1*gravity * Time.deltaTime,0f));
            //			else {
            moveDirection = moveDirection * chargeTimer * 3f;
            
		    chargeTimer = 0;

		}

        //Set player movement to be in the corresponding direction
        playerRB.velocity = moveDirection;
    }

    //Match the player's rotation with the camera's rotation using the rigidbody in the player
    private void faceDirectionOfCamera() {
        Transform camTransform = Camera.main.GetComponent < Transform >();
        float neededYRotation = camTransform.rotation.eulerAngles.y;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = neededYRotation;
        playerRB.MoveRotation (Quaternion.Euler(currentRotation));
    }

    //Returns if the player is stunned (can't move)
    private bool isStunned() {
        return stunTimer > 0;
    }

    //Decrements and checks the stun timer if the player is stunned
    private void decrementStunTimer() {
        if (stunTimer <= 0) {
            return;
        }
        stunTimer -= Time.deltaTime;
    }

    //Public variable for stunning the player for an amount of seconds
    public void StunPlayer(float stunTime) {
        stunTimer = stunTime;
    }

    //Checks if the player is grounded
    public bool IsGrounded { get {
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            return Physics.Raycast(transform.position, Vector3.down, col.height / 2);
        }
    }

    //Drawing Gizmos for debugging purposes
    private void OnDrawGizmos() {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Gizmos.DrawLine(transform.position,
            transform.position - Vector3.up * col.height / 2);
    }
}
