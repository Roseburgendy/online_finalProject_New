using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {


    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSingleplayerButton;
    [SerializeField] private Button backButton;


    private void Awake() {
        playMultiplayerButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        playSingleplayerButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);

        });
        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.TitleScene);
        });

        Time.timeScale = 1f;
    }

}