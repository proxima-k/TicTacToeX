using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkButton : MonoBehaviour, IInteractable {
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    [SerializeField] private int _markIndex; // 0 for O, 1 for X
    
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _highlightMaterial;
    
    public void Interact(GameObject Interactor) {
        _ticTacToeGrid.RegisterPlayerServerRpc(_markIndex);
    }

    public void Focus(GameObject Interactor) {
        foreach (MeshRenderer meshRenderer in _meshRenderers) {
            meshRenderer.sharedMaterial = _highlightMaterial;
        }
    }

    public void Unfocus(GameObject Interactor) {
        foreach (MeshRenderer meshRenderer in _meshRenderers) {
            meshRenderer.sharedMaterial = _defaultMaterial;
        }
    }
}
