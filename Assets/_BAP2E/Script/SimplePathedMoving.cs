using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePathedMoving : MonoBehaviour, IListener {

	//	public LayerMask PassengerMask;
	public float delayOnStart = 0;
	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;

	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;

	public bool isLoop = true;

	//	List<PassengerMovement> passengerMovement;
	//	Dictionary<Transform,Rigidbody2D> passengerDictionary = new Dictionary<Transform, Rigidbody2D>();
	bool isPlaying = false;
	 IEnumerator Start () {
		if (delayOnStart > 0)
			yield return new WaitForSeconds(delayOnStart);

		if (localWaypoints.Length >= 2)
		{
			isPlaying = true;
			globalWaypoints = new Vector3[localWaypoints.Length];
			for (int i = 0; i < localWaypoints.Length; i++)
			{
				globalWaypoints[i] = localWaypoints[i] + transform.position;
			}
		}
	}

	void Update () {
		if (!isPlaying)
			return;

		if (isStop)
			return;
		
		Vector3 velocity = CalculatePlatformMovement();
		transform.Translate (velocity);
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}
	
	Vector3 CalculatePlatformMovement() {

		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex ++;

			if (fromWaypointIndex >= globalWaypoints.Length - 1) {
				if (!isLoop) {
					enabled = false;
				}
			}

			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	void OnDrawGizmos() {
		if (Application.isPlaying)
			return;

		if (!enabled)
			return;
		
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i =0; i < localWaypoints.Length; i ++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
	bool isStop = false;
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

	public void IOnStopMovingOn ()
	{
		Debug.Log ("IOnStopMovingOn");
//		anim.enabled = false;
		isStop = true;

	}

	public void IOnStopMovingOff ()
	{
//		anim.enabled = true;
		isStop = false;
	}

	#endregion
}
