using UnityEngine;
using Photon.Pun;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }
    [Header("UI 提示")]
    [SerializeField] private GameObject successUIPanel;
    [SerializeField] private GameObject failureUIPanel;

    private void Awake()
    {
        Instance = this;
    }

    private bool PlateMatchesRecipe(PlateKitchenObject plate)
    {
        bool hasBottomBun = false;
        bool hasTomato = false;
        bool hasLettuce = false;
        bool hasCheese = false;
        bool hasMeat = false;

        foreach (KitchenObjectSO ingredient in plate.GetKitchenObjectSOList())
        {
            switch (ingredient.objectName)
            {
                case "Bread": hasBottomBun = true; break;
                case "TomatoSlices": hasTomato = true; break;
                case "CabbageSlices": hasLettuce = true; break;
                case "CheeseSlices": hasCheese = true; break;
                case "MeatPattyCooked": hasMeat = true; break;
            }
        }

        return hasBottomBun && hasTomato && hasLettuce && hasCheese && hasMeat;
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // 检查盘子配料是否满足订单
                if (PlateMatchesRecipe(plateKitchenObject))
                {
                    // 使用 LXRecipeManager 完成订单（Photon RPC）
                    PhotonView photonView = PhotonView.Get(this);
                    if (photonView != null)
                    {
                        photonView.RPC(nameof(RequestCompleteOrder), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
                    }
                    KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                    ShowSuccessUI();
                }
                else
                {
                    Debug.Log("配料不完整，订单未完成！");
                    ShowFailureUI();
                    // 可选：播放失败音效或提示 UI
                }
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

    private void ShowSuccessUI()
    {
        if (successUIPanel != null)
        {
            successUIPanel.SetActive(true);
            Invoke(nameof(HideSuccessUI), 5f);
        }
    }

    private void HideSuccessUI()
    {
        if (successUIPanel != null)
        {
            successUIPanel.SetActive(false);
        }
    }

    private void ShowFailureUI()
    {
        if (failureUIPanel != null)
        {
            failureUIPanel.SetActive(true);
            Invoke(nameof(HideFailureUI), 5f);
        }
    }

    private void HideFailureUI()
    {
        if (failureUIPanel != null)
        {
            failureUIPanel.SetActive(false);
        }
    }
}