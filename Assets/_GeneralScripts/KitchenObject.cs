using System;
using UnityEngine;
using Photon.Pun;

public class KitchenObject : MonoBehaviourPun {

    // The ScriptableObject representing data for this kitchen object (e.g. tomato, plate, meat)
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // The object that currently holds this kitchen object (could be a player or a counter)
    private IKitchenObjectParent kitchenObjectParent;

    // Component that keeps this object visually following its parent
    private FollowTransform followTransform;

    protected virtual void Awake() {
        // Get the FollowTransform component used to visually follow the parent
        followTransform = GetComponent<FollowTransform>();
    }

    // Returns the ScriptableObject data for this kitchen object
    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    // Assigns a new parent (e.g. player) to this kitchen object
    public void SetKitchenObjectParent(IKitchenObjectParent newParent) {
        // If currently held by a parent, clear the previous reference
        if (kitchenObjectParent != null) {
            kitchenObjectParent.ClearKitchenObject();
        }

        kitchenObjectParent = newParent;

        // Sanity check: log error if new parent already has a kitchen object
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        // Set this object as the parent's held object
        kitchenObjectParent.SetKitchenObject(this);

        // Make the object visually follow the new parent's designated hold point
        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());

        // Inform all other clients to also update this object’s parent
        photonView.RPC(nameof(RPC_SetParent), RpcTarget.Others, ((MonoBehaviour)newParent).GetComponent<PhotonView>().ViewID);
    }

    // Called remotely to sync the parent object across clients
    [PunRPC]
    private void RPC_SetParent(int parentViewId) {
        PhotonView parentView = PhotonView.Find(parentViewId);
        if (parentView == null) {
            Debug.LogWarning("Parent view not found.");
            return;
        }

        // Clear current parent if set
        if (kitchenObjectParent != null) {
            kitchenObjectParent.ClearKitchenObject();
        }

        // Set and sync new parent
        IKitchenObjectParent newParent = parentView.GetComponent<IKitchenObjectParent>();
        kitchenObjectParent = newParent;
        kitchenObjectParent.SetKitchenObject(this);
        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    // Returns the current parent of this kitchen object
    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    // Destroys the object across the network (only the owning client can call this)
    public void DestroySelf() {
        if (photonView.IsMine) {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // Clear reference on parent (used when object is destroyed)
    public void ClearKitchenObjectOnParent() {
        kitchenObjectParent?.ClearKitchenObject();
    }

    // Tries to cast this object to a PlateKitchenObject if applicable
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject plate) {
            plateKitchenObject = plate;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }

    // Static helper method to spawn a new kitchen object on the network
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        // Note: prefab must be in a "Resources" folder for Photon to instantiate
        GameObject kitchenObjectPrefab = kitchenObjectSO.prefab;
        Transform followTarget = kitchenObjectParent.GetKitchenObjectFollowTransform();

        // Spawn the object on the network
        GameObject kitchenObjectInstance = PhotonNetwork.Instantiate(kitchenObjectPrefab.name, followTarget.position, Quaternion.identity);

        // Set its parent immediately after instantiation
        KitchenObject kitchenObject = kitchenObjectInstance.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    // Static helper method to destroy the kitchen object
    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        kitchenObject.DestroySelf();
    }
}
