using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Cards;
using NUnit.Framework.Internal;
using deckSpace;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Hand : NetworkBehaviour
{
    private NetworkList<CardType> _hand = new();
    private NetworkVariable<int> _center = new();
    public GameObject cardPrefab;
    private Deck _deck;
    private PlayerInput _input;
    private int _compressionThreshold = 7;


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

        if (_input.actions["Draw"].triggered)
        {
            DrawCardToHandRpc();
        }

        if (_input.actions["RepositionLeft"].triggered)
        {
            RepositionHandRpc(-1);
        }

        if (_input.actions["RepositionRight"].triggered)
        {
            RepositionHandRpc(1);
        }

    }

    [Rpc(SendTo.Server)]
    void RepositionHandRpc(int offset)
    {
        _center.Value = Mathf.Clamp(_center.Value + offset, 0, _hand.Count-1);
        Reposition();
    }

    // TODO: WHEN REPLACING WITH GENERAL CLICK ON DECK, PASS THROUGH A RAYCAST AND HANDLE IN SERVER AAAAA POOP EMOJI DEVIL EMOJI (my window period broke)
    [Rpc(SendTo.Server)]
    public void DrawCardToHandRpc()
    {
        var card = _deck.DrawCard();
        
        if (card.HasValue) AddCard(card.Value);
    }

    public void AddCard(CardType card)
    {
        var cardObject = NetworkManager.SpawnManager.InstantiateAndSpawn(cardPrefab.GetComponent<NetworkObject>(), OwnerClientId, position: _deck.transform.position + new Vector3(0, _deck.transform.lossyScale.y * 1.5f, 0)).gameObject;
        Debug.Log(_deck.transform.lossyScale.y);
        cardObject.GetComponent<NetworkObject>().TrySetParent(transform);
        
        var cardComponent = cardObject.GetComponent<Card>();
        cardComponent.InitializeCard(card);
        _hand.Add(card);
        if (_hand.Count % 2 == 0)
        {
            _center.Value++;
        }
        
        Reposition();
    }
    
    void Reposition()
    {
        transform.GetChild(_center.Value).transform.localPosition = Vector3.zero;
        transform.GetChild(_center.Value).transform.localRotation = Quaternion.Euler(90, 270, 90);
        var prevRot = 90f;
        for (var i = _center.Value - 1; i >= 0; i--)
        {
            var divideFactor = _hand.Count < _compressionThreshold ? 1 : _center.Value - i;
            var prevPos = transform.GetChild(i + 1).transform.localPosition;
            transform.GetChild(i).transform.localPosition = new Vector3(prevPos.x - 0.1f / divideFactor, prevPos.y - 0.02f / divideFactor,
                prevPos.z + 0.005f / divideFactor);
            transform.GetChild(i).transform.localRotation = Quaternion.Euler(prevRot + 5.0f / Mathf.Sqrt(divideFactor), 270, 90);
            prevRot += 5.0f / Mathf.Sqrt(divideFactor);
        }
    
        prevRot = 90f;
        for (var i = _center.Value + 1; i < _hand.Count; i++)
        {
            var divideFactor = _hand.Count < _compressionThreshold ? 1 : i - _center.Value;
            var prevPos = transform.GetChild(i - 1).transform.localPosition;
            transform.GetChild(i).transform.localPosition = new Vector3(prevPos.x + 0.1f / divideFactor, prevPos.y - 0.02f / divideFactor,
                prevPos.z - 0.005f / divideFactor);
            transform.GetChild(i).transform.localRotation = Quaternion.Euler(prevRot - 5.0f / Mathf.Sqrt(divideFactor), 270, 90);
            prevRot -= 5.0f / Mathf.Sqrt(divideFactor);
        }
    }
}
