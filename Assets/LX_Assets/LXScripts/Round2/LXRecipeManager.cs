using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LXRecipeManager : MonoBehaviourPun
{
    public static LXRecipeManager Instance;

    private List<LXRecipe> activeRecipes = new List<LXRecipe>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterRecipe(LXRecipe recipe)
    {
        activeRecipes.Add(recipe);
    }

    public void UnregisterRecipe(LXRecipe recipe)
    {
        activeRecipes.Remove(recipe);
    }

    // 本地方法：用于 MasterClient 匹配订单
    public bool TryCompleteOrder(int playerId)
    {
        foreach (LXRecipe recipe in activeRecipes)
        {
            if (!recipe.IsCompleted())
            {
                recipe.CompleteByPlayer(playerId);
                return true;
            }
        }

        return false;
    }

    // 所有客户端都可以调用：让 MasterClient 代为处理
    [PunRPC]
    public void RequestCompleteOrder(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TryCompleteOrder(playerId);
        }
    }

}