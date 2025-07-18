using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlateKitchenObject : KitchenObject, IPunObservable
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!photonView.IsMine)
        {
            // 如果当前不是 Owner，请求 Owner 来添加
            int index = WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO);
            photonView.RPC(nameof(RequestAddIngredientRPC), photonView.Owner, index);
            return false; // 非 Owner 不处理，等待 Owner 来广播
        }

        return TryAddIngredient_Internal(kitchenObjectSO);
    }

    [PunRPC]
    private void RequestAddIngredientRPC(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        TryAddIngredient(kitchenObjectSO); // 再次走 TryAdd，因 Owner 会执行内部添加和广播
    }

    private bool TryAddIngredient_Internal(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) return false;

        int index = WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO);
        photonView.RPC(nameof(RPC_AddIngredient), RpcTarget.All, index);
        return true;
    }

    [PunRPC]
    private void RPC_AddIngredient(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送盘子上的配料列表
            stream.SendNext(kitchenObjectSOList.Count);
            foreach (var ingredient in kitchenObjectSOList)
            {
                stream.SendNext(WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(ingredient));
            }
        
            // 发送当前位置和旋转
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 接收配料列表
            kitchenObjectSOList.Clear();
            int count = (int)stream.ReceiveNext();
            for (int i = 0; i < count; i++)
            {
                int index = (int)stream.ReceiveNext();
                var ingredient = WY_KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(index);
                kitchenObjectSOList.Add(ingredient);
            }
        
            // 接收位置和旋转
            Vector3 networkPosition = (Vector3)stream.ReceiveNext();
            Quaternion networkRotation = (Quaternion)stream.ReceiveNext();
        
            // 如果不是本地对象，应用网络位置
            if (!photonView.IsMine)
            {
                transform.position = networkPosition;
                transform.rotation = networkRotation;
            }
        
            // 更新视觉表现
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        // 触发所有添加事件来更新视觉表现
        foreach (var ingredient in kitchenObjectSOList)
        {
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = ingredient
            });
        }
    }
}