using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockNewLevel : MonoBehaviour {
	public AudioClip soundReward;

	public void ShowRewardVideo(){
		SoundManager.Click ();
	}

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
		GlobalValue.allowClickUnityAdAgain = true;
		if (isSuccess)
        {
			SoundManager.PlaySfx(soundReward, 0.5f);
			GlobalValue.LevelPass = (GlobalValue.LevelPass + 1);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
    }
}
