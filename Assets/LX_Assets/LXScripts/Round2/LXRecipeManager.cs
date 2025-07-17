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

    // ���ط��������� MasterClient ƥ�䶩��
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

    // ���пͻ��˶����Ե��ã��� MasterClient ��Ϊ����
    [PunRPC]
    public void RequestCompleteOrder(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TryCompleteOrder(playerId);
        }
    }

}