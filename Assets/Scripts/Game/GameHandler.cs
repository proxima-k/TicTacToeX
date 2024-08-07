using Unity.Netcode;
using UnityEngine;

public class GameHandler : NetworkBehaviour
{
    public static GameHandler Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public enum GameState {
        WaitingForPlayers,
        InProgress,
        Ended
    }
    public GameState CurrentGameState => _currentGameState;
    private GameState _currentGameState = GameState.WaitingForPlayers;
    
    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc() {
        // if game is already started, return
        if (_currentGameState == GameState.InProgress)
            return;
        // sets random player to be the first
        
        _currentGameState = GameState.InProgress;
        Debug.Log("Game started");
    }

    public void IsPlayerTurn(int playerIndex) {
        // check if it's the player's turn
    }
    
    private void EndGame() {
        
    }
}
