using UnityEngine;
using Photon.Pun;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }
    [Header("UI ��ʾ")]
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
                // ������������Ƿ����㶩��
                if (PlateMatchesRecipe(plateKitchenObject))
                {
                    // ʹ�� LXRecipeManager ��ɶ�����Photon RPC��
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
                    Debug.Log("���ϲ�����������δ��ɣ�");
                    ShowFailureUI();
                    // ��ѡ������ʧ����Ч����ʾ UI
                }
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