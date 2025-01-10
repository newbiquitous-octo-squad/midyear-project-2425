using UnityEngine;
using Cards;
using deckSpace;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Hand : NetworkBehaviour
{
    public NetworkList<CardType> hand = new();
    public NetworkVariable<int> center = new();
    public NetworkVariable<bool> centerSelected = new();
    public GameObject cardPrefab;
    private Deck _deck;
    private PlayerInput _input;
    private int _compressionThreshold = 12;
    private float playerCameraYOffset = 0.4f;


    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _deck = GameObject.Find("Deck").GetComponent<Deck>();
    }

    public override void OnNetworkSpawn()
    {
        _input.enabled = IsOwner;
        base.OnNetworkSpawn();
    }


    void Update()
    {
        if (!IsOwner) return;

        // if (_input.actions["Draw"].triggered)
        // {
        //     DrawCardToHandRpc();
        // }

        if (_input.actions["RepositionLeft"].triggered)
        {
            RepositionHandRpc(-1);
        }

        if (_input.actions["RepositionRight"].triggered)
        {
            RepositionHandRpc(1);
        }

        if (_input.actions["LeftClick"].triggered)
        {
            ClickOnDeckRpc();
        }

    }

    [Rpc(SendTo.Server)]
    void RepositionHandRpc(int offset)
    {
        centerSelected.Value = false;
        center.Value = Mathf.Clamp(center.Value + offset, 0, hand.Count-1);
        Reposition();
    }

    [Rpc(SendTo.Server)]
    void ClickOnDeckRpc()
    {
        var cameraTransform = transform.parent.GetComponentInChildren<Camera>().transform;
        if (Physics.Raycast(cameraTransform.position + new Vector3(0, playerCameraYOffset, 0), cameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, 5, LayerMask.GetMask("Deck")))
        {
            DrawCardToHand(hit.transform.GetComponent<Deck>());
        }
    }


    public void DrawCardToHand(Deck deck)
    {
        var card = deck.DrawCard();
        
        if (card.HasValue) AddCard(card.Value);
    }

    public void AddCard(CardType card)
    {
        var cardObject = NetworkManager.SpawnManager.InstantiateAndSpawn(cardPrefab.GetComponent<NetworkObject>(), OwnerClientId, position: _deck.transform.position + new Vector3(0, _deck.transform.lossyScale.y * 1.5f, 0)).gameObject;
        cardObject.GetComponent<NetworkObject>().TrySetParent(transform);
        
        var cardComponent = cardObject.GetComponent<Card>();
        cardComponent.InitializeCard(card);
        hand.Add(card);
        if (hand.Count % 2 == 0)
        {
            center.Value++;
        }
        
        Reposition();
    }
    
    public void Reposition()
    {
        transform.GetChild(center.Value).transform.localPosition = Vector3.zero;
        transform.GetChild(center.Value).transform.localRotation = Quaternion.Euler(-90, -270, -90);
        var prevRot = -90f;
        for (var i = center.Value - 1; i >= 0; i--)
        {
            var divideFactor = hand.Count < _compressionThreshold ? 1 : center.Value - i;
            var prevPos = transform.GetChild(i + 1).transform.localPosition;
            transform.GetChild(i).transform.localPosition = new Vector3(prevPos.x - 0.1f / divideFactor, prevPos.y - 0.02f / divideFactor,
                prevPos.z + 0.005f / divideFactor);
            transform.GetChild(i).transform.localRotation = Quaternion.Euler(prevRot + 5.0f / Mathf.Sqrt(divideFactor), 270, 90);
            prevRot += 5.0f / Mathf.Sqrt(divideFactor);
        }
    
        prevRot = -90f;
        for (var i = center.Value + 1; i < hand.Count; i++)
        {
            var divideFactor = hand.Count < _compressionThreshold ? 1 : i - center.Value;
            var prevPos = transform.GetChild(i - 1).transform.localPosition;
            transform.GetChild(i).transform.localPosition = new Vector3(prevPos.x + 0.1f / divideFactor, prevPos.y - 0.02f / divideFactor,
                prevPos.z - 0.005f / divideFactor);
            transform.GetChild(i).transform.localRotation = Quaternion.Euler(prevRot - 5.0f / Mathf.Sqrt(divideFactor), 270, 90);
            prevRot -= 5.0f / Mathf.Sqrt(divideFactor);
        }

        transform.GetChild(center.Value).transform.localPosition +=
            centerSelected.Value ? new Vector3(0, 0.05f, 0) : Vector3.zero;
    }
}
