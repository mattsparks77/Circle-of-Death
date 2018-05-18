using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float playerSpeed = 5f;
	Rigidbody rb;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public float chargeTimer = 0;
	private Vector3 moveDirection = Vector3.zero;
	void Update() {
		CharacterController controller = GetComponent<CharacterController>();
		if (controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
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

}
