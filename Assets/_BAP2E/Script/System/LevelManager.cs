using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LevelManager : MonoBehaviour, IListener
{
    public static LevelManager Instance { get; private set; }
    public int point1Star = 50, point2Stars = 300, point3Stars = 500;

    [Header("TIMER")]
    public bool useTimer = true;
    public int timer = 300;
    public int currentTimer;
    public int alarmTimeLess = 60;
    public AudioClip soundTimeLess;
    public AudioClip soundTimeUp;

    void Awake()
    {
        Instance = this;
        currentTimer = timer;
    }

    void Start()
    {
        StartCoroutine(BeginGameAfterCo(0.1f));
    }

    IEnumerator BeginGameAfterCo(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void StartGame()
    {
        if (useTimer)
            StartCoroutine(CountDownTimer());
    }

    public void KillPlayer()
    {
        GameManager.Instance.Player.Kill();
        GameManager.Instance.GameOver();
    }

    public void GotoCheckPoint()
    {
        if (useTimer)
        {
            StartCoroutine(CountDownTimer());
            currentTimer = timer;
        }
    }

    IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(1);
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            StartCoroutine(CountDownTimer());
            yield break;
        }

        currentTimer--;
        if (currentTimer == alarmTimeLess)
            SoundManager.PlaySfx(soundTimeLess);
        else if (currentTimer <= 0)
        {
            SoundManager.PlaySfx(soundTimeUp);
            KillPlayer();
        }

        if (currentTimer > 0)
            StartCoroutine(CountDownTimer());
    }

    public void IPlay()
    {
      
    }

    public void ISuccess()
    {
     
    }

    public void IPause()
    {
        
    }

    public void IUnPause()
    {
      
    }

    public void IGameOver()
    {
        
    }

    public void IOnRespawn()
    {
        

        if (currentTimer <= 0)
        {
            currentTimer = timer;
            if (useTimer)
                StartCoroutine(CountDownTimer());
        }
        else { currentTimer = timer; }
    }

    public void IOnStopMovingOn()
    {
     
    }

    public void IOnStopMovingOff()
    {
     
    }
}
