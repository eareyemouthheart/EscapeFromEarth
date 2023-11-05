using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterTriggerHelper : MonoBehaviour,IPlayerRespawnListener, IListener, ITriggerPlayer
{
	public bool canUseAgain = true;
	public AudioClip soundShowUp;
	public GameObject[] Monsters;

	public Rigidbody2D[] rigidbodyObjs;
	bool isWorked = false;

	void Awake(){
		if (Monsters != null) {
			foreach (var monster in Monsters) {
				if (monster != null)
					monster.SetActive (false);
			}
		}

		if (rigidbodyObjs != null) {
			foreach (var rig in rigidbodyObjs) {
				if (rig != null)
					rig.isKinematic = true;
			}
		}
	}

	public void OnTrigger()
	{
		if (isWorked)
			return;

		foreach (var monster in Monsters)
		{
			if (monster != null)
				monster.SetActive(true);
		}

		foreach (var rig in rigidbodyObjs)
		{
			if (rig != null)
			{
				rig.isKinematic = false;
			}
		}

		SoundManager.PlaySfx(soundShowUp);

		isWorked = !canUseAgain;
	}

	public void OnPlayerRespawnInThisCheckPoint (CheckPoint checkpoint, Player player)
	{
		StartCoroutine (DisableAllEnemiesCo (0.1f));		//dothis to make sure disable all enemies after Respawn event
	}

	IEnumerator DisableAllEnemiesCo(float delay){
		yield return new WaitForSeconds (delay);
		foreach (var monster in Monsters) {
			if (monster != null)
				monster.SetActive (false);
		}
	}

	void OnDrawGizmos(){
		if (Monsters.Length > 0) {
			foreach (var obj in Monsters) {
				if(obj)
				Gizmos.DrawLine (transform.position, obj.transform.position);
			}
		}

		if (rigidbodyObjs.Length > 0) {
			foreach (var obj in rigidbodyObjs) {
				if(obj)
					Gizmos.DrawLine (transform.position, obj.transform.position);
			}
		}
	}
	bool isStop = false;
	#region IListener implementation

	public void IPlay ()
	{
		
	}

	public void ISuccess ()
	{
		
	}

	public void IPause ()
	{
		
	}

	public void IUnPause ()
	{
		
	}

	public void IGameOver ()
	{

	}

	public void IOnRespawn ()
	{
        if (isWorked)
            isWorked = !canUseAgain;
	}

	public void IOnStopMovingOn ()
	{
		isStop = true;
	}

	public void IOnStopMovingOff ()
	{
		isStop = false;
	}

    #endregion
}
