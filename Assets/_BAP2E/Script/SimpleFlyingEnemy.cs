using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlyingEnemy : MonoBehaviour, ICanTakeDamage, IListener{
	public float minX = 3;
	public float maxX = 3;
	public float minY = 3;
	public float maxY = 3;

	public float speedY = 3;
	public float speedX = 5;

	public bool isMovingRight = false;
	public bool isMovingTop = false;

	public GameObject deadFX;

	float targetR,targetL,targetT,targetB;

	public bool dropBonusItem = true;
	public GameObject dropItem;
	public AudioClip soundDead;

	// Use this for initialization
	void Start () {
		targetR = transform.position.x + maxX;
		targetL = transform.position.x - minX;
		targetT = transform.position.y + maxY;
		targetB = transform.position.y - minY;

		Debug.Log (targetR + "/" + targetL + "/"+ targetT + "/"+ targetB);
	}
	
	// Update is called once per frame
	void Update () {
		if (isStop)
			return;
		
		float x, y;
		x = transform.position.x;
		y = transform.position.y;
		//moving horizontal
		if (isMovingRight) {
			transform.Translate (speedX * Time.deltaTime, 0, 0);
			if (transform.position.x >= targetR)
				isMovingRight = false;
		} else {
			transform.Translate (-speedX * Time.deltaTime, 0, 0);
			if (transform.position.x <= targetL)
				isMovingRight = true;
		}

		transform.localScale = new Vector3 ((isMovingRight ? -transform.localScale.x: transform.localScale.x) * transform.localScale.x, transform.localScale.y, transform.localScale.z);
		if (isMovingTop) {
			y = Mathf.Lerp (y, targetT, speedY * Time.deltaTime);
			if (Mathf.Abs (y - targetT) < 0.1f)
				isMovingTop = false;
		} else {
			y = Mathf.Lerp (y, targetB, speedY * Time.deltaTime);
			if (Mathf.Abs (y - targetB) < 0.1f)
				isMovingTop = true;
		}

		transform.position = new Vector2 (transform.position.x, y);
	}

    [Header("Contact Player")]
    public float makeDamage = 30;
    [Tooltip("delay a moment before give next damage to Player")]
    public float rateDamage = 0.2f;
    public Vector2 pushPlayer = new Vector2(15, 10);
    float nextDamage;

    IEnumerator OnTriggerStay2D(Collider2D other)
    {
        if (this)
        {
            var Player = other.GetComponent<Player>();
            if (Player == null)
                yield break;

            if (!Player.isPlaying)
                yield break;

            if (Player.GodMode)
                yield break;

            if (Player.gameObject.layer == LayerMask.NameToLayer("HidingZone"))
                yield break;

            if (GetComponent<CanBeJumpOn>() && transform.position.y + 1 < GameManager.Instance.Player.transform.position.y)
                yield break;

            if (Time.time < nextDamage + rateDamage)
                yield break;

            nextDamage = Time.time;

            if (makeDamage == 0)
                yield break;

            var facingDirectionX = Mathf.Sign(Player.transform.position.x - transform.position.x);
            var facingDirectionY = Mathf.Sign(Player.velocity.y);

            Vector2 _makeForce = new Vector2(Mathf.Clamp(Mathf.Abs(Player.velocity.x), 10, 15) * facingDirectionX,
                Mathf.Clamp(Mathf.Abs(Player.velocity.y), 5, 9) * facingDirectionY * -1);

            Player.TakeDamageFromContactEnemy(makeDamage, _makeForce, gameObject, true);

        }
    }

    #region ICanTakeDamage implementation


    public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
	{
		if (deadFX)
			Instantiate (deadFX, transform.position, Quaternion.identity);

		if (dropBonusItem && dropItem)
			Instantiate (dropItem, transform.position + Vector3.up * 0.5f, Quaternion.identity);

		SoundManager.PlaySfx (soundDead);
		Destroy (gameObject);
	}


	#endregion

	#region IListener implementation

	public void IPlay ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void ISuccess ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IUnPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		//		throw new System.NotImplementedException ();
	}
	bool isStop = false;
	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
//		anim.enabled = false;
		GetComponent<Animator>().enabled =  false;
		isStop = true;
	}

	public void IOnStopMovingOff ()
	{
		GetComponent<Animator>().enabled =  true;
		isStop = false;
	}

	#endregion

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			Gizmos.DrawWireCube(new Vector2((transform.position.x - minX + transform.position.x + maxX) * 0.5f, (transform.position.y - minY + transform.position.y + maxY) * 0.5f), new Vector2(minX + maxX, minY + maxY));
		}
	}
}
