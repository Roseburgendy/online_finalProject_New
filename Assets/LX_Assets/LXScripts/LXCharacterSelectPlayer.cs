using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LXCharacterSelectPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private GameObject characterVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;
    [SerializeField] private List<GameObject> characters = new List<GameObject>();

    private Player photonPlayer;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient && photonPlayer != null)
            {
                PhotonNetwork.CloseConnection(photonPlayer);
            }
        });
    }

    private void Start()
    {
        kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        
        if (PhotonNetwork.LocalPlayer != null && characters.Count > 0)
        {
            object selectedCharacter;
            if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out selectedCharacter))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
                    { "SelectedCharacter", characters[0].name }
                });
            }
        }
        
        UpdatePlayer();
    }

    public void ShowCharacter(string characterId)
    {
        if (photonPlayer != PhotonNetwork.LocalPlayer) return;
        
        foreach (GameObject character in characters)
        {
            character.SetActive(character.name == characterId);
        }
    }

    public bool IsLocalPlayer()
    {
        return photonPlayer == PhotonNetwork.LocalPlayer;
    }

    private void UpdatePlayer()
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (playerIndex < players.Length)
        {
            photonPlayer = players[playerIndex];
            Show();

            bool isReady = photonPlayer.CustomProperties.TryGetValue("IsReady", out object readyObj) && (bool)readyObj;
            string selectedCharacter = photonPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object charObj)
                ? charObj.ToString()
                : (characters.Count > 0 ? characters[0].name : "");

            readyGameObject.SetActive(isReady);
            playerNameText.text = photonPlayer.NickName;
            UpdateCharacterVisual(selectedCharacter);
        }
        else
        {
            photonPlayer = null;
            Hide();
        }
    }

    private void UpdateCharacterVisual(string characterId)
    {
        if (photonPlayer == null) return;
        
        if (string.IsNullOrEmpty(characterId) && characters.Count > 0) 
            characterId = characters[0].name;

        if (!string.IsNullOrEmpty(characterId))
        {
            foreach (GameObject character in characters)
            {
                character.SetActive(character.name == characterId);
            }
        }
        else
        {
            foreach (GameObject character in characters)
            {
                character.SetActive(false);
            }
        }
    }

    private void Show()
    {
        readyGameObject.SetActive(true);
        characterVisual.SetActive(true);
        kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        playerNameText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        readyGameObject.SetActive(false);
        characterVisual.SetActive(false);
        kickButton.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(false);
        
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (photonPlayer != null && targetPlayer.ActorNumber == photonPlayer.ActorNumber && changedProps.ContainsKey("SelectedCharacter"))
        {
            string selectedCharacter = changedProps["SelectedCharacter"].ToString();
            UpdateCharacterVisual(selectedCharacter);
        }
        else if (photonPlayer != null && targetPlayer.ActorNumber == photonPlayer.ActorNumber)
        {
            UpdatePlayer();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayer();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayer();
    }
}