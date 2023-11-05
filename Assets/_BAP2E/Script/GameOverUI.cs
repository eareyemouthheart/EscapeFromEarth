using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
	public int RewaredLive = 3;
public GameObject ContinueBut;
	public Text rewardTxt;

	public  void Start ()
	{

	}

	public void FreeLive()
	{
		if (!GlobalValue.allowClickUnityAdAgain)
			return;

		SoundManager.Click();

		return;

	}
    public void NoWatch(){
		SoundManager.Click ();
		GameManager.Instance.ResetValue ();
	}

	public void Exit(){ 
		MenuManager.Instance.ExitGame ();
	}


	public void Restart(){
		NoWatch ();
		MenuManager.Instance.RestartGame ();
	}

	public void OnSuccess ()
	{
		Debug.Log ("get free lives");
        GlobalValue.SavedLives = Mathf.Max(0, GlobalValue.SavedLives);
        GlobalValue.SavedLives += RewaredLive;
		GameManager.Instance.Continues ();

		gameObject.SetActive (false);
	}
}
