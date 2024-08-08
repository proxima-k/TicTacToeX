using System;
using Unity.Netcode;
using UnityEngine;

public class StartGameButton : MonoBehaviour, IInteractable {
    
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    // have two player press the button before it starts. (this will allow more people to join the game and spectate)
    [SerializeField] private MeshRenderer _buttonMeshRenderer;
    [SerializeField] private Material _buttonMaterial;
    [SerializeField] private Material _buttonHighlightMaterial;
    
    public void Interact(GameObject interactor) {
        _ticTacToeGrid.StartGameServerRpc();
    }
    
    public void Focus(GameObject interactor) {
        _buttonMeshRenderer.sharedMaterial = _buttonHighlightMaterial;
    }
    
    public void Unfocus(GameObject interactor) {
        _buttonMeshRenderer.sharedMaterial = _buttonMaterial;
    }
}
