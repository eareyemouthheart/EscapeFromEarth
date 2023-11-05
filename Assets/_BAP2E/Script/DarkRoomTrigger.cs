using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkRoomTrigger : MonoBehaviour {
	public enum Type{BlackToWhite, WhiteToBlack

		}
	public Type type;
	public bool overPlayer = false;

	SpriteRenderer sprite;
	Color oriColor;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		oriColor = sprite.color;

		if (type == Type.BlackToWhite)
			oriColor.a = 1;
		else
			oriColor.a = 0;

		sprite.color = oriColor;

		if (overPlayer) {
//			Debug.LogWarning (sprite.sortingLayerID +"/" +SortingLayer.GetLayerValueFromName ("Front"));
			sprite.sortingLayerName = "Front";
			sprite.sortingOrder = -10;
		} else {
			sprite.sortingOrder = -10;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == LayerMask.NameToLayer ("Player") || (other.gameObject.layer == LayerMask.NameToLayer ("IgnoreAll"))) {
			if (type == Type.BlackToWhite)
				oriColor.a = 0;
			else
				oriColor.a = 1;
			StartCoroutine (MMFade.FadeSpriteRenderer (sprite, 0.5f, oriColor));
		}
	}


	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.layer == LayerMask.NameToLayer ("Player") || (other.gameObject.layer == LayerMask.NameToLayer ("IgnoreAll"))) {
			if (type == Type.BlackToWhite)
				oriColor.a = 1;
			else
				oriColor.a = 0;
			StartCoroutine (MMFade.FadeSpriteRenderer (sprite, 0.5f, oriColor));
		}
	}
}
