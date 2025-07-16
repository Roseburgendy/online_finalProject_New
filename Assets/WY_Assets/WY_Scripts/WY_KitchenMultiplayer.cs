using System;
using UnityEngine;
using Photon.Pun;

public class WY_KitchenGameMultiplayer : MonoBehaviourPunCallbacks
{
    public static WY_KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent parent)
    {
        GameObject prefab = kitchenObjectSO.prefab;
        GameObject instance = PhotonNetwork.Instantiate(prefab.name, parent.GetKitchenObjectFollowTransform().position, Quaternion.identity);

        KitchenObject kitchenObject = instance.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(parent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        if (kitchenObject != null && kitchenObject.photonView.IsMine)
        {
            PhotonNetwork.Destroy(kitchenObject.gameObject);
        }
    }
    
    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
    {
        if (index >= 0 && index < kitchenObjectListSO.kitchenObjectSOList.Count)
        {
            return kitchenObjectListSO.kitchenObjectSOList[index];
        }
        else
        {
            Debug.LogWarning($"Invalid KitchenObjectSO index: {index}");
            return null;
        }
    }

} 

