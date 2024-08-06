using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;

    private void Awake() {
        _startHostButton.onClick.AddListener(StartHost);
        _startClientButton.onClick.AddListener(StartClient);
    }
    
    private void StartHost() {
        Debug.Log($"Connection: {NetworkManager.Singleton.StartHost()}");
    }
    private void StartClient() {
        Debug.Log($"Connection: {NetworkManager.Singleton.StartClient()}");
    }
}
