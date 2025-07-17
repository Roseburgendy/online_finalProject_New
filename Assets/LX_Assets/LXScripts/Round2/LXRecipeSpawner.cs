using System.Collections;
using UnityEngine;
using Photon.Pun;

public class LXRecipeSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private string recipePrefabName = "RecipeItem";

    [SerializeField] private float spawnInterval = 60f;

    [SerializeField] private PhotonView recipeContainerView;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnRecipe();
            StartCoroutine(SpawnRecipeLoop());
        }
    }

    private IEnumerator SpawnRecipeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRecipe();
        }
    }

    private void SpawnRecipe()
    {
        GameObject recipeObj = PhotonNetwork.Instantiate(recipePrefabName, Vector3.zero, Quaternion.identity);
        PhotonView recipeView = recipeObj.GetComponent<PhotonView>();

        // ¸½¼Óµ½ UI ÈÝÆ÷
        recipeView.RPC(nameof(LXRecipe.AttachToUI), RpcTarget.All, recipeContainerView.ViewID);
    }
}
