using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviourPunCallbacks {


    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSingleplayerButton;
    [SerializeField] private Button backButton;


    private void Awake() {
        playMultiplayerButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.LobbyScene);
        });
        playSingleplayerButton.onClick.AddListener(() =>
        {
            StartSinglePlayerGame();

            Loader.Load(Loader.Scene.LXXCharacterSelectScene);
        });
        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.TitleScene);
        });

        Time.timeScale = 1f;
    }
    public void StartSinglePlayerGame()
    {
        PhotonNetwork.OfflineMode = true; 
        PhotonNetwork.CreateRoom("OfflineRoom"); 
    }
}