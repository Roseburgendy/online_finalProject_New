using UnityEngine;
using Photon.Pun;

public class LXBurgerTester : MonoBehaviourPun
{
    private void Update()
    {
        if (!photonView.IsMine) return; //只让本地控制对象处理

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            photonView.RPC(nameof(RequestOrderComplete), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }


    [PunRPC]
    private void RequestOrderComplete(int playerId)
    {
        // 只有 MasterClient 会处理这个逻辑
        LXRecipeManager.Instance.TryCompleteOrder(playerId);
    }
}
