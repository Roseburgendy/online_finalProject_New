using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LXCharacterSelectReady : MonoBehaviourPunCallbacks
{
    public static LXCharacterSelectReady Instance { get; private set; }
    
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;

    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        readyButton.onClick.AddListener(SetPlayerReady);
        startGameButton.onClick.AddListener(StartGame);

        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsReady", out object readyObj) && (bool)readyObj;
        
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && isReady);

        if (PhotonNetwork.IsMasterClient)
        {
            readyButton.gameObject.SetActive(!isReady);
        }
        
        readyButton.interactable = !isReady;
    }

    public void SetPlayerReady()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
            { "IsReady", true }
        });
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && AllPlayersReady())
        {
            PhotonNetwork.LoadLevel("LXGameScene");
        }
    }

    private bool AllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                return false;
            }
        }
        return true;
    }

    public bool IsPlayerReady(int actorNumber)
    {
        Player player = GetPlayerByActorNumber(actorNumber);
        if (player == null) return false;

        if (player.CustomProperties.TryGetValue("IsReady", out object isReady))
        {
            return (bool)isReady;
        }

        return false;
    }

    private Player GetPlayerByActorNumber(int actorNumber)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber) return player;
        }
        return null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            OnReadyChanged?.Invoke(this, EventArgs.Empty);
            UpdateUI();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateUI();
    }
}