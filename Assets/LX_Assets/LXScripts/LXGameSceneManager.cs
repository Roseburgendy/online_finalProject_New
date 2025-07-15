using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LXGameSceneManager : MonoBehaviourPunCallbacks
{
    public static LXGameSceneManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints = new Transform[4];
    [SerializeField] private List<GameObject> characterPrefabs = new List<GameObject>();
    //[SerializeField] private GameObject characterWrapper;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        string characterId = "";
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object selected))
        {
            characterId = selected.ToString();
        }

        if (string.IsNullOrEmpty(characterId)) return;

        int playerIndex = GetPlayerIndex();
        Vector3 spawnPosition = spawnPoints[playerIndex].position;

        // 直接使用选择的角色名字生成角色（确保角色 prefab 放在 Resources 文件夹中）
        PhotonNetwork.Instantiate(characterId, spawnPosition, Quaternion.identity);
    }


    public void RespawnPlayer()
    {
        // 简单地复用 SpawnPlayer 方法
        SpawnPlayer();
    }

    private GameObject GetCharacterPrefab(string characterId)
    {
        foreach (var data in characterPrefabs)
        {
            if (data.name == characterId)
            {
                return data;
            }
        }
        return null;
    }

    private int GetPlayerIndex()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == PhotonNetwork.LocalPlayer)
            {
                return i;
            }
        }
        return 0;
    }
}
