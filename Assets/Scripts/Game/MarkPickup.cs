using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MarkPickup : NetworkBehaviour, IInteractable
{
    public void Interact(GameObject Interactor) {
        // if (Interactor.TryGetComponent(out PlayerNetwork player)) {
        //     player.AttachObjectServerRpc(NetworkObjectId);
        // }
    }
}
