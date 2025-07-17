using UnityEngine;
using Photon.Pun;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // ʹ�� LXRecipeManager ��ɶ�����Photon RPC��
                PhotonView photonView = PhotonView.Get(this); // ��ȡ��ǰ�ű���������� PhotonView
                if (photonView != null)
                {
                    photonView.RPC(nameof(RequestCompleteOrder), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
                }

                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

    // ���ص��õ� RPC���� MasterClient ���� LXRecipeManager ����ɶ���
    [PunRPC]
    private void RequestCompleteOrder(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LXRecipeManager.Instance.TryCompleteOrder(playerId);
        }
    }
}
