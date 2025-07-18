using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class CharacterSelectUI : MonoBehaviourPunCallbacks {


    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    //[SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
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
                PhotonNetwork.Disconnect(); // 断开 Photon 网络连接
                Loader.Load(Loader.Scene.MainMenuScene);
            }
        });
    }
    
    public override void OnJoinedRoom() {
        lobbyNameText.text = "Lobby Name: " + PhotonNetwork.CurrentRoom.Name;
    }
}