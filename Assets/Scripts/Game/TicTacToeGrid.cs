using Unity.Netcode;
using UnityEngine;

public class TicTacToeGrid : NetworkBehaviour {
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform testTransform;
        
    // have events so visual scripts can listen when one of the cell is filled and they can update separately on each client
    // instead of syncing the same object
    
    // 0 = empty
    // 1 = o
    // 2 = x
    private int[,] _grid = new int[3, 3];


    private void Update() {
        Debug.Log(GetCellCoords(testTransform.position));
    }

    // false ownership since we want any client to be able to tell the host to set
    [ServerRpc(RequireOwnership = false)]
    public void SetCellServerRpc(int xCoord, int yCoord, int playerIndex) {
        if (_grid[xCoord, yCoord] != 0)
            return;

        _grid[xCoord, yCoord] = playerIndex;
        // call event
        // owner will receive the event and remove the object it's holding
    }
    
    
    
    private bool TryGetWinner(int playerIndex) {
        // rows
        for (int i = 0; i < 3; i++) 
            if (IsPlayer(playerIndex, i, 0) && (_grid[i, 0] + _grid[i, 1] + _grid[i, 2]) % 3 == 0)
                return true;
        
        // columns
        for (int i = 0; i < 3; i++) 
            if (IsPlayer(playerIndex, 0, i) && (_grid[0, i] + _grid[1, i] + _grid[2, i]) % 3 == 0)
                return true;

        // top left to bottom right
        if (IsPlayer(playerIndex, 0, 0) && (_grid[0, 0] + _grid[1, 1] + _grid[2, 2]) % 3 == 0)
            return true;
        
        // top right to bottom left
        if (IsPlayer(playerIndex, 0, 0) && (_grid[0, 2] + _grid[1, 1] + _grid[2, 0]) % 3 == 0)
            return true;

        return false;
    }

    private bool IsPlayer(int playerIndex, int xCoord, int yCoord) {
        return _grid[xCoord, yCoord] == playerIndex;
    }
    
    public Vector2 GetCellCoords(Vector3 position) {
        Vector3 offset = new Vector3(cellSize * 1.5f, 0, cellSize * 1.5f);
        Vector3 relativePosition = position - transform.position + offset;
        return new Vector2(Mathf.Floor(relativePosition.x / cellSize), Mathf.Floor(relativePosition.z / cellSize));
    }
    
    private void OnDrawGizmos() {
        // draw grid where the center cell is at (0, 0, 0)
        Gizmos.color = Color.black;
        
        Vector3 offset = new Vector3(cellSize * 1.5f, 0, cellSize * 1.5f);
        for (int i = 0; i < 4; i++) {
            Vector3 horizontalStart = new Vector3(0, 0, i * cellSize)           + transform.position - offset;
            Vector3 horizontalEnd = new Vector3(cellSize * 3, 0, i * cellSize)  + transform.position - offset;
            Vector3 verticalStart = new Vector3(i * cellSize, 0, 0)             + transform.position - offset;
            Vector3 verticalEnd = new Vector3(i * cellSize, 0, cellSize * 3)    + transform.position - offset;
            Gizmos.DrawLine(horizontalStart, horizontalEnd);
            Gizmos.DrawLine(verticalStart, verticalEnd);
        }

    }
}
