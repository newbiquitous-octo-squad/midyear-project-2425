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
    private float shift = 0.1f;
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
            AddCard(card.Value.suit, card.Value.number);
            object[] parameters = { card.Value.suit, card.Value.number };
            InvokeRemoteMethod("AddCard", parameters: parameters);
        }

        if (Input.GetKeyDown(KeyCode.H)) ShiftLeft();
    }

    [SynchronizableMethod]
    private void ShiftLeft()
    {
        foreach (var card in hand)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x - 0.1f, 0, 0);
        }
    }

    [SynchronizableMethod]
    public void AddCard(string suit, int number)
    {
        ShiftLeft();
            
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        cardObject.transform.SetParent(handObject.transform);
        cardObject.transform.localPosition = new Vector3(shift, 0, 0);
        cardObject.transform.localRotation = Quaternion.identity;
            
        var card = cardObject.AddComponent<Card>();
        card.InitializeCard(suit, number);
        hand.Add(card);
            
        shift += 0.1f;

        // foreach (var c in hand)
        // {
        //     Debug.Log($"Suit: {c.cardSuit}, Number: {c.cardNumber}");
        // }
    }

    // public void AddCard(Card card)
    // {
    //     hand.Add(card);
    //     hand.Sort(delegate(Card x, Card y)
    //     {
    //         return x.cardNumber - y.cardNumber;
    //     });
    //     
    //     
    // }

    
}
