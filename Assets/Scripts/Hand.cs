using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Cards;
using NUnit.Framework.Internal;
using deckSpace;
using Alteruna;
using Avatar = Alteruna.Avatar;

public class Hand : AttributesSync
{
    List<Card> hand;
    private int center;
    public GameObject cardPrefab;
    public GameObject handObject;
    private Deck deck;
    private Avatar _avatar;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = new List<Card>();
        _avatar = transform.parent.GetComponent<Avatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_avatar.IsMe) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            deck = GameObject.Find("Deck(Clone)").GetComponent<Deck>();
            var card = deck.DrawCard();
            if (card == null) return;
            AddCard(card.Value.suit, card.Value.number);
            object[] parameters = { card.Value.suit, card.Value.number };
            InvokeRemoteMethod("AddCard", parameters: parameters);
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            center = Mathf.Max(0, center - 1);
            BroadcastMessage("Reposition");
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            center = Mathf.Min(hand.Count-1, center + 1);
            BroadcastMessage("Reposition");
        }

    }

    [SynchronizableMethod]
    public void AddCard(string suit, int number)
    {
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        cardObject.transform.SetParent(handObject.transform);
        cardObject.transform.localPosition = new Vector3(0, 0, 0);
        cardObject.transform.localRotation = Quaternion.identity;
            
        var card = cardObject.AddComponent<Card>();
        card.InitializeCard(suit, number);
        hand.Add(card);
        if (hand.Count % 2 == 0)
        {
            center++;
        }
        
        Reposition();
    }

    // TODO: Proper repositioning synchronization
    [SynchronizableMethod]
    void Reposition()
    {
        hand[center].transform.localPosition = Vector3.zero;
        hand[center].transform.localRotation = Quaternion.identity;
        for (var i = center - 1; i >= 0; i--)
        {
            var dist = center - i;
            var prevPos = hand[i + 1].transform.localPosition;
            hand[i].transform.localPosition = new Vector3(prevPos.x-0.1f/dist, prevPos.y-0.005f/dist, prevPos.z+0.005f/dist);
        }
        for (var i = center + 1; i < hand.Count; i++)
        {
            var dist = i - center;
            var prevPos = hand[i - 1].transform.localPosition;
            hand[i].transform.localPosition = new Vector3(prevPos.x+0.1f/dist, prevPos.y-0.005f/dist, prevPos.z-0.005f/dist);
        }

        // if (hand.Count > 1)
        // {
        //     foreach (var c in hand)
        //     {
        //         c.transform.localRotation = Quaternion.Euler(1, 0, 0);
        //     }
        //
        // }

    }
}
