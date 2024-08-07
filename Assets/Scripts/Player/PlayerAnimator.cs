using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork _playerNetwork;
    
    private Animator _animator;
    private const string IS_WALKING = "IsWalking";

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!IsOwner)
            return;

        _animator.SetBool(IS_WALKING, _playerNetwork.IsWalking());
    }
}
