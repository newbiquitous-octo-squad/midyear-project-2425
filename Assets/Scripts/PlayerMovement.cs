using Cards;
using deckSpace;
using Unity.Netcode;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

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
    private Card _selectedCard;

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
            CreateHandRpc(OwnerClientId);
            ResetCursorState();
        }
        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Server)] 
    // note that rpc methods must end with "Rpc". why? idk the compiler demands it tho
    // also note that this code will be run on the server.
    void CreateHandRpc(ulong ownerId)
    {
        var hand = NetworkManager.SpawnManager.InstantiateAndSpawn(handPrefab.GetComponent<NetworkObject>(), ownerClientId: ownerId);
        hand.GetComponent<NetworkObject>().TrySetParent(transform);
        hand.transform.localPosition += new Vector3(0, -0.1f, 0.75f);
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

    if (Input.GetMouseButtonDown(0)) // Left click
    {
        HandleClickRpc(OwnerClientId);
    }

    if (Input.GetMouseButtonDown(1)) // Right click
    {
        HandleRightClickRpc();
    }

    if (Input.GetKeyDown(KeyCode.F))
    {
        FlipWithFRpc();
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

    [Rpc(SendTo.Server)]
    void HandleClickRpc(ulong clicker)
    {
        // cast the ray
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, 5,
                LayerMask.GetMask("Card", "Deck", "Table")))
        {
            switch (hit.transform.gameObject.layer)
            {
                case 3: // layer 3 is the deck layer
                    ClickOnDeck(hit);
                    break;
                case 6: // layer 6 is the card layer
                    ClickOnCard(clicker, hit);
                    break;
                case 7: // layer 7 is the table layer
                    ClickOnTable(hit);
                    break;
            }
        }
    }
    
    
    void ClickOnCard(ulong clicker, RaycastHit hit)
    {
        var hand = hit.transform.parent.GetComponent<Hand>();
        
        if (hand.GetComponent<NetworkObject>().OwnerClientId != clicker) return;
        
        hand.centerSelected.Value = !hand.centerSelected.Value;
        hand.Reposition();
    }

    void ClickOnTable(RaycastHit hit, bool flip = false)
    {
        var hand = transform.GetComponentInChildren<Hand>();
        if (!hand.centerSelected.Value) return;
        
        var card = hand.gameObject.transform.GetChild(hand.center.Value);
        card.GetComponent<NetworkObject>().TryRemoveParent();
        
        card.position = hit.point;
        card.rotation = flip ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
        
        hand.centerSelected.Value = false;
        hand.hand.RemoveAt(hand.center.Value);
        hand.center.Value = Mathf.Max(hand.center.Value - 1, 0);
        hand.Reposition();
    }
    
    void ClickOnDeck(RaycastHit hit)
    {
        transform.GetComponentInChildren<Hand>().DrawCardToHand(hit.transform.GetComponent<Deck>());
    }


    [Rpc(SendTo.Server)]
    void HandleRightClickRpc()
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, 5,
                LayerMask.GetMask("Card", "Deck", "Table")))
        {
            switch (hit.transform.gameObject.layer)
            {
                case 3:
                    // right-clicking on deck shuffles it
                    hit.transform.GetComponent<Deck>().Shuffle();
                    break;
                case 6:
                    // right-clicking on non-hand card flips it
                    FlipCard(hit);
                    break;
                case 7:
                    // right-clicking on the table places card face down
                    ClickOnTable(hit, true);
                    break;
            }
        }
    }

    void FlipCard(RaycastHit hit)
    {
        if (hit.transform.parent != null) return;

        hit.transform.rotation = Quaternion.Euler(hit.transform.rotation.eulerAngles + new Vector3(0, 0, 180));
    }

    [Rpc(SendTo.Server)]
    void FlipWithFRpc()
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, 5,
                LayerMask.GetMask("Card")))
        {
            FlipCard(hit);
        }
        
    }
}