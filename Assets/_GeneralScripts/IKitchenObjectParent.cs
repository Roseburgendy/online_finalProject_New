using UnityEngine;

public interface IKitchenObjectParent
{

    Transform GetKitchenObjectFollowTransform();

    void SetKitchenObject(KitchenObject kitchenObject);

    KitchenObject GetKitchenObject();

    void ClearKitchenObject();

    bool HasKitchenObject();

    // 删除这一行，因为你不再用 Unity Netcode
    // NetworkObject GetNetworkObject(); 
}
