using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LZJPauseUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI; // �� Inspector ������� Pause ���

    private bool isPaused = false;

    private void Start()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        }

        HidePauseUI();
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPauseAction -= GameInput_OnPauseAction;
        }
    }

    // ���� ESC �� Start ���л���ͣ״̬
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        ShowPauseUI();

        if (LXGameTimePanel.Instance != null)
        {
            LXGameTimePanel.Instance.enabled = false;
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        HidePauseUI();

        if (LXGameTimePanel.Instance != null)
        {
            LXGameTimePanel.Instance.enabled = true;
        }
    }

    private void ShowPauseUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
    }

    private void HidePauseUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
