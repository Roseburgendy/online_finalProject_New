using UnityEngine;

public class LZJDeliveryFeedbackUI : MonoBehaviour
{
    public static LZJDeliveryFeedbackUI Instance;

    [SerializeField] private GameObject successUI;
    [SerializeField] private GameObject failUI;

    private void Awake()
    {
        Instance = this;
        HideAll();
    }

    private void HideAll()
    {
        successUI.SetActive(false);
        failUI.SetActive(false);
    }

    public void ShowSuccess()
    {
        HideAll();
        successUI.SetActive(true);
        Invoke(nameof(HideAll), 5f); // 5 √Î∫Ûπÿ±’
    }

    public void ShowFail()
    {
        HideAll();
        failUI.SetActive(true);
        Invoke(nameof(HideAll), 5f);
    }
}
