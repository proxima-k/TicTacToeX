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
    public void StartGameServerRpc() {
        // if game is already started, return
        if (_currentGameState == GameState.InProgress) {
            Debug.Log("Game already started");
            return;
        }
        
        _currentGameState = GameState.InProgress;
        Debug.Log("Starting game");
        
        // binds two players to a match with their player data
        // sets random player to be the first
    }

    public void IsPlayerTurn(int playerIndex) {
        // check if it's the player's turn
    }
    
    private void EndGame() {
        
    }
}
