﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackItem : MonoBehaviour {
	public GameObject Effect;
	public AudioClip sound;
	[Range(0,1)]
	public float soundVolume = 0.5f;

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null)
			return;

		SoundManager.PlaySfx (sound, soundVolume);
		GameManager.Instance.Player.isJetPackAvailable = true;
		GameManager.Instance.Player.jetpackEnegry = 100; 

		if (Effect != null)
			Instantiate (Effect, transform.position, transform.rotation);

		gameObject.SetActive (false);
	}
}
