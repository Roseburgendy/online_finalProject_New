using UnityEngine;
using UnityEngine.UI;

public class LXRecipe : MonoBehaviour
{
    public float countdownDuration = 60f; // ����ʱ��ʱ�����룩

    private float timeLeft;
    private Image timerImage;

    private void Start()
    {
        timerImage = transform.Find("TimerCircle").GetComponent<Image>();
        timeLeft = countdownDuration;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timerImage != null)
        {
            timerImage.fillAmount = timeLeft / countdownDuration;
        }

        if (timeLeft <= 0f)
        {
            Destroy(gameObject); // �����������
        }
    }
}
