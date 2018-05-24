﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlatformShrinker : NetworkBehaviour {
    
    [Header("Shrinking Details")]
    [Range(0.0f, 120f)]public float m_timeAlive = 30f;
    [Range(0.001f, 0.99f)]public float m_minimumThreshold = 0.05f;

    private Vector3 original_scale;

    // Use this for initialization
    void Start () {
        original_scale = gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale -= (Time.deltaTime / m_timeAlive) * original_scale;

        if (gameObject.transform.localScale.x < original_scale.x * m_minimumThreshold ||
            gameObject.transform.localScale.z < original_scale.y * m_minimumThreshold)
        {
            Destroy(gameObject);
        }
	}
}
