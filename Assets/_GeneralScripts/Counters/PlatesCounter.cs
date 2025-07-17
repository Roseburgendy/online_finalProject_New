using System;
using UnityEngine;
using Photon.Pun;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 5f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (platesSpawnedAmount < platesSpawnedAmountMax)
            {
                photonView.RPC(nameof(RPC_SpawnPlate), RpcTarget.All);
            }
        }
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                WY_KitchenGameMultiplayer.SpawnKitchenObject(plateKitchenObjectSO, player);
                photonView.RPC(nameof(RPC_RemovePlate), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void RPC_SpawnPlate()
    {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    [PunRPC]
    private void RPC_RemovePlate()
    {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}