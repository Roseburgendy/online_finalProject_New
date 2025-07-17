using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LXRecipe : MonoBehaviourPun
{
    public float countdownDuration = 60f;

    private float timeLeft;
    private Image timerImage;
    private bool isCompleted = false;

    private void Start()
    {
        timerImage = transform.Find("TimerCircle")?.GetComponent<Image>();
        timeLeft = countdownDuration;

        // ֻ�� MasterClient �����Ǳ������ɵĶ����ע��
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
                timerImage.color = new Color(0.5f, 1f, 0.5f); // ǳ��ɫ (R=0.5, G=1, B=0.5)
        }
    }


    private void SyncTime()
    {
        timeLeft -= 1f;
        timeLeft = Mathf.Max(0f, timeLeft);
        photonView.RPC(nameof(UpdateTime), RpcTarget.All, timeLeft);

        if (timeLeft <= 0f)
        {
            //�ĳ�ֻ�����Լ������㲥
            DestroyRecipeLocal();
            photonView.RPC(nameof(SyncDestroy), RpcTarget.Others);
        }
    }


    [PunRPC]
    private void UpdateTime(float syncedTime)
    {
        timeLeft = syncedTime;
    }

    // ���������߼�
    private void DestroyRecipeLocal()
    {
        if (!isCompleted)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LXScoreManager.Instance.BroadcastScore(-5);
            }
        }

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // ����ͬ�����٣�����һ������
    [PunRPC]
    private void SyncDestroy()
    {
        DestroyRecipeLocal();
    }


    public void CompleteByPlayer(int _)
    {
        if (isCompleted) return;

        isCompleted = true;

        int reward = 10;
        if (timeLeft >= 40f) reward = 20;
        else if (timeLeft >= 20f) reward = 15;

        if (PhotonNetwork.IsMasterClient)
        {
            LXScoreManager.Instance.BroadcastScore(reward);
        }

        //ֻ���ٵ�ǰ���󣬲�������������
        DestroyRecipeLocal();
        photonView.RPC(nameof(SyncDestroy), RpcTarget.Others);
    }


    // �ù��������������ж��Ƿ������
    public bool IsCompleted()
    {
        return isCompleted;
    }

    // ��ʾ��ƥ���߼������Զ����߼�����Ҳ���Դ����Զ����������ͣ�
/*    public bool Matches(KitchenObject burger)
    {
        // ʾ�������������а��� "Burger"���� burger �Ǻ�������
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
            Debug.LogWarning("�Ҳ������� PhotonView��AttachToUI ʧ��");
        }
    }
}
