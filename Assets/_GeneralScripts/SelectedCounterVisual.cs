using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start() {
        // 如果本地玩家实例已创建，直接注册事件
        if (PJR_PlayerMovement.LocalInstance != null) {
            PJR_PlayerMovement.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        } else {
            // 否则等待玩家生成后注册
            PJR_PlayerMovement.OnAnyPlayerSpawned += OnAnyPlayerSpawned;
        }
    }

    private void OnDestroy() {
        // 取消注册，避免内存泄漏
        if (PJR_PlayerMovement.LocalInstance != null) {
            PJR_PlayerMovement.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
        }
        PJR_PlayerMovement.OnAnyPlayerSpawned -= OnAnyPlayerSpawned;
    }

    private void OnAnyPlayerSpawned(object sender, System.EventArgs e) {
        // 再次尝试注册事件（防止多次重复注册）
        if (PJR_PlayerMovement.LocalInstance != null) {
            PJR_PlayerMovement.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            PJR_PlayerMovement.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, PJR_PlayerMovement.OnSelectedCounterChangedEventArgs e) {
        if (e.selectedCounter == baseCounter) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}