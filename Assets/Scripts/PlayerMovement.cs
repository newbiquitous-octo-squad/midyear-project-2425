using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 3.5f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    private bool roomsMenuOpen = false;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    private Alteruna.Avatar _avatar;
    private GameObject canvasObject;

    private GameObject rooms;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (_avatar == null)
        {
            Debug.LogError("No avatar ;(");
            return;
        }

        if (!_avatar.IsMe)
            return;

        canvasObject = GameObject.Find("Canvas");

        if (canvasObject == null)
        {
            Debug.LogError("Canvas GameObject not found in the hierarchy.");
        }

        rooms = GameObject.Find("Room Menu");
        if (rooms == null)
        {
            Debug.LogError("rooms aren't");
        }
        rooms.SetActive(false);

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("NO CAMERA");
        }

        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        playerCamera.transform.SetParent(transform);

        ResetCursorState();
    }

    void Update()
{
    if (!_avatar.IsMe)
        return;

    if (playerCamera == null)
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Camera not found!");
            return;
        }
    }
    if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Z))
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (canvasObject != null)
            canvasObject.SetActive(false);
        else
            Debug.LogWarning("Canvas object not found.");
    }
    else if (Cursor.lockState == CursorLockMode.None && Input.GetKeyDown(KeyCode.Z))
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (canvasObject != null)
            canvasObject.SetActive(true);
        else
            Debug.LogWarning("Canvas object not found.");
    }

    if (Cursor.lockState == CursorLockMode.Locked)
    {
        HandleMovement();
        HandleRotation();
    }

    if (Input.GetKeyDown(KeyCode.BackQuote))
    {
        roomsMenuOpen = !roomsMenuOpen;
        rooms.SetActive(roomsMenuOpen);
        canvasObject.SetActive(!roomsMenuOpen);
        Cursor.visible = roomsMenuOpen;
        Cursor.lockState = roomsMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    ApplyGravity();
}

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y = movementDirectionY;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void ResetCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotationX = 0;
    }
}
