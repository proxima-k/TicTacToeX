using Unity.Netcode;
using UnityEngine;

public class StartGameButton : MonoBehaviour, IInteractable
{
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    // have two player press the button before it starts. (this will allow more people to join the game and spectate)
    public void Interact(GameObject interactor) {
        _ticTacToeGrid.StartGameServerRpc();
    }
}
