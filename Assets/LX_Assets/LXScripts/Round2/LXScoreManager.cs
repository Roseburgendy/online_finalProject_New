using UnityEngine;
using Photon.Pun;

public class LXScoreManager : MonoBehaviourPun
{
    public static LXScoreManager Instance;

    private int totalScore = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // MasterClient 增加分数并同步所有人
    public void BroadcastScore(int delta)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            totalScore += delta;

            // 限制最小为 0 分
            totalScore = Mathf.Max(0, totalScore);

            photonView.RPC(nameof(SyncScore), RpcTarget.All, totalScore);
        }
    }


    [PunRPC]
    private void SyncScore(int syncedScore)
    {
        totalScore = syncedScore;
        Debug.Log($"当前总分：{totalScore}");

        // 如果你有分数 UI，可以在这里更新
        LXScoreUI.Instance?.UpdateScoreDisplay(totalScore);
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}
