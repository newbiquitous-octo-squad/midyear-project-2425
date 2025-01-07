using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 3.5f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public GameObject handPrefab;


    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0;

    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera _playerCamera;

    private GameObject _crosshair;
    private GameObject _hand;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();
        _playerCamera.enabled = false;
        _crosshair = GameObject.Find("Crosshair");
    }

    public override void OnNetworkSpawn()
    {
        _playerCamera.enabled = IsOwner;
        if (IsOwner)
        {
            Camera.main!.enabled = false;
            _playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            CreateHandRpc();
            ResetCursorState();
        }
        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Server)] 
    // note that rpc methods must end with "Rpc". why? idk the compiler demands it tho
    // also note that this code will be run on the server.
    void CreateHandRpc()
    {
        var hand = NetworkManager.SpawnManager.InstantiateAndSpawn(handPrefab.GetComponent<NetworkObject>());
        hand.GetComponent<NetworkObject>().TrySetParent(transform);
    }

    void Update()
    {
    if (!IsOwner)
        return;

    if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Z))
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (_crosshair != null)
            _crosshair.SetActive(false);
        else
            Debug.LogWarning("Canvas object not found.");
    }
    else if (Cursor.lockState == CursorLockMode.None && Input.GetKeyDown(KeyCode.Z))
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (_crosshair != null)
            _crosshair.SetActive(true);
        else
            Debug.LogWarning("Canvas object not found.");
    }

    if (Cursor.lockState == CursorLockMode.Locked)
    {
        HandleMovement();
        HandleRotation();
    }

    ApplyGravity();
}

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!_characterController.isGrounded)
        {
            _moveDirection.y = movementDirectionY;
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (canMove && _playerCamera != null)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded) return;
        
        _moveDirection.y -= gravity * Time.deltaTime;
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private void ResetCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _rotationX = 0;
    }
}