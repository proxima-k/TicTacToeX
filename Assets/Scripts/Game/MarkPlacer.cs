using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPlacer : MonoBehaviour
{
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    [SerializeField] private Transform _xMarkPrefab;
    [SerializeField] private Transform _oMarkPrefab;
    
    // 0 = O, 1 = X
    private int _playerOneMarkIndex = 0;
    private int _playerTwoMarkIndex = 1;
    
    private void Awake() {
        _ticTacToeGrid.OnMarkPlaced += OnMarkPlaced;
        _ticTacToeGrid.OnGridReset += OnGridReset;
        _ticTacToeGrid.OnPlayerOneRegistered += OnPlayerOneRegistered;
    }

    private void OnMarkPlaced(object sender, TicTacToeGrid.OnMarkPlacedEventArgs e) {
        Vector3 positionToPlace = _ticTacToeGrid.GetCellCenter(e.xCoord, e.yCoord);
        Transform newMark = Instantiate(GetPlayerMarkPrefab(e.playerIndex), positionToPlace, Quaternion.identity);
        newMark.SetParent(transform);
    }

    private void OnGridReset(object sender, System.EventArgs e) {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    private void OnPlayerOneRegistered(object sender, TicTacToeGrid.OnPlayerOneRegisteredEventArgs e) {
        SetPlayerOneMarkIndex(e.markIndex);
    }


    private void OnDestroy() {
        _ticTacToeGrid.OnMarkPlaced -= OnMarkPlaced;
    }

    // put 0 for O, 1 for X
    public void SetPlayerOneMarkIndex(int playerOneMarkIndex) {
        _playerOneMarkIndex = playerOneMarkIndex;
        _playerTwoMarkIndex = playerOneMarkIndex == 0 ? 1 : 0;
        
        Debug.Log($"Player 1 mark index: {_playerOneMarkIndex}");
        Debug.Log($"Player 2 mark index: {_playerTwoMarkIndex}");
    }

    private Transform GetMarkPrefab(int markIndex) {
        Debug.Log($"Spawning Mark index: {markIndex}");
        if (markIndex == 0) {
            return _oMarkPrefab;
        }
        return _xMarkPrefab;
    }
    
    private Transform GetPlayerMarkPrefab(int playerIndex) {
        if (playerIndex == 0) {
            return GetMarkPrefab(_playerOneMarkIndex);
        }
        return GetMarkPrefab(_playerTwoMarkIndex);
    }
}
