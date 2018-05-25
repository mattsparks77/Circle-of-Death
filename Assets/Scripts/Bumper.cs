using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bumper : NetworkBehaviour
{

    Vector3 bounceVelocity;
    [Range(0.1f,50f)] public float bounceForce = 1f;


    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Collided");

            bounceVelocity = new Vector3((transform.position.x - col.gameObject.transform.position.x) * bounceForce,
                (transform.position.y - col.gameObject.transform.position.y) * bounceForce,
                (transform.position.z - col.gameObject.transform.position.z) * bounceForce);
            Debug.Log(bounceVelocity);
            col.gameObject.GetComponent<Rigidbody>().AddForce(-bounceVelocity, ForceMode.VelocityChange);
            col.gameObject.GetComponent<PlayerController>().StunPlayer(0.3f);
        }
    }
}
