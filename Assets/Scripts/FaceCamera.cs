using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Make the nametag face the camera
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}