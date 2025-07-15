using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public enum GameScene
{
    TitleScene,
    MainMenuScene,
    LobbyScene,
    CharacterSelectScene,
    LoadingScene,
    GameScene
}

public class PhotonSceneLoader : MonoBehaviourPunCallbacks
{
    private static string pendingSceneName = null;

    /// <summary>
    /// 非网络房间跳转场景，直接跳转
    /// </summary>
    public static void LoadLocal(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    /// <summary>
    /// 在 Photon 房间中由 MasterClient 触发场景同步加载
    /// </summary>
    public static void LoadNetworkScene(GameScene scene)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(scene.ToString());
        }
        else
        {
            Debug.LogWarning("Only MasterClient can load network scene.");
        }
    }

    /// <summary>
    /// 离开房间后再加载指定场景（如返回菜单）
    /// </summary>
    public static void LeaveRoomAndLoad(GameScene scene)
    {
        if (PhotonNetwork.InRoom)
        {
            pendingSceneName = scene.ToString();
            PhotonNetwork.LeaveRoom();
            Debug.Log("LeaveRoomAndLoad");
        }
        else
        {
            SceneManager.LoadScene(scene.ToString());
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom(); 
        if (!string.IsNullOrEmpty(pendingSceneName))
        {
            SceneManager.LoadScene(pendingSceneName);
            pendingSceneName = null;
        }
    }
}