using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : IEquatable<PlayerData>, INetworkSerializable {
    
    public ulong ClientID => _clientID;
    public FixedString32Bytes PlayerName => _playerName;

    private ulong _clientID;
    private FixedString32Bytes _playerName;

    public PlayerData(ulong clientID) {
        _clientID = clientID;
        _playerName = new FixedString32Bytes("Player" + clientID.ToString());
    }
    
    public PlayerData(ulong clientID, FixedString32Bytes playerName) {
        _clientID = clientID;
        _playerName = playerName;
    }
    
    public void SetPlayerName(FixedString32Bytes playerName) {
        _playerName = playerName;
    }
    
    public bool Equals(PlayerData other) {
        return _clientID == other._clientID;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref _clientID);
        serializer.SerializeValue(ref _playerName);
    }
}
