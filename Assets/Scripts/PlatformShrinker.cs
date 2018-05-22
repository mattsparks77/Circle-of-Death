using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlatformShrinker : NetworkBehaviour {
    
    private Vector3 original_scale;
    private float platform_x;
    private float platform_z;
    private float play_time;
    private const float round_time = 30f;

	// Use this for initialization
	void Start () {
        original_scale = gameObject.transform.localScale;
        //platform_x = platform.localScale.x;
        //platform_z = platform.localScale.z;
        play_time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        play_time += Time.deltaTime;
        gameObject.transform.localScale -= (Time.deltaTime / round_time) * original_scale;

        if (gameObject.transform.localScale.x < original_scale.x * 0.05 ||
            gameObject.transform.localScale.z < original_scale.y * 0.05)
        {
            Destroy(gameObject);
        }
	}
}
