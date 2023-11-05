using UnityEngine;
using System.Collections;

public class MonsterSimpleAI : EnemyAI {
	[Header("Owner Setup")]
	public Animator animator;

	protected override void HitEvent ()
	{
		base.HitEvent ();
	}

	public override void Update ()
	{
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		base.Update ();
		HandleAnimation ();
	}

	void HandleAnimation(){
		animator.SetFloat ("speed", Mathf.Abs (velocity.x));
	}

    protected override void Dead ()
	{
        base.Dead();
		Debug.Log ("die");
		animator.SetTrigger ("die");

		velocity.x = 0;
		Destroy (gameObject, 0);
        SetForce(0, 5);
        controller.HandlePhysic = false;
    }

	protected override void OnRespawn ()
	{
		base.OnRespawn ();
		controller.HandlePhysic = true;
	}
}
