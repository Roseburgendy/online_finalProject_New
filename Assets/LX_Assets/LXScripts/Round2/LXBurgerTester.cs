using UnityEngine;
using Photon.Pun;

public class LXBurgerTester : MonoBehaviourPun
{
    private void Update()
    {
        if (!photonView.IsMine) return; //ֻ�ñ��ؿ��ƶ�����

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            photonView.RPC(nameof(RequestOrderComplete), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }


    [PunRPC]
    private void RequestOrderComplete(int playerId)
    {
        // ֻ�� MasterClient �ᴦ������߼�
        LXRecipeManager.Instance.TryCompleteOrder(playerId);
    }
}
