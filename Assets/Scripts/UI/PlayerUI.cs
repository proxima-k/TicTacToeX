using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TicTacToeGrid _ticTacToeGrid;
    [SerializeField] private TextMeshProUGUI _playerIndexText;
    [SerializeField] private GameObject _container;

    private void Awake() {
        _ticTacToeGrid.OnGameStarted += OnGameStarted;
        _ticTacToeGrid.OnGameEnded += OnGameEnded;
        
        Hide();
    }

    private void OnGameStarted(object sender, TicTacToeGrid.OnGameStartedEventArgs e) {
        // if local client ID is player 1, set the text to "Player 1"
        if ((int) NetworkManager.Singleton.LocalClientId == e.playerOneID) {
            _playerIndexText.text = "Player 1";
        }
        // if local client ID is player 2, set the text to "Player 2"
        else if ((int) NetworkManager.Singleton.LocalClientId == e.playerTwoID) {
            _playerIndexText.text = "Player 2";
        }
        
        Show();
    }

    private void OnGameEnded(object sender, EventArgs eventArgs) {
        Hide();
    }

    public void Show() {
        _container.SetActive(true);
    }
    
    public void Hide() {
        _container.SetActive(false);
    }
}
