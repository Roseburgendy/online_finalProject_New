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
        var ingredients = plate.GetKitchenObjectSOList();

        Debug.Log("检测配料开始：");
        foreach (var ing in ingredients)
        {
            Debug.Log($"配料：{ing.objectName}");
        }

        bool hasBottomBun = false;
        bool hasTomato = false;
        bool hasLettuce = false;
        bool hasCheese = false;
        bool hasMeat = false;

        foreach (KitchenObjectSO ingredient in ingredients)
        {
            switch (ingredient.objectName)
            {
                case "Bread": hasBottomBun = true; break;
                case "Tomato Slices": hasTomato = true; break;
                case "Cabbage Slices": hasLettuce = true; break;
                case "Cheese Slices": hasCheese = true; break;
                case "Meat Patty Cooked": hasMeat = true; break;
                default:
                    Debug.LogWarning($"未知配料：{ingredient.objectName}");
                    break;
            }

        }

        Debug.Log($"配料检测结果 => 面包:{hasBottomBun}，番茄:{hasTomato}，生菜:{hasLettuce}，芝士:{hasCheese}，肉:{hasMeat}");

        return hasBottomBun && hasTomato && hasLettuce && hasCheese && hasMeat;
    }


    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            Debug.Log("卡在第1步");
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                Debug.Log("卡在第2步");
                // 检查盘子配料是否满足订单
                if (PlateMatchesRecipe(plateKitchenObject))
                {
                    Debug.Log("卡在第3步");
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
            Debug.Log("显示成功UI");
            successUIPanel.SetActive(true);
            Invoke(nameof(HideSuccessUI), 5f);
        }
        else
        {
            Debug.LogWarning("successUIPanel 未绑定！");
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