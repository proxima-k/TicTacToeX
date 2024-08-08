using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPlacer : MonoBehaviour
{
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    [SerializeField] private Transform _xMarkPrefab;
    [SerializeField] private Transform _oMarkPrefab;

    private void Awake() {
        _ticTacToeGrid.OnMarkPlaced += OnMarkPlaced;
        _ticTacToeGrid.OnGridReset += OnGridReset;
    }

    private void OnMarkPlaced(object sender, TicTacToeGrid.OnMarkPlacedEventArgs e) {
        Vector3 positionToPlace = _ticTacToeGrid.GetCellCenter(e.xCoord, e.yCoord);

        Transform newMark;
        if (e.playerID == 0) {
            newMark = Instantiate(_xMarkPrefab, positionToPlace, Quaternion.identity);
        } else {
            newMark = Instantiate(_oMarkPrefab, positionToPlace, Quaternion.identity);
        }
        newMark.SetParent(transform);
    }

    private void OnGridReset(object sender, System.EventArgs e) {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy() {
        _ticTacToeGrid.OnMarkPlaced -= OnMarkPlaced;
    }
}
