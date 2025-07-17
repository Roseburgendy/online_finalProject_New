using UnityEngine;
using UnityEngine.UI;

public class LXScoreUI : MonoBehaviour
{
    public static LXScoreUI Instance;

    [SerializeField] private Text scoreText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore}";
        }
    }
}
