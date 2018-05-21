using System.Collections;
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
        if (isLocalPlayer) {
            CameraThirdPerson.SetPlayerTarget(transform);
        }
        else {
            this.enabled = false;
        }
    }

    void Update() {
        decrementStunTimer();
    }

    void FixedUpdate() {
        faceDirectionOfCamera();

        if (isStunned()) {
            Vector3 velocityWGravity = playerRB.velocity;
            velocityWGravity.y -= gravity * Time.fixedDeltaTime;
            playerRB.velocity = velocityWGravity;
            return;
        }

        print(IsGrounded);

        moveDirection.y -= gravity * Time.fixedDeltaTime;
        if (IsGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
            if (isStunned()) {
                moveDirection = Vector3.zero;
            } else if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
            
		}
        
        print("Move Direction: " + moveDirection);
        print("Player Vel: " + playerRB.velocity);
        if (Input.GetKey (KeyCode.Mouse0)) {
			chargeTimer += Time.deltaTime;
			Debug.Log (chargeTimer.ToString ());
		}
		if (Input.GetKeyUp (KeyCode.Mouse0) && chargeTimer > 2) {
			Debug.Log ("MOVE: " +  moveDirection.ToString ());
            //			if (moveDirection.x <= 0) {
            //				transform.forward *= 2;
            //			}
            //				controller.Move(new Vector3(10f, -1*gravity * Time.deltaTime,0f));
            //			else {
            moveDirection = moveDirection * /*Time.deltaTime **/ chargeTimer * 3f;
            //playerRB.velocity = moveDirection;
            //playerRB.MovePosition (transform.position + moveDirection);
            
		    chargeTimer = 0;

		}
        playerRB.velocity = moveDirection;
        //playerRB.MovePosition (transform.position + moveDirection * Time.deltaTime);
        //playerRB.AddForce(moveDirection, ForceMode.VelocityChange);

        if (playerRB.velocity.magnitude > speed) {
            playerRB.velocity = playerRB.velocity.normalized * speed;
        }
    }

    private void faceDirectionOfCamera() {
        Transform camTransform = Camera.main.GetComponent < Transform >();
        float neededYRotation = camTransform.rotation.eulerAngles.y;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = neededYRotation;
        playerRB.MoveRotation (Quaternion.Euler(currentRotation));
    }

    private bool isStunned() {
        return stunTimer > 0;
    }

    private void decrementStunTimer() {
        if (stunTimer <= 0) {
            return;
        }
        stunTimer -= Time.deltaTime;
    }

    public void StunPlayer(float stunTime) {
        stunTimer = stunTime;
    }

    public bool IsGrounded { get {
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            return Physics.Raycast(transform.position, Vector3.down, col.height / 2);
        }
    }


    private void OnDrawGizmos() {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Gizmos.DrawLine(transform.position,
            transform.position - Vector3.up * col.height / 2);
    }
}
