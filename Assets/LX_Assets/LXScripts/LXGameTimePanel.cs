using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class LXGameTimePanel : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI timeText;


    private float timeRemaining = 240f; // 4分钟 = 240秒

    private void Start()
    {
        if (timeText == null)
        {
            CreateTimeUI();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) // 只有主机计时（防止多人同时计时冲突）
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            photonView.RPC(nameof(SyncTimeDisplay), RpcTarget.All, timeRemaining);
        }
    }
    
    void CreateTimeUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO;
            Canvas canvasComp = canvasGO.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        GameObject timeObj = new GameObject("GameTimePanel");
        timeObj.transform.SetParent(canvas.transform, false);
        
        RectTransform timeRect = timeObj.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(1f, 1f); // 改成右上角
        timeRect.anchorMax = new Vector2(1f, 1f); // 改成右上角
        timeRect.anchoredPosition = new Vector2(-100f, -50f); // 相对右上角偏移一点
        timeRect.sizeDelta = new Vector2(200, 50);
        
        timeText = timeObj.AddComponent<TextMeshProUGUI>();
        timeText.text = "04:00";
        timeText.fontSize = 40;
        timeText.color = Color.white;
        timeText.alignment = TextAlignmentOptions.Center;
        timeText.fontStyle = FontStyles.Bold;
        
        var outline = timeObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);
    }
    
    public void UpdateTimeDisplay(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        
        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            if (timeRemaining <= 30f)
            {
                timeText.color = Color.red;
            }
            else if (timeRemaining <= 60f)
            {
                timeText.color = Color.yellow;
            }
            else
            {
                timeText.color = Color.white;
            }
        }
    }

    [PunRPC]
    private void SyncTimeDisplay(float syncedTime)
    {
        UpdateTimeDisplay(syncedTime);
    }
}
