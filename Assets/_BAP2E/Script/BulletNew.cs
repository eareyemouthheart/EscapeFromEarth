﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNew : MonoBehaviour {
	public GameObject Effect;
	public AudioClip sound;
	[Range(0,1)]
	public float soundVolume = 0.5f;
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> () == null)
			return;

		SoundManager.PlaySfx (sound, soundVolume);

		GameManager.Instance.AddPoint (10,transform);

		if (Effect != null)
			Instantiate (Effect, transform.position, transform.rotation);

		GameManager.Instance.isSpecialBullet = true;

		//		if (pointToAdd != 0)
		//			GameManager.Instance.ShowFloatingText (pointToAdd.ToString (), transform.position, Color.white);

		gameObject.SetActive (false);
	}
}
