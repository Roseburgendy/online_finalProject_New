using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LXRecipe : MonoBehaviourPun
{
    [SerializeField] private float countdownDuration = 60f;

    private float timeLeft;
    private Image timerImage;
    private bool isCompleted = false;
    [SerializeField] private int reward = 10;

    [Header("音乐起")]
    [SerializeField] private AudioSource localSource;
    [SerializeField] private AudioClip successDeliveryClip;
    [SerializeField] private AudioClip failDeliveryClip;

    private void Start()
    {
        timerImage = transform.Find("TimerCircle")?.GetComponent<Image>();
        timeLeft = countdownDuration;

        if (localSource == null)
        {
            GameObject musicManager = GameObject.Find("MusicManager");
            if (musicManager != null)
            {
                localSource = musicManager.GetComponent<AudioSource>();
            }

            if (localSource == null)
            {
                Debug.LogWarning("找不到 MusicManager 或者它没有 AudioSource！");
            }
        }

        // 只有 MasterClient 并且是本地生成的对象才注册
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            LXRecipeManager.Instance.RegisterRecipe(this);
            InvokeRepeating(nameof(SyncTime), 0f, 1f);
        }
    }


    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine && LXRecipeManager.Instance != null)
        {
            LXRecipeManager.Instance.UnregisterRecipe(this);
        }
    }


    private void Update()
    {
        if (timerImage != null)
        {
            timerImage.fillAmount = timeLeft / countdownDuration;

            float ratio = timeLeft / countdownDuration;

            if (ratio <= 0.25f)
                timerImage.color = new Color(1f, 0.5f, 0.5f, 1f);
            else if (ratio <= 0.5f)
                timerImage.color = Color.yellow;
            else
                timerImage.color = new Color(0.5f, 1f, 0.5f); // 浅绿色 (R=0.5, G=1, B=0.5)
        }
    }


    private void SyncTime()
    {
        timeLeft -= 1f;
        timeLeft = Mathf.Max(0f, timeLeft);
        photonView.RPC(nameof(UpdateTime), RpcTarget.All, timeLeft);

        if (timeLeft <= 0f)
        {
            //改成只销毁自己，不广播
            DestroyRecipeLocal();
            photonView.RPC(nameof(SyncDestroy), RpcTarget.Others);
        }
    }


    [PunRPC]
    private void UpdateTime(float syncedTime)
    {
        timeLeft = syncedTime;
    }

    // 本地销毁逻辑
    private void DestroyRecipeLocal()
    {
        if (!isCompleted)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LXScoreManager.Instance.BroadcastScore(-reward/2);
                localSource.PlayOneShot(failDeliveryClip);
            }
        }

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // 用于同步销毁（仅这一个对象）
    [PunRPC]
    private void SyncDestroy()
    {
        DestroyRecipeLocal();
    }


    public void CompleteByPlayer(int _)
    {
        if (isCompleted) return;

        isCompleted = true;

        int finalReward = reward; // 保留原始 reward 变量不变

        // 根据剩余时间动态加分
        if (timeLeft >= countdownDuration / 2f)
            finalReward += reward / 2;
        else if (timeLeft >= countdownDuration / 4f)
            finalReward += reward / 4;

        // 最后 60 秒加倍奖励
        if (LXGameTimePanel.Instance != null && LXGameTimePanel.Instance.TimeRemaining <= 60f)
        {
            finalReward *= 2;
        }


        localSource.PlayOneShot(successDeliveryClip);

        if (PhotonNetwork.IsMasterClient)
        {
            LXScoreManager.Instance.BroadcastScore(finalReward);
        }

        //只销毁当前对象，不误伤其他订单
        DestroyRecipeLocal();
        photonView.RPC(nameof(SyncDestroy), RpcTarget.Others);
    }


    // 让管理器或其他类判断是否已完成
    public bool IsCompleted()
    {
        return isCompleted;
    }

    // 简单示例匹配逻辑，可自定义逻辑（你也可以传入自定义数据类型）
/*    public bool Matches(KitchenObject burger)
    {
        // 示例：订单名称中包含 "Burger"，且 burger 是汉堡类型
        return burger != null && burger.GetKitchenObjectSO().objectName.Contains("Burger");
    }*/

    [PunRPC]
    public void AttachToUI(int containerViewID)
    {
        PhotonView containerView = PhotonView.Find(containerViewID);
        if (containerView != null)
        {
            Transform container = containerView.transform;
            transform.SetParent(container, false);
            transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning("找不到容器 PhotonView，AttachToUI 失败");
        }
    }
}
