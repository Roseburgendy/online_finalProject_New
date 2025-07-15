using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class CharacterSelectUI : MonoBehaviour {


    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            //KitchenGameLobby.Instance.LeaveLobby();
           // NetworkManager.Singleton.Shutdown();
            // Leave lobby and return to options panel
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            else
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            }
            
        });
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start() {
        //Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        lobbyNameText.text = "Lobby Name: " + PhotonNetwork.CurrentRoom.Name;
    }
}