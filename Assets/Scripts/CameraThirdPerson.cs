using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CameraThirdPerson : MonoBehaviour {

    [SerializeField] Transform playerTarget;
    bool cursorLocked;

    [SerializeField] [Range(1, 10)] float mouseSensitivity = 2;
    [SerializeField] [Range(-45, 0)] float minAngle = -25;
    [SerializeField] [Range(0, 75)] float maxAngle = 60;

    // Use this for initialization
    void Start () {
        //if (Application.isPlaying) {
        setCursorLock(true);
        //}
	}
	
	// Update is called once per frame
	void Update () {

        setPositionToTarget();

        //if (Application.isPlaying) {
        registerMouseLockCommands();
        registerMouseMovement();
        //}
        //else {
        //    setCursorLock(false);
        //}
    }

    void setPositionToTarget() {
        transform.position = playerTarget.position;
    }

    void registerMouseMovement() {
        if (!cursorLocked) 
            return;

        float mouseMoveAmountX = Input.GetAxis("Mouse X");
        float mouseMoveAmountY = -Input.GetAxis("Mouse Y"); //Negative so that moving mouse down looks upward
        if (Mathf.Abs(mouseMoveAmountX) < .15f)
            mouseMoveAmountX = 0;
        if (Mathf.Abs(mouseMoveAmountY) < .15f)
            mouseMoveAmountY = 0;

        if (mouseMoveAmountX == 0 && mouseMoveAmountY == 0)
            return;

        mouseMoveAmountX *= mouseSensitivity;
        mouseMoveAmountY *= mouseSensitivity;

        Vector3 currEuler = transform.rotation.eulerAngles;
        float getXRotation = (currEuler.x > 180 ? currEuler.x - 360 : currEuler.x);
        float clampedXRotation = Mathf.Clamp(getXRotation + mouseMoveAmountY, minAngle, maxAngle);
        Quaternion newRotation = Quaternion.Euler(clampedXRotation, currEuler.y + mouseMoveAmountX, currEuler.z);
        transform.rotation = newRotation;
    }

    void registerMouseLockCommands() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            setCursorLock(false);
        }
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            setCursorLock(true);
        }
    }

    void setCursorLock(bool lockCursor) {
        cursorLocked = lockCursor;
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
