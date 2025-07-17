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

    // MasterClient ���ӷ�����ͬ��������
    public void BroadcastScore(int delta)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            totalScore += delta;

            // ������СΪ 0 ��
            totalScore = Mathf.Max(0, totalScore);

            photonView.RPC(nameof(SyncScore), RpcTarget.All, totalScore);
        }
    }


    [PunRPC]
    private void SyncScore(int syncedScore)
    {
        totalScore = syncedScore;
        Debug.Log($"��ǰ�ܷ֣�{totalScore}");

        // ������з��� UI���������������
        LXScoreUI.Instance?.UpdateScoreDisplay(totalScore);
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}
