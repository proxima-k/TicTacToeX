using System;
using Unity.Netcode;
using UnityEngine;

public class TicTacToeGrid : NetworkBehaviour, IInteractable {
    // potentially make a scalable grid with different width and height
    
    // on reset event
    // on place down mark event
    public event EventHandler<OnMarkPlacedEventArgs> OnMarkPlaced;
    public class OnMarkPlacedEventArgs : EventArgs { public int xCoord, yCoord, playerID; }

    public event EventHandler<OnGameStartedEventArgs> OnGameStarted;
    public class OnGameStartedEventArgs : EventArgs { public int playerOneID, playerTwoID; }
    
    public event EventHandler OnGameEnded;
    
    public event EventHandler OnGridReset;
    
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform _visualBox;
    [SerializeField] private Transform _highlightBox;
    
    // -1 = empty
    // players' ID = players' mark
    [SerializeField] private int[,] _grid = 
    new int[3, 3] {
        {-1, -1, -1},
        {-1, -1, -1},
        {-1, -1, -1}
    };
    
    private int[] _playerIDs = new int[2];
    private int _currentPlayerIndex = -1;
    private int _currentTurn = 0;
    
    private bool _isGameInProgress => _currentPlayerIndex != -1;
    private bool _isFocused = false;
    private Transform _playerTransform;

    private void Awake() {
        ResetGrid();
    }

    private void Update() {
        Highlight();
    }

    public void Interact(GameObject interactor) {
        // if interactor is a player
        if (!interactor.TryGetComponent(out PlayerNetwork playerNetwork))
            return;
        
        Vector3 interactorPosition = interactor.transform.position;
        Vector2Int coords = GetCellCoords(interactorPosition);
        coords = ClampCellCoords(coords);
        if (IsCoordsOutOfBounds(coords.x, coords.y))
            return;
        
        // get player data from interactor
        MarkCellServerRpc(coords.x, coords.y, (int) playerNetwork.OwnerClientId);
    }
    
    // false ownership since we want any client to be able to tell the host to set
    [ServerRpc(RequireOwnership = false)]
    public void MarkCellServerRpc(int xCoord, int yCoord, int playerID) {
        if (!_isGameInProgress) {
            Debug.Log("Game has not started yet");
            return;
        }
        
        if (!IsPlayerTurn(playerID)) {
            Debug.Log($"Current player is {_currentPlayerIndex} but player {playerID} tried to mark a cell");
            return;
        }

        if (!IsCellEmpty(xCoord, yCoord)) {
            Debug.Log($"Cell ({xCoord}, {yCoord}) is already filled");
            return;
        }

        _grid[xCoord, yCoord] = playerID;
        MarkCellClientRpc(xCoord, yCoord, playerID);
        _currentTurn++;
        
        OnMarkPlaced?.Invoke(this, new OnMarkPlacedEventArgs {xCoord = xCoord, yCoord = yCoord, playerID = playerID});
        
        // check if player won
        if (TryGetWinner(playerID)) {
            // call win event
            Debug.Log($"Player {_currentPlayerIndex + 1} won");
            EndGame();
            return;
        } else if (_currentTurn == 9) {
            // call draw event
            Debug.Log("Draw");
            EndGame();
            return;
        }
        
        NextPlayer();
    }

    [ClientRpc]
    private void MarkCellClientRpc(int xCoord, int yCoord, int playerID) {
        // if server, in this case a host, ignore since server already has updated the method
        if (IsServer || IsHost)
            return;
        
        _grid[xCoord, yCoord] = playerID;
        OnMarkPlaced?.Invoke(this, new OnMarkPlacedEventArgs {xCoord = xCoord, yCoord = yCoord, playerID = playerID});
    }
    
    private bool TryGetWinner(int playerID) {
        // rows
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (!IsCellPlayer(playerID, i, j))
                    break;
                if (j == 2)
                    return true;
            }            
        }
        // columns
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (!IsCellPlayer(playerID, j, i))
                    break;
                if (j == 2)
                    return true;
            }            
        }
        // top left to bottom right
        for (int i = 0; i < 3; i++) {
            if (!IsCellPlayer(playerID, i, i))
                break;
            if (i == 2)
                return true;
        }
        // top right to bottom left
        for (int i = 0; i < 3; i++) {
            if (!IsCellPlayer(playerID, i, 2 - i))
                break;
            if (i == 2)
                return true;
        }
        
        // todo: check for draw
        
        return false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc() {
        if (_isGameInProgress) {
            Debug.Log("Game has already started");
            return;
        }
        
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 2) {
            Debug.Log("Not enough players to start the game");
            return;
        }
        
        ulong playerOneID = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
        ulong playerTwoID = NetworkManager.Singleton.ConnectedClientsList[1].ClientId;
        
        _playerIDs[0] = (int) playerOneID;
        _playerIDs[1] = (int) playerTwoID;
        
        _currentPlayerIndex = 0;
        _currentTurn = 0;
        ResetGrid();
        Debug.Log("Game is starting!");
        UpdateCurrentPlayerClientRpc(_currentPlayerIndex);
        StartGameClientRpc(_playerIDs[0], _playerIDs[1]);
    }
    
    [ClientRpc]
    public void StartGameClientRpc(int playerOneID, int playerTwoID) {
        _playerIDs[0] = playerOneID;
        _playerIDs[1] = playerTwoID;
        
        OnGameStarted?.Invoke(this, new OnGameStartedEventArgs {playerOneID = playerOneID, playerTwoID = playerTwoID});
    }
    
    private void NextPlayer() {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
        UpdateCurrentPlayerClientRpc(_currentPlayerIndex);
    }
    
    [ClientRpc]
    private void UpdateCurrentPlayerClientRpc(int currentPlayerIndex) {
        _currentPlayerIndex = currentPlayerIndex;
        
        if (_currentPlayerIndex == -1) {
            Debug.Log("Game has ended");
            OnGameEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void EndGame() {
        _currentPlayerIndex = -1;
        _currentTurn = 0;
        
        UpdateCurrentPlayerClientRpc(_currentPlayerIndex);
    }
    
    private void ResetGrid() {
        _grid = new int[3, 3];
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                _grid[x, y] = -1;
            }
        }
        
        ResetGridClientRpc();
        OnGridReset?.Invoke(this, EventArgs.Empty);
    }
    
    [ClientRpc]
    public void ResetGridClientRpc() {
        OnGridReset?.Invoke(this, EventArgs.Empty);
    }

    // Getters =========================================================================================================
   
    public Vector2Int GetCellCoords(Vector3 position) {
        Vector3 offset = new Vector3(cellSize * 1.5f, 0, cellSize * 1.5f);
        Vector3 relativePosition = position - transform.position + offset;
        return new Vector2Int((int)Mathf.Floor(relativePosition.x / cellSize), (int)Mathf.Floor(relativePosition.z / cellSize));
    }
    
    public Vector3 GetCellCenter(int xCoord, int yCoord) {
        Vector3 offset = new Vector3(cellSize * 1.5f, 0, cellSize * 1.5f);
        return new Vector3(xCoord * cellSize + cellSize/2, 0, yCoord * cellSize + cellSize/2) + transform.position - offset;
    }
    
    public Vector3 GetCellCenter(Vector3 position) {
        Vector2 coords = GetCellCoords(position);
        return GetCellCenter((int) coords.x, (int) coords.y);
    }
    
    private bool IsCellEmpty(int xCoord, int yCoord) {
        return _grid[xCoord, yCoord] == -1;
    }
    
    private bool IsCellPlayer(int playerID, int xCoord, int yCoord) {
        return _grid[xCoord, yCoord] == playerID;
    }
    
    private bool IsCoordsOutOfBounds(int xCoord, int yCoord) {
        return xCoord < 0 || xCoord >= 3 || yCoord < 0 || yCoord >= 3;
    }
    
    private bool IsPlayerTurn(int playerID) {
        return _playerIDs[_currentPlayerIndex] == playerID;
    }
    
    private Vector2Int ClampCellCoords(Vector2Int coords) {
        return new Vector2Int(Mathf.Clamp(coords.x, 0, 2), Mathf.Clamp(coords.y, 0, 2));
    }
    
    // Visuals =========================================================================================================
    public void Focus(GameObject interactor) {
        _isFocused = true;
        _playerTransform = interactor.transform;
    }
    
    public void Unfocus(GameObject interactor) {
        _isFocused = false;
    }
    
    private void Highlight() {
        if (!_isGameInProgress) {
            _highlightBox.gameObject.SetActive(false);
            return;
        }
        
        if (!_isFocused) {
            _highlightBox.gameObject.SetActive(false);
            return;
        }
        
        if (_playerTransform == null)
            return;
        
        // set highlight box to the cell that the player is at
        Vector2Int coords = GetCellCoords(_playerTransform.position);
        coords = ClampCellCoords(coords);
        if (IsCoordsOutOfBounds(coords.x, coords.y))
            return;
        
        _highlightBox.position = GetCellCenter(coords.x, coords.y);
        _highlightBox.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        _visualBox.localScale = new Vector3(cellSize * 3, 0.1f, cellSize * 3);
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
        
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                if (_grid[x, y] == 0) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(x * cellSize + cellSize/2, 0, y * cellSize + cellSize/2) + transform.position - offset, 0.2f);
                }
                else if (_grid[x, y] == 1) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(new Vector3(x * cellSize + cellSize/2, 0, y * cellSize + cellSize/2) + transform.position - offset, 0.2f);
                }
            }
        }
    }
#endif
}
