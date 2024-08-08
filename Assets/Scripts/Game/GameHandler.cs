using Unity.Netcode;
using UnityEngine;

public class GameHandler : NetworkBehaviour
{
    public static GameHandler Instance { get; private set; }

    public enum GameState {
        WaitingForPlayers,
        InProgress,
        Ended
    }
    public GameState CurrentGameState => _currentGameState;
    private GameState _currentGameState = GameState.WaitingForPlayers;

    private int[] _playerIDs = new int[2];
    
    private int _currentPlayerIndex = 0;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnected;
    }

    private void NetworkManager_OnClientConnected(ulong clientID) {
        Debug.Log($"Client {clientID} joined!");
    }

    private void NetworkManager_OnClientDisconnected(ulong clientID) {
        Debug.Log($"Client {clientID} left!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(ulong playerOneID, ulong playerTwoID) {
        // if game is already started, return
        if (_currentGameState == GameState.InProgress) {
            Debug.Log("Game already started");
            return;
        }
        
        _currentGameState = GameState.InProgress;
        Debug.Log("Starting game");
        
        _playerIDs[0] = (int) playerOneID;
        _playerIDs[1] = (int) playerTwoID;
        
        // binds two players to a match with their player data
        // sets random player to be the first
    }

    public bool IsPlayerTurn(int playerIndex) {
        return _currentPlayerIndex == playerIndex;
    }
    
    private void EndGame() {
        
    }
}
