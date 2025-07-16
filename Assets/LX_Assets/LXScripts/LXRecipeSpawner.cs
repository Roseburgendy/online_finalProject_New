using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LXRecipeSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Point")]
    [SerializeField] private GameObject recipePrefab; // ���� RecipeItem Ԥ����
    [SerializeField] private Transform recipeContainer; // ���� RecipeListPanel

    [Header("Spawn Interval")]
    [SerializeField] private float spawnInterval = 60f; // Ĭ��ÿ 60 ������һ��

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

            // �����Ҫ�������ݣ������ƣ�
            Text text = newRecipe.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = "�¶��� - " + Random.Range(100, 999); // ʾ���õ�������
            }
        }
    }
}
