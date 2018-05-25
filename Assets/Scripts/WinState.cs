using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : MonoBehaviour {

	private bool started = false;
	public GameObject winUI;

	// Use this for initialization
	void Start () {
		started = false;
		winUI = transform.Find("Panel").gameObject;
		winUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
			started = true;
		if (GameObject.FindGameObjectsWithTag("Player").Length <= 1 && started){
			winUI.SetActive(true);
			started = false;
		}
	}
}
