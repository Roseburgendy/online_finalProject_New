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
        var ingredients = plate.GetKitchenObjectSOList();

        Debug.Log("������Ͽ�ʼ��");
        foreach (var ing in ingredients)
        {
            Debug.Log($"���ϣ�{ing.objectName}");
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
                    Debug.LogWarning($"δ֪���ϣ�{ingredient.objectName}");
                    break;
            }

        }

        Debug.Log($"���ϼ���� => ���:{hasBottomBun}������:{hasTomato}������:{hasLettuce}��֥ʿ:{hasCheese}����:{hasMeat}");

        return hasBottomBun && hasTomato && hasLettuce && hasCheese && hasMeat;
    }


    public override void Interact(IKitchenObjectParent player)
    {
        if (player.HasKitchenObject())
        {
            Debug.Log("���ڵ�1��");
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                Debug.Log("���ڵ�2��");
                // ������������Ƿ����㶩��
                if (PlateMatchesRecipe(plateKitchenObject))
                {
                    Debug.Log("���ڵ�3��");
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
            Debug.Log("��ʾ�ɹ�UI");
            successUIPanel.SetActive(true);
            Invoke(nameof(HideSuccessUI), 5f);
        }
        else
        {
            Debug.LogWarning("successUIPanel δ�󶨣�");
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