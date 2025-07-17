using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_wy : MonoBehaviourPunCallbacks
{
    // UI element showing connection status to Photon
    [Header("Connection Status")]
    public Text connectionStatusText;

    
    // Login UI: player enters nickname
    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject Login_UI_Panel;

    // Panel shown after login: options to create/join rooms
    [Header("Game Options UI Panel")]
    public GameObject GameOptions_UI_Panel;

    // Panel for creating a new room
    [Header("Create Room UI Panel")]
    public GameObject CreateRoom_UI_Panel;
    public InputField roomNameInputField;
    public InputField maxPlayerInputField;

    // Panel shown after joining a room
    [Header("Inside Room UI Panel")]
    public GameObject InsideRoom_UI_Panel;
    public Text roomNameText;
    public Text memberText;
    
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;

    // Panel listing all available rooms
    [Header("Room List UI Panel")]
    public GameObject RoomList_UI_Panel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameObject;

    // Panel for joining a random room
    [Header("Join Random Room UI Panel")]
    public GameObject JoinRandomRoom_UI_Panel;

    // Local cache of room and player UI entries
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods

    void Start()
    {
        // Show login panel on game start
        ActivatePanel(Login_UI_Panel.name);

        // Initialize room list cache
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();

        // Auto sync all players to same scene when MasterClient loads one
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        // Update the connection status text every frame
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }
    #endregion

    #region UI Callbacks

    public void OnLoginButtonClick()
    {
        // Connect to Photon if name is valid
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Playername is invalid");
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        // Use provided room name or generate a random one
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room" + Random.Range(1000, 10000);
        }

        // Set max players and create room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnShowRoomListButtonClicked()
    {
        // Join lobby and show room list
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(RoomList_UI_Panel.name);
    }

    public void OnBackButtonClicked()
    {
        // Leave lobby and return to options panel
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        // Try to join any room
        ActivatePanel(JoinRandomRoom_UI_Panel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClicked()
    {
        // Only the master client can start the game (load scene)
        if (PhotonNetwork.IsMasterClient)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene); // 使用 Loader 枚举加载联机场景
            //PhotonNetwork.LoadLevel("GameScene");
            //Loader.Load(Loader.Scene.GameScene);
            
        }
    }
    public void OnBackToMenuButtonClicked()
    {
        PhotonNetwork.Disconnect(); // 断开 Photon 网络连接
        Loader.Load(Loader.Scene.MainMenuScene); // 返回主菜单
    }

    
    #endregion

    #region Photon Callbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet");
    }

    public override void OnConnectedToMaster()
    {
        // Successfully connected to Photon servers
        Debug.Log(PhotonNetwork.NickName + " is connected to Photon");
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name);
        //ActivatePanel(InsideRoom_UI_Panel.name);
        Loader.Load(Loader.Scene.LXXCharacterSelectScene);

        // Show start button only for MasterClient
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        // Update room info text
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        memberText.text = " Players/Max: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;


        if (playerListGameObjects == null)
            playerListGameObjects = new Dictionary<int, GameObject>();

        AssignTeam(PhotonNetwork.LocalPlayer);
        
        // Instantiate a player list entry UI for each player
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            CreatePlayerListEntry(player);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (playerListGameObjects.ContainsKey(targetPlayer.ActorNumber) && changedProps.ContainsKey("Team"))
        {
            GameObject entry = playerListGameObjects[targetPlayer.ActorNumber];
            string team = changedProps["Team"].ToString();
            entry.transform.Find("PlayerNameText").GetComponent<Text>().text =
                $"{targetPlayer.NickName} [{team}]";
        }
    }

    void AssignTeam(Player player)
    {
        if (player.CustomProperties.ContainsKey("Team"))
            return; // 避免重复设置

        string assignedTeam = (player.ActorNumber % 2 == 0) ? "Red" : "Blue";
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "Team", assignedTeam }
        };
        player.SetCustomProperties(props);
    }


    void CreatePlayerListEntry(Player player)
    {
        GameObject playerListGameobject = Instantiate(playerListPrefab, playerListContent.transform);
        playerListGameobject.transform.localScale = Vector3.one;

        string team = player.CustomProperties.ContainsKey("Team") ? player.CustomProperties["Team"].ToString() : "None";

        playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text =
            $"{player.NickName} [{team}]";

        playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(player.ActorNumber, playerListGameobject);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        memberText.text = " Players/Max: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        CreatePlayerListEntry(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        memberText.text = " Players/Max: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptions_UI_Panel.name);

        foreach (GameObject playerListGameobject in playerListGameObjects.Values)
        {
            Destroy(playerListGameobject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }

        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameobject = Instantiate(roomListEntryPrefab, roomListParentGameObject.transform);
            roomListEntryGameobject.transform.localScale = Vector3.one;

            roomListEntryGameobject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameobject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
            roomListEntryGameobject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

            roomListGameObjects.Add(room.Name, roomListEntryGameobject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomList.Clear();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        string roomName = "Room" + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region Private Methods

    void OnJoinRoomButtonClicked(string _roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(_roomName);
    }

    void ClearRoomListView()
    {
        foreach (var roomListGameobject in roomListGameObjects.Values)
        {
            Destroy(roomListGameobject);
        }
        roomListGameObjects.Clear();
    }

    #endregion

    #region Public Methods

    // Activate only one UI panel at a time
    public void ActivatePanel(string panelToBeActivated)
    {
        Login_UI_Panel.SetActive(panelToBeActivated.Equals(Login_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        CreateRoom_UI_Panel.SetActive(panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
        InsideRoom_UI_Panel.SetActive(panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
        RoomList_UI_Panel.SetActive(panelToBeActivated.Equals(RoomList_UI_Panel.name));
        JoinRandomRoom_UI_Panel.SetActive(panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }

    #endregion
}
