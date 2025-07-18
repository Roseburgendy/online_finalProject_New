using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ClearCounter : BaseCounter, IPunObservable
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送当前柜台上的物品信息
            stream.SendNext(HasKitchenObject());
            if (HasKitchenObject())
            {
                stream.SendNext(GetKitchenObject().GetKitchenObjectSO().objectName);
            }
        }
        else
        {
            // 接收并更新柜台上的物品
            bool hasObject = (bool)stream.ReceiveNext();
            if (hasObject)
            {
                string objectName = (string)stream.ReceiveNext();
                // 这里需要实现根据名称创建物品的逻辑
            }
            else
            {
                ClearKitchenObject();
            }
        }
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (!HasKitchenObject())
        {
            // There is no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    // Player is not carrying Plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Counter is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
