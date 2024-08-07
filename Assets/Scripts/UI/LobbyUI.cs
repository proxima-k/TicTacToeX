using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private GameObject _lobbyUI;
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;

    private void Awake() {
        _startHostButton.onClick.AddListener(StartHost);
        _startClientButton.onClick.AddListener(StartClient);
    }
    
    private void StartHost() {
        if (NetworkManager.Singleton.StartHost()) {
            Hide();
            Debug.Log("Host started");
            return;
        }
        Debug.Log("Failed to start host");
    }
    private void StartClient() {
        if (NetworkManager.Singleton.StartClient()) {
            Hide();
            Debug.Log("Client started");
            return;
        }
        Debug.Log("Failed to start client");
    }

    private void Show() {
        _lobbyUI.SetActive(true);
    }
    
    private void Hide() {
        _lobbyUI.SetActive(false);
    }
}
