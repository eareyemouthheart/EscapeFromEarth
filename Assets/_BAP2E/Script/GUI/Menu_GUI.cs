using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUI : MonoBehaviour {
    public static Menu_GUI Instance;
    public Text scoreText;
	public Text coinText;
	public Text timerText;
	public Text liveText;

	public Transform healthBar;
    public GameObject TimerUI;
	public GameObject Key, Jetpack;
	public Image playerHead;

	CanvasGroup canvasGroup;

    public GameObject starGroup;
    public Image star1, star2, star3;
    bool isStar1Collected, isStar2Colltected, isStar3Collected;
    bool firstPlay = true;

    private void Awake()
    {
        Instance = this;

        if (LevelMapType.Instance.levelType == LEVELTYPE.BossFight)
            starGroup.SetActive(false);
    }

    private void OnEnable()
    {
        if (firstPlay)
            return;

        if (isStar1Collected)
        {
            star1.color = Color.white;
        }
        if (isStar2Colltected)
        {
            star2.color = Color.white;
        }
        if (isStar3Collected)
        {
            star3.color = Color.white;
        }
    }

    private void Start()
    {
        TimerUI.SetActive(LevelManager.Instance.useTimer);
		canvasGroup = GetComponent<CanvasGroup>();

        star1.color = Color.black;
        star2.color = Color.black;
        star3.color = Color.black;

        firstPlay = false;
    }

    // Update is called once per frame
    void Update () {
		scoreText.text = GameManager.Instance.Point.ToString ("00000");
		coinText.text = GameManager.Instance.Coin.ToString ("00");
        timerText.text = LevelManager.Instance.currentTimer.ToString("000");
        if (GlobalValue.levelPlaying == -1)
            liveText.text = "MAX";
        else
            liveText.text = "x" + GlobalValue.SavedLives;

		healthBar.localScale = new Vector3 ((float)GameManager.Instance.Player.Health / (float)GameManager.Instance.Player.maxHealth, 1, 1);

		Key.SetActive (GameManager.Instance.isHasKey);
        Jetpack.SetActive(GameManager.Instance.Player.isJetPackAvailable);

        canvasGroup.alpha = GameManager.Instance.isInDialogue ? 0 : 1;
        if (GameManager.Instance.Player)
            playerHead.sprite = GameManager.Instance.Player.headImage;
    }

    public void ScrollCollectAnim(int ID)
    {
        switch (ID)
        {
            case 1:
                star1.color = Color.white;
                isStar1Collected = true;
                break;
            case 2:
                star2.color = Color.white;
                isStar2Colltected = true;
                break;
            case 3:
                star3.color = Color.white;
                isStar3Collected = true;
                break;
            default:
                break;
        }
    }
}
