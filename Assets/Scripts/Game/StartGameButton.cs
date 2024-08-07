using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour, IInteractable
{
    public void Interact() {
        GameHandler.Instance.StartGameServerRpc();
    }
}
