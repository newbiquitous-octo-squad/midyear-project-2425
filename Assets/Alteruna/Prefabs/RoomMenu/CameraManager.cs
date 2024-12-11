using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Reference to the camera prefab
    public static GameObject cameraPrefab = (GameObject) Resources.Load("prefabs/MainCamera", typeof(GameObject));

        // Function to reinstantiate the camera at specific coordinates
        public static void ReinstantiateCamera(Vector3 position, Quaternion rotation, Transform playerTransform = null)
        {
            // Destroy the current main camera
            Camera currentCamera = Camera.main;
            if (currentCamera != null)
            {
                Destroy(currentCamera.gameObject);
            }

            // Instantiate a new camera from the prefab at the specified coordinates
            GameObject newCamera = Instantiate(cameraPrefab, position, rotation);

            // Ensure the new camera has the "MainCamera" tag
            newCamera.tag = "MainCamera";

            // Parent the camera to the player (if provided)
            if (playerTransform != null)
            {
                newCamera.transform.SetParent(playerTransform);
            }
        }
    }
