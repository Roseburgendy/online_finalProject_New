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
                // 使用 LXRecipeManager 完成订单（Photon RPC）
                PhotonView photonView = PhotonView.Get(this); // 获取当前脚本挂载物体的 PhotonView
                if (photonView != null)
                {
                    photonView.RPC(nameof(RequestCompleteOrder), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
                }

                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

    // 本地调用的 RPC，让 MasterClient 调用 LXRecipeManager 来完成订单
    [PunRPC]
    private void RequestCompleteOrder(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LXRecipeManager.Instance.TryCompleteOrder(playerId);
        }
    }
}
