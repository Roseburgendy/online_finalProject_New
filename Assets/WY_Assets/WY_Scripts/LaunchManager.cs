using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject enterGamePanel;
    public GameObject connectionStatusPanel;
    public GameObject lobbyPanel;


    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        enterGamePanel.SetActive(true);
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region Public Methods

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // Connect to the server
            PhotonNetwork.ConnectUsingSettings();
            
            // Set
            connectionStatusPanel.SetActive(true);
            enterGamePanel.SetActive(false);
        }

    }

    // Add to random room
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " CONNECTED to Server");
        // Enable lobby panel
        lobbyPanel.SetActive(true);
        connectionStatusPanel.SetActive(false);
    }

    public override void OnConnected()
    {
        Debug.Log("CONNECTED to Internet");
    }


    // Fail to join random room message
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);

        // if join room failed, create and join a room
        createAndJoinRoom();
    }

    // Server joined the room
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to "+ PhotonNetwork.CurrentRoom.Name);

    }

    // Other player joined the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "Joined to "+ PhotonNetwork.CurrentRoom.Name+" "+
            PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion

    #region private methods

    // CREATE AND JOIN A ROOM
    void createAndJoinRoom()
    {
        // GENERATE RANDOM ROOM NAME
        string randomRoomName = "Room" + Random.Range(0, 10000);

        // CREATE RULES FOR THE ROOM
        RoomOptions roomOptions = new RoomOptions();
        // SET UP ROOM RULE
        roomOptions.IsOpen = true;// is the room open?
        roomOptions.IsVisible = true;// is the room visible in the lobby?
        roomOptions.MaxPlayers = 20;// max player allowed

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions );
    }

    public void StartGame()
    {
        // AFTER JOIN THE ROOM, ENTER GAME SCENE
        PhotonNetwork.LoadLevel("Game Scene");
    }

    #endregion

}
