using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class FaceCamera : NetworkBehaviour
{
    private Camera _playerCamera;

    void Start()
    {
        // Find the main camera
        _playerCamera = Camera.allCameras.First();
        
        if (_playerCamera == null)
        {
            Debug.LogError("Main camera not found!");
            Debug.Log(_playerCamera);
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