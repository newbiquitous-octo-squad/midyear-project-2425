using Unity.Netcode;
using UnityEngine;

// TODO: FIX WITH ARR PEE SEE 😈 omega

public class FaceCamera : NetworkBehaviour
{
    private Camera _playerCamera;

    void Start()
    {
        // Find the main camera
        _playerCamera = transform.parent.GetComponentInChildren<Camera>();

        if (_playerCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    void Update()
    {
        if (_playerCamera != null)
        {
            // Make the nametag face the camera
            transform.LookAt(transform.position + _playerCamera.transform.rotation * Vector3.forward,
                _playerCamera.transform.rotation * Vector3.up);
        }
    }
}