using System.Linq;
using Cards;
using deckSpace;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Base setup")] public float walkingSpeed = 3.5f;

    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public GameObject handPrefab;
    public GameObject deckPrefab;

    public bool canMove = true;

    [SerializeField] private float cameraYOffset = 0.4f;

    private CharacterController _characterController;

    private GameObject _crosshair;
    private GameObject _hand;
    private Vector3 _moveDirection = Vector3.zero;
    private Camera _playerCamera;
    private float _rotationX;
    private Card _selectedCard;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();
        _playerCamera.enabled = false;
        _crosshair = GameObject.Find("Crosshair");
    }

    private void Update()
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

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetMouseButtonDown(0)) // Left click
            HandleClickRpc(OwnerClientId, isShiftPressed);

        if (Input.GetMouseButtonDown(1)) // Right click
            HandleRightClickRpc(isShiftPressed);

        if (Input.GetKeyDown(KeyCode.F)) FlipWithFRpc();

        if (Input.GetKey(KeyCode.Q)) RotateCardRpc(-0.5f);

        if (Input.GetKey(KeyCode.E)) RotateCardRpc(0.5f);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMovement();
            HandleRotation();
        }

        ApplyGravity();
    }

    public override void OnNetworkSpawn()
    {
        var nameShow = GetComponentInChildren<NameShow>();
        _playerCamera.enabled = IsOwner;
        if (IsOwner)
        {
            Camera.main!.enabled = false;
            _playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset,
                transform.position.z);
            CreateHandRpc(OwnerClientId);
            ResetCursorState();
        }

        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Server)]
    // note that rpc methods must end with "Rpc". why? idk the compiler demands it tho
    // also note that this code will be run on the server.
    private void CreateHandRpc(ulong ownerId)
    {
        var hand = NetworkManager.SpawnManager.InstantiateAndSpawn(handPrefab.GetComponent<NetworkObject>(), ownerId);
        hand.GetComponent<NetworkObject>().TrySetParent(transform);
        hand.transform.localPosition += new Vector3(0, -0.15f, 0.7f);
    }

    private void HandleMovement()
    {
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);

        var curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        var curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        var movementDirectionY = _moveDirection.y;
        _moveDirection = forward * curSpeedX + right * curSpeedY;

        if (!_characterController.isGrounded) _moveDirection.y = movementDirectionY;

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
    private void HandleClickRpc(ulong clicker, bool isShiftPressed)
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out var hit, 5,
                LayerMask.GetMask("Card", "Deck", "Table")))
            switch (hit.transform.gameObject.layer)
            {
                case 3: // layer 3 is the deck layer
                    ClickOnDeck(hit);
                    break;
                case 6: // layer 6 is the card layer
                    ClickOnCard(clicker, hit);
                    break;
                case 7: // layer 7 is the table layer
                    ClickOnTable(hit, isShiftPressed);
                    break;
            }
    }


    private void ClickOnCard(ulong clicker, RaycastHit hit)
    {
        if (hit.transform.parent != null)
        {
            var hand = hit.transform.parent.GetComponent<Hand>();

            if (hand.GetComponent<NetworkObject>().OwnerClientId != clicker) return;

            hand.centerSelected.Value = !hand.centerSelected.Value;
            hand.Reposition();
        }
        else if (!transform.GetComponentInChildren<Hand>().centerSelected.Value)
        {
            var hand = transform.GetComponentInChildren<Hand>();
            hit.transform.GetComponent<NetworkObject>().TrySetParent(hand.transform);

            var card = hit.transform.GetComponent<Card>();
            hand.hand.Add(new CardType { Number = card.cardNumber.Value, Suit = card.cardSuit.Value });
            if (hand.hand.Count % 2 == 0) hand.center.Value++;

            hand.Reposition();
        }
        else
        {
            var hand = transform.GetComponentInChildren<Hand>();
            var deckObject = NetworkManager.SpawnManager.InstantiateAndSpawn(deckPrefab.GetComponent<NetworkObject>(),
                position: hit.transform.position, rotation: Quaternion.Euler(hit.transform.rotation.eulerAngles + new Vector3(0, 0, 180)));
            var deck = deckObject.gameObject.GetComponent<Deck>();
            var card = hit.transform.GetComponent<Card>();
            var centerCard = hand.transform.GetChild(hand.center.Value);
            
            deck.Empty();
            deck.AddCard(new CardType {Suit = card.cardSuit.Value, Number = card.cardNumber.Value});
            
            centerCard.transform.position = hit.transform.position;
            deck.AddCard(new CardType {Suit = centerCard.GetComponent<Card>().cardSuit.Value, Number = centerCard.GetComponent<Card>().cardNumber.Value});
            centerCard.GetComponent<NetworkObject>().Despawn();
            card.GetComponent<NetworkObject>().Despawn();

            hand.hand.RemoveAt(hand.center.Value);
            hand.centerSelected.Value = false;
            hand.Reposition();
        }
    }

    private void ClickOnTable(RaycastHit hit, bool isShiftPressed, bool flip = false)
    {
        var hand = transform.GetComponentInChildren<Hand>();
        if (!hand.centerSelected.Value) return;

        var card = hand.gameObject.transform.GetChild(hand.center.Value);
        card.GetComponent<NetworkObject>().TryRemoveParent();

        float cardYRotation;
        if (isShiftPressed)
        {
            cardYRotation = transform.localEulerAngles.y;
        }
        else
        {
            var playerYRotation = transform.localEulerAngles.y;
            float[] angles = { 0, 90, 180, 270 };
            cardYRotation = angles.OrderBy(angle => Mathf.Abs(Mathf.DeltaAngle(playerYRotation, angle))).First();
        }

        card.position = hit.point + new Vector3(0, 0.01f, 0);
        card.rotation = flip ? Quaternion.Euler(0, cardYRotation, 180) : Quaternion.Euler(0, cardYRotation, 0);

        hand.RemoveCenterCard();
    }

    private void ClickOnDeck(RaycastHit hit)
    {
        var hand = transform.GetComponentInChildren<Hand>();
        var deck = hit.transform.GetComponent<Deck>();
        if (!hand.centerSelected.Value) 
            hand.DrawCardToHand(deck);
        else
        {
            var card = hand.transform.GetChild(hand.center.Value).GetComponent<Card>();
            deck.AddCard(new CardType {Number = card.cardNumber.Value, Suit = card.cardSuit.Value});
            
            card.GetComponent<NetworkObject>().Despawn();
            
            hand.RemoveCenterCard();
        }
    }


    [Rpc(SendTo.Server)]
    private void HandleRightClickRpc(bool isShiftPressed)
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out var hit, 5,
                LayerMask.GetMask("Card", "Deck", "Table")))
            switch (hit.transform.gameObject.layer)
            {
                case 3:
                    // right-clicking on deck shuffles it
                    hit.transform.GetComponent<Deck>().Shuffle();
                    break;
                case 6:
                    // right-clicking on non-hand card flips it
                    Flip(hit);
                    break;
                case 7:
                    // right-clicking on the table places card face down
                    ClickOnTable(hit, isShiftPressed, true);
                    break;
            }
    }

    private void Flip(RaycastHit hit)
    {
        if (hit.transform.parent != null) return;

        hit.transform.rotation = Quaternion.Euler(hit.transform.rotation.eulerAngles + new Vector3(0, 0, 180));
        if (hit.transform.gameObject.layer == 3)
            hit.transform.position += new Vector3(0, 0.35f, 0);
    }

    [Rpc(SendTo.Server)]
    private void FlipWithFRpc()
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out var hit, 5,
                LayerMask.GetMask("Card", "Deck")))
            Flip(hit);
    }

    [Rpc(SendTo.Server)]
    private void RotateCardRpc(float direction)
    {
        var cameraTransform = transform.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, cameraYOffset, 0),
                cameraTransform.TransformDirection(Vector3.forward), out var hit, 5,
                LayerMask.GetMask("Card")))
        {
            if (hit.transform.parent != null) return;

            hit.transform.rotation =
                Quaternion.Euler(hit.transform.rotation.eulerAngles + new Vector3(0, direction, 0));
        }
    }
}