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

    private CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();
        if (isLocalPlayer) {
            CameraThirdPerson.SetPlayerTarget(transform);
        }
        else {
            this.enabled = false;
        }
    }

    void Update() {
        faceDirectionOfCamera();
        decrementStunTimer();
        
		if (controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
            if (isStunned()) {
                moveDirection = Vector3.zero;
            }
            if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
            
		}

		moveDirection.y -= gravity * Time.deltaTime;
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
				controller.Move (moveDirection * Time.deltaTime * chargeTimer * 3f);
				chargeTimer = 0;

		} else {
			controller.Move (moveDirection * Time.deltaTime);
		}
	}

    private void faceDirectionOfCamera() {
        Transform camTransform = Camera.main.GetComponent < Transform >();
        float neededYRotation = camTransform.rotation.eulerAngles.y;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = neededYRotation;
        transform.rotation = Quaternion.Euler(currentRotation);
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


}
