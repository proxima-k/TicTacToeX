using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _interactRadius = 2f;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            Camera mainCamera = Camera.main;
            mainCamera.gameObject.transform.SetParent(_cameraHolder);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update() {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDir += transform.right;
        
        transform.position += moveDir.normalized * Time.deltaTime * 5f;
        
        // camera rotation
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseDelta.x * 5f, 0);
        
        float xRotation = _cameraHolder.localEulerAngles.x;
        xRotation -= mouseDelta.y * 5f;
        // xRotation = Mathf.Clamp(xRotation, -90, 90);
        _cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
        
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
            
            interactable.Interact();
        }
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}
