using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    private void Awake() {
        _inputActions = new PlayerInputActions();
        
    }

    private void OnEnable() {
        _inputActions.Movement.Enable();
    }
    
    private void OnDisable() {
        _inputActions.Movement.Disable();
    }
}
