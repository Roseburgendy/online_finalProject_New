using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Photon.Pun;

public class LXGameTimePanel : MonoBehaviourPun
{
    [SerializeField] private Text timeText;
    [SerializeField] private float timeRemaining = 240f;
    public float TimeRemaining => timeRemaining; // 只读属性

    [Header("结算界面")]
    [SerializeField] private GameObject scoringPanel;

    [Header("星星点亮图")]
    [SerializeField] private GameObject star_L_lighten;
    [SerializeField] private GameObject star_R_lighten;
    [SerializeField] private GameObject star_M_lighten;

    [Header("分数文本")]
    [SerializeField] private Text finalScoreText;

    [Header("星星分数阈值")]
    [SerializeField] private int starLThreshold = 40;
    [SerializeField] private int starRThreshold = 60;
    [SerializeField] private int starMThreshold = 80;

    [Header("爆分提示 UI")]
    [SerializeField] private GameObject doubleScoreTipRoot; 
    [SerializeField] private float tipDuration = 3f;
    [SerializeField] private float tipScaleTime = 0.3f;


    [Header("回家是最好的礼物")]
    [SerializeField] private Button playBackButton;

    [Header("音乐起")]
    [SerializeField] private AudioSource localSource;
    [SerializeField] private AudioClip gameWinClip;
    [SerializeField] private AudioClip countdownClip;

    private bool gameEnded = false;
    private bool hasPlayedCountdown = false;
    private bool doubleScoreAnnounced = false;

    public static LXGameTimePanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        playBackButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.LXXCharacterSelectScene);
        });

        Time.timeScale = 1f;
    }



    private void Start()
    {
        if (timeText == null)
        {
            Debug.LogWarning("请在 Inspector 中拖入 Text 组件！");
        }

        if (scoringPanel != null)
        {
            scoringPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !gameEnded)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            photonView.RPC(nameof(SyncTimeDisplay), RpcTarget.All, timeRemaining);

            // 触发倒数 60 秒提示
            if (!doubleScoreAnnounced && timeRemaining <= 60f)
            {
                doubleScoreAnnounced = true;
                photonView.RPC(nameof(ShowDoubleScoreTip), RpcTarget.All);
            }

            if (!hasPlayedCountdown && timeRemaining <= 4f)
            {
                hasPlayedCountdown = true;
                photonView.RPC(nameof(PlayCountdownSound), RpcTarget.All);
            }


            if (timeRemaining <= 0f)
            {
                gameEnded = true;
                photonView.RPC(nameof(EndGame), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void SyncTimeDisplay(float syncedTime)
    {
        UpdateTimeDisplay(syncedTime);
    }

    private void UpdateTimeDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (timeText != null)
        {
            timeText.text = $"{minutes:00}:{seconds:00}";

            if (time <= 60f)
                timeText.color = Color.red;
            else
                timeText.color = Color.white;
        }
    }

    [PunRPC]
    private void EndGame()
    {
        Debug.Log("游戏结束，显示结算界面");

        if (scoringPanel != null)
        {
            scoringPanel.SetActive(true);
        }

        int score = LXScoreManager.Instance.GetTotalScore();

        if (finalScoreText != null)
        {
            finalScoreText.text = $"{score}";
        }

        // 使用 Inspector 中的可配置值
        if (star_L_lighten != null)
            star_L_lighten.SetActive(score >= starLThreshold);

        if (star_R_lighten != null)
            star_R_lighten.SetActive(score >= starRThreshold);

        if (star_M_lighten != null)
            star_M_lighten.SetActive(score >= starMThreshold);

        localSource.PlayOneShot(gameWinClip);
    }
    [PunRPC]
    private void PlayCountdownSound()
    {
        if (localSource != null && countdownClip != null)
        {
            localSource.PlayOneShot(countdownClip);
        }
    }

    [PunRPC]
    private void ShowDoubleScoreTip()
    {
        if (doubleScoreTipRoot != null)
        {
            doubleScoreTipRoot.SetActive(true);

            // 重置为不可见（缩放为0）
            doubleScoreTipRoot.transform.localScale = Vector3.zero;

            // 开始放大动画
            StartCoroutine(AnimateTipShow());
        }
    }


    private IEnumerator AnimateTipShow()
    {
        float timer = 0f;

        while (timer < tipScaleTime)
        {
            timer += Time.deltaTime;
            float t = timer / tipScaleTime;
            float scale = Mathf.SmoothStep(0f, 1f, t);
            doubleScoreTipRoot.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        doubleScoreTipRoot.transform.localScale = Vector3.one;

        // 停留一定时间后隐藏
        yield return new WaitForSeconds(tipDuration);

        doubleScoreTipRoot.SetActive(false);
    }


}
