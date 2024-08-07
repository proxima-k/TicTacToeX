using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _cameraDistance = 2f;
    [SerializeField] private Transform _model;
    
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _interactRadius = 2f;

    private Vector3 _moveDir;
    // holds player data

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            Camera mainCamera = Camera.main;
            mainCamera.gameObject.transform.SetParent(_cameraHolder);
            mainCamera.transform.localPosition = new Vector3(0, 0, -_cameraDistance);
            mainCamera.transform.localRotation = Quaternion.identity;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update() {
        if (!IsOwner) return;

        
        // player movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 forward = Vector3.Cross(_cameraHolder.right, Vector3.up).normalized;
        Vector3 right = _cameraHolder.right;
        _moveDir = Vector3.Normalize(forward * input.y + right * input.x);
        transform.position += _moveDir * Time.deltaTime * _moveSpeed;
        
        
        // camera rotation
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float xRotation = _cameraHolder.localEulerAngles.x;
        xRotation -= mouseDelta.y * 5f;
        // xRotation = Mathf.Clamp(xRotation, -90, 90);
        _cameraHolder.localRotation = Quaternion.Euler(xRotation, _cameraHolder.eulerAngles.y + mouseDelta.x * 5f, 0);
        
        
        // move direction
        if (_moveDir != Vector3.zero) {
            _model.forward = Vector3.Slerp(_model.forward, _moveDir, 6f * Time.deltaTime);
        }
        
        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Interact");
            Interact();
        }
    }

    private void Interact() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactRadius);
        foreach (Collider collider in colliders) {
            // get componentinparent so we can interact with the parent object
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();

            if (interactable == null) 
                continue;
            
            interactable.Interact(this.gameObject);
        }
    }

    public bool IsWalking() {
        return _moveDir != Vector3.zero;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
    
}
