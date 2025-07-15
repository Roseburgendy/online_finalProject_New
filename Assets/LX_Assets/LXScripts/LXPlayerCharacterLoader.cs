using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class LXPlayerCharacterLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform characterRoot;
    [SerializeField] private int targetPlayerIndex; // 指定这个Loader对应哪个玩家索引

    private string currentCharacterId = "";
    private Player targetPlayer;

    private void Start()
    {
        UpdateTargetPlayer();
        
        if (targetPlayer != null && targetPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object selected))
        {
            currentCharacterId = selected.ToString();
            SetCharacter(currentCharacterId);
        }
    }

    private void UpdateTargetPlayer()
    {
        Player[] players = PhotonNetwork.PlayerList;
        if (targetPlayerIndex < players.Length)
        {
            targetPlayer = players[targetPlayerIndex];
        }
        else
        {
            targetPlayer = null;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player changedPlayer, Hashtable changedProps)
    {
        // 只响应目标玩家的角色选择变化
        if (targetPlayer != null && changedPlayer.ActorNumber == targetPlayer.ActorNumber && changedProps.ContainsKey("SelectedCharacter"))
        {
            if (changedProps["SelectedCharacter"] is string newId)
            {
                currentCharacterId = newId;
                SetCharacter(currentCharacterId);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateTargetPlayer();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateTargetPlayer();
    }

    private void SetCharacter(string selectedId)
    {
        if (characterRoot == null) return;

        foreach (Transform child in characterRoot)
        {
            child.gameObject.SetActive(child.name == selectedId);
        }
    }
}