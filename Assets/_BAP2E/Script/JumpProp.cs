using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpProp : MonoBehaviour {
	public enum Gravity {Up, Down, Drop}
	public enum JumpType{Normal, FallDown}
	public JumpType jumpType;
	public bool useIt = true;
	public bool canUseAgain = true;
    //	public bool reserve = false;
    public bool changeDirection = false;
    public bool autoJump = false;
//	public bool sReserveJump = false;
	public AudioClip sound;
	public float force = 750f;
	public float radius = 0.8f;
	public float redRadius = 3f;

	[Header("Value Jump Type")]
	public float sJumpExtent = 5;
	public float fallDownSpeed = 5;
	[Space]

	public Color colorWhenJump = Color.red;
	public float distanceToDisappear = 4f;

	public enum EffectChoose{Bubble, New, Anim

		}
	public EffectChoose effectChoose;
	public GameObject child;
	public ParticleSystem bubble;
	public ParticleSystem newFX;
	ParticleSystem bubbleFX;
	Animator anim;
	//private Player player;
	private bool isJump = false;

	bool allowPush;

	float rate = 0.3f;
	// Use this for initialization
	IEnumerator Start () {
		GetComponent<CircleCollider2D> ().enabled = useIt;
		GetComponent<CircleCollider2D> ().radius = radius;


		bubble.gameObject.SetActive (effectChoose == EffectChoose.Bubble);
		newFX.gameObject.SetActive (effectChoose == EffectChoose.New);
		if (effectChoose == EffectChoose.Bubble) {
			bubbleFX = bubble;
		} else if (effectChoose == EffectChoose.New) {
			bubbleFX = newFX;
		} else if (effectChoose == EffectChoose.Anim) {
			bubble.gameObject.SetActive (false);
			newFX.gameObject.SetActive (false);
		}

		if(bubbleFX)
		bubbleFX.startSize = radius * bubbleFX.startSize / 0.8f;

		yield return new WaitForSeconds (1);

		//player = GameManager.Instance.Player;
		anim = GetComponent<Animator> ();
		allowPush = true;
	}

	float oriSize;
	Color oriColor;

	public void MakeJump(){
		if (useIt && allowPush && !isJump) {
			anim.SetTrigger ("hide");
			SoundManager.PlaySfx (sound);

			Invoke ("TurnFX", 0.1f);
			GameManager.Instance.Player.JumpOff ();
			switch (jumpType) {
			case JumpType.Normal:
				GameManager.Instance.Player.velocity = new Vector2 (0, force);

				break;
			case JumpType.FallDown:
				GameManager.Instance.Player.velocity = new Vector2 (0, (Physics2D.gravity.y > 0 ? fallDownSpeed : -fallDownSpeed));
				break;
			default:
				break;
			}

            if (GameManager.Instance.Player.isBoostSpeed2)
            {
                GameManager.Instance.Player.StopBoost2();
            }

            if (bubbleFX) {
				oriSize = bubbleFX.startSize;
				oriColor = bubbleFX.startColor;

				bubbleFX.startSize = redRadius;
				bubbleFX.startColor = colorWhenJump;
			}

			GetComponent<Animator> ().SetBool ("isUsed", true);

			isJump = true;
			allowPush = false;
			GameManager.Instance.Player.isInJumpPropArea = false;
			GameManager.Instance.Player.currentJumpProp = null;
//			enabled = false;
			StartCoroutine (WaitingCo ());

			//			StartCoroutine (Wait (rate));
		}
	}

	void TurnFX(){
//		player.EnableTrail();
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> ()) {
			if (autoJump)
				MakeJump ();
			else {
				GameManager.Instance.Player.isInJumpPropArea = true;
				GameManager.Instance.Player.currentJumpProp = this;
			}

//			GetComponent<CircleCollider2D> ().enabled = false;
		}


	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.GetComponent<Player> ()) {
			
				GameManager.Instance.Player.isInJumpPropArea = false;
				GameManager.Instance.Player.currentJumpProp = null;
			

//			GetComponent<CircleCollider2D> ().enabled = false;
		}


	}

	IEnumerator WaitingCo(){

		if (canUseAgain) {
			child.SetActive (false);
			yield return new WaitForSeconds (1);
			child.SetActive (true);
			switch (effectChoose) {
			case EffectChoose.Anim:
				break;
			default:
				bubbleFX.startSize = oriSize;
				bubbleFX.startColor = oriColor;
				break;
			}

			GetComponent<CircleCollider2D> ().enabled = true;
			isJump = false;
			GetComponent<Animator> ().SetBool ("isUsed", false);
			allowPush = true;
		} else {
			yield return new WaitForSeconds (0.6f);
			gameObject.SetActive (false);
		}
	}

//	void PushPlayer(){
//		var gravity = Physics2D.gravity.y>0? Gravity.Down:Gravity.Up;
//		Debug.Log (gravity);
//		switch (gravity) {
//		case Gravity.Up:
//			Physics2D.gravity = new Vector2 (Physics2D.gravity.x, Mathf.Abs (Physics2D.gravity.y) * 1);
//			//			player.rig.gravityScale = -1 * Mathf.Abs (player.rig.gravityScale);
//			Invoke ("Up", 0.15f);
//			player.isUp = true;
//			break;
//		case Gravity.Down:
//			Physics2D.gravity = new Vector2 (Physics2D.gravity.x, Mathf.Abs (Physics2D.gravity.y) * -1);
//			//			player.rig.gravityScale = Mathf.Abs (player.rig.gravityScale);
//			Invoke ("Down", 0.15f);
//			player.isUp = false;
//			break;
//		case Gravity.Drop:
//
//			break;
//		default:
//			break;
//		}
//
//		anim.SetTrigger ("hide");
//		Debug.LogWarning (player.rig.velocity.y);
////		Debug.LogError (Mathf.Abs (player.rig.velocity.y));
//		if (jumpType != JumpType.SJumpReserve) {
//			player.rig.velocity = Vector2.zero;
//		} else {
//			player.rig.velocity = new Vector2 (0, (Physics2D.gravity.y > 0 ? - sJumpExtent : sJumpExtent));
//		}
////		if (gravity == Gravity.Drop) {
////			player.rig.AddForce (new Vector2 (0, dropSpeed * -1));
////		} else
////		if (jumpType == JumpType.SJumpReserve)
////			player.rig.AddForce (new Vector2 (0, force * (Physics2D.gravity.y > 0 ? -1 : 1)));
//	}

//	void Up(){
//		//		player.rig.velocity = Vector2.zero;
//		//		Physics2D.gravity = new Vector2 (0, Physics2D.gravity.y / pushForce);
//		player.transform.localScale = new Vector3 (player.transform.localScale.x, -1 * Mathf.Abs (player.transform.localScale.y), player.transform.localScale.z);
//		//		player.DisableTrailSlow ();
////		if (jumpType == JumpType.SJumpReserve)
////			player.rig.AddForce (new Vector2 (0, force *0.5f* (Physics2D.gravity.y > 0 ? -1 : 1)));
//	}
//
//	void Down(){
//		//		player.rig.velocity = Vector2.zero;
//		player.transform.localScale = new Vector3 (player.transform.localScale.x, Mathf.Abs (player.transform.localScale.y), player.transform.localScale.z);
//		//		Physics2D.gravity = new Vector2 (0, Physics2D.gravity.y /pushForce);
//		//		player.DisableTrailSlow ();
////		if (jumpType == JumpType.SJumpReserve)
////			player.rig.AddForce (new Vector2 (0, force *0.5f* (Physics2D.gravity.y > 0 ? -1 : 1)));
//	}
}



//	IEnumerator Wait(float time){
//		allowPush = false;
//		yield return new WaitForSeconds (time);
//		allowPush = true;
//	}
