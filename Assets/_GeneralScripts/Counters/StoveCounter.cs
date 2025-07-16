using System;
using UnityEngine;
using Photon.Pun;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state = State.Idle;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        // Only the MasterClient controls the frying/burning logic
        if (!PhotonNetwork.IsMasterClient) return;

        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        WY_KitchenGameMultiplayer.DestroyKitchenObject(GetKitchenObject());
                        WY_KitchenGameMultiplayer.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(fryingRecipeSO.output);

                        photonView.RPC(nameof(RPC_UpdateState), RpcTarget.All, (int)state);
                    }
                    break;

                case State.Fried:
                    burningTimer += Time.deltaTime;
                    float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        WY_KitchenGameMultiplayer.DestroyKitchenObject(GetKitchenObject());
                        WY_KitchenGameMultiplayer.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;
                        photonView.RPC(nameof(RPC_UpdateState), RpcTarget.All, (int)state);
                    }
                    break;

                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(IKitchenObjectParent player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObject.GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;

                    photonView.RPC(nameof(RPC_UpdateState), RpcTarget.All, (int)state);
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        WY_KitchenGameMultiplayer.DestroyKitchenObject(GetKitchenObject());

                        state = State.Idle;
                        photonView.RPC(nameof(RPC_UpdateState), RpcTarget.All, (int)state);
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                photonView.RPC(nameof(RPC_UpdateState), RpcTarget.All, (int)state);
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
        }
    }

    [PunRPC]
    private void RPC_UpdateState(int newState)
    {
        state = (State)newState;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

        if (state == State.Idle || state == State.Burned)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var recipe in fryingRecipeSOArray)
        {
            if (recipe.input == inputKitchenObjectSO)
            {
                return recipe;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var recipe in burningRecipeSOArray)
        {
            if (recipe.input == inputKitchenObjectSO)
            {
                return recipe;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }
}
