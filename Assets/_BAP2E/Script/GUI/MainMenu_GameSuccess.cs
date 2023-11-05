/// <summary>
/// Main menu game success.
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_GameSuccess : MonoBehaviour
{
	public GameObject Star1;        //the star object image
	public GameObject Star2;
	public GameObject Star3;

	public Text score, collectedCoins, totalCoin;
	public float timeCounter = 2;
	int counter = 0;
	int star = 0;
	int scoreRunning;

	public GameObject NextBut;

	public AudioClip soundCounting;
	public AudioClip soundStar1;
	public AudioClip soundStar2;
	public AudioClip soundStar3;

	// Use this for initialization
	IEnumerator Start()
	{
		Star1.SetActive(false);
		Star2.SetActive(false);
		Star3.SetActive(false);
		scoreRunning = Mathf.Clamp((int)((float)GameManager.Instance.Point / (timeCounter * 60f)), 1, int.MaxValue);    //nornally 1s = 60 frames
		NextBut.SetActive(GlobalValue.levelPlaying != -1);

		yield return new WaitForSeconds(0.5f);

		if (GlobalValue.bigStar1)
		{
			Star1.SetActive(true);
			SoundManager.PlaySfx(soundStar1);
			yield return new WaitForSeconds(1);
		}

		if (GlobalValue.bigStar2)
		{
			Star2.SetActive(true);
			SoundManager.PlaySfx(soundStar2);
			yield return new WaitForSeconds(1);
		}

		if (GlobalValue.bigStar3)
		{
			Star3.SetActive(true);
			SoundManager.PlaySfx(soundStar3);
			yield return new WaitForSeconds(1f);
		}
	}

	void Update()
	{
		//counting and display the current point to the text score
		counter += scoreRunning;
		counter = Mathf.Clamp(counter, 0, GameManager.Instance.Point);

		SoundManager.PlaySfx(soundCounting);

		score.text = counter + "";
		totalCoin.text = "Total coins = " + GlobalValue.SavedCoins;
		collectedCoins.text = "Collected coins +" + GameManager.Instance.Coin;

		if (counter == GameManager.Instance.Point)
		{
			enabled = false;
		}
	}
}
