using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform _cameraHolder;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            Camera mainCamera = Camera.main;
            mainCamera.gameObject.transform.SetParent(_cameraHolder);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
    }

    private void Update() {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir.z = 1;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1;
        if (Input.GetKey(KeyCode.D)) moveDir.x = 1;
        
        transform.position += moveDir.normalized * Time.deltaTime * 5f;
        
        // camera rotation
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseDelta.x * 5f, 0);
        
        float xRotation = _cameraHolder.localEulerAngles.x;
        xRotation -= mouseDelta.y * 5f;
        // xRotation = Mathf.Clamp(xRotation, -90, 90);
        _cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
