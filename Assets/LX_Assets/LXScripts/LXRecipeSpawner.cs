using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LXRecipeSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Point")]
    [SerializeField] private GameObject recipePrefab; // 拖入 RecipeItem 预制体
    [SerializeField] private Transform recipeContainer; // 拖入 RecipeListPanel

    [Header("Spawn Interval")]
    [SerializeField] private float spawnInterval = 60f; // 默认每 60 秒生成一个

    private void Start()
    {
        SpawnRecipe();
        StartCoroutine(SpawnRecipeLoop());
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
        if (recipePrefab != null && recipeContainer != null)
        {
            GameObject newRecipe = Instantiate(recipePrefab, recipeContainer);
            newRecipe.transform.localScale = Vector3.one;

            // 如果需要设置内容，如名称：
            Text text = newRecipe.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = "新订单 - " + Random.Range(100, 999); // 示例用的随机编号
            }
        }
    }
}
