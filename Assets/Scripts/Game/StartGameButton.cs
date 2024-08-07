using UnityEngine;

public class StartGameButton : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor) {
        GameHandler.Instance.StartGameServerRpc();
    }
}
