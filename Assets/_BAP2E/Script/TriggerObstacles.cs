using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObstacles : MonoBehaviour, IListener {
	public GameObject target;
	public bool disableTargetOnStart = true;
	public bool resetWhenPlayerDead = false;
	public bool resetToOriginalPostion = false;
    [Header("Only use Reset Rotation for Fire 1 and 2, because some other may problem")]
    public bool resetRotation = false;
    List<MonoBehaviour> listMono;

	bool isWorked = false;
	Vector3 originalPos;
    Vector3 originalRot;

	// Use this for initialization
	void Awake () {
		Init ();

		originalPos = target.transform.position;
        originalRot = target.transform.eulerAngles;

    }

	void Init(){
		listMono = new List<MonoBehaviour> ();
		MonoBehaviour[] monos = target.GetComponents<MonoBehaviour> ();
		foreach (var mono in monos) {
			listMono.Add (mono);
			mono.enabled = false;
		}

		isWorked = false;
		target.SetActive (!disableTargetOnStart);
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if (isWorked)
			return;
		
		if (other.GetComponent<Player> () == null)
			return;
		target.SetActive (true);
		foreach (var mono in listMono) {
			mono.enabled = true;
		}

		isWorked = true;
	}

	void OnDrawGizmos(){
		if(target)
		Gizmos.DrawLine (transform.position, target.transform.position);
	}

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
		if (resetWhenPlayerDead) {
			if (resetToOriginalPostion)
				target.transform.position = originalPos;
            if (resetRotation)
                target.transform.eulerAngles = originalRot;

            Init ();
		}
	}

	public void IOnStopMovingOn ()
	{

	}

	public void IOnStopMovingOff ()
	{

	}

	#endregion
}
