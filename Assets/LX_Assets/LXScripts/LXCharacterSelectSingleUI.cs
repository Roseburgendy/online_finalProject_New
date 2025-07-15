using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class LXCharacterSelectSingleUI : MonoBehaviour
{
    [SerializeField] private string characterId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;
    [SerializeField] private bool isDefaultCharacter = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
                { "SelectedCharacter", characterId }
            });
        });
    }

    private LXCharacterSelectPlayer FindLocalPlayer()
    {
        LXCharacterSelectPlayer[] allPlayers = FindObjectsOfType<LXCharacterSelectPlayer>();
        foreach (LXCharacterSelectPlayer player in allPlayers)
        {
            if (player.IsLocalPlayer())
            {
                return player;
            }
        }
        return null;
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void Update()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        object selectedCharacter;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out selectedCharacter))
        {
            selectedGameObject.SetActive(selectedCharacter.ToString() == characterId);
        }
        else
        {
            selectedGameObject.SetActive(isDefaultCharacter);
        }
    }
}