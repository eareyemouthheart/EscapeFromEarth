using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public enum ITEM_TYPE { iap1, iap2, iap3, watchVideo, removeAd}

    public ITEM_TYPE itemType;
    public int rewarded = 100;
    public float price = 100;
    public GameObject watchVideocontainer;

    public AudioClip soundRewarded;

    public Text priceTxt, rewardedTxt, rewardTimeCountDownTxt;

    private void Start()
    {

    }

    private void Update()
    {
     }

 
    public void Buy()
    {
        switch (itemType)
        {
            case ITEM_TYPE.watchVideo:
                 break; 
        }
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        if (isSuccess)
        {
            GlobalValue.SavedCoins += rewarded;
            SoundManager.PlaySfx(soundRewarded);
         }
    }

    private void Purchaser_iAPResult(int id)
    { 
        GlobalValue.SavedCoins += rewarded;
        SoundManager.PlaySfx(soundRewarded);
     }
}
