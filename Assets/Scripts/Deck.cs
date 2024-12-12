using System;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;
using Cards;
using Random = System.Random;

public class Deck : AttributesSync
{
    public GameObject cardPrefab;
    [SynchronizableField] private int poopoo = 0;
    [SynchronizableField] private List<(string suit, int number)> deckList = new()
    {
        ("Heart", 1), ("Heart", 2), ("Heart", 3), ("Heart", 4), ("Heart", 5), ("Heart", 6), ("Heart", 7), ("Heart", 8), ("Heart", 9), ("Heart", 10), ("Heart", 11), ("Heart", 12), ("Heart", 13),
        ("Diamond", 1), ("Diamond", 2), ("Diamond", 3), ("Diamond", 4), ("Diamond", 5), ("Diamond", 6), ("Diamond", 7), ("Diamond", 8), ("Diamond", 9), ("Diamond", 10), ("Diamond", 11), ("Diamond", 12), ("Diamond", 13),
        ("Club", 1), ("Club", 2), ("Club", 3), ("Club", 4), ("Club", 5), ("Club", 6), ("Club", 7), ("Club", 8), ("Club", 9), ("Club", 10), ("Club", 11), ("Club", 12), ("Club", 13),
        ("Spade", 1), ("Spade", 2), ("Spade", 3), ("Spade", 4), ("Spade", 5), ("Spade", 6), ("Spade", 7), ("Spade", 8), ("Spade", 9), ("Spade", 10), ("Spade", 11), ("Spade", 12), ("Spade", 13)
    };

void Start()
    {

        foreach (var card in deckList)
        {
            Debug.Log($"Suit: {card.suit}, Number: {card.number}");
        }
    }

    [SynchronizableMethod]
    public (string suit, int number)? DrawCard()
    {
        if (deckList.Count > 0)
        {
            var toReturn = deckList[^1];
            deckList.Remove(toReturn);
            var cardObject = Instantiate(cardPrefab, new Vector3(poopoo,0,0), Quaternion.identity);
            var card = cardObject.AddComponent<Card>();
            card.InitializeCard(toReturn.suit, toReturn.number);
            poopoo++;

            return toReturn;
        }
        else
        {
            Debug.LogWarning("The deck is empty!");
            return null;
        }
    }
    [SynchronizableMethod]
    public void Shuffle()
    {
        Random r = new();
        for (var i = 0; i < deckList.Count; i++)
        {
            var n = r.Next(i, deckList.Count);
            (deckList[i], deckList[n]) = (deckList[n], deckList[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var drawnCard = DrawCard();
            InvokeRemoteMethod("DrawCard");
            if (drawnCard.HasValue)
            {
                Debug.Log($"Drawn Card - Suit: {drawnCard.Value.suit}, Number: {drawnCard.Value.number}");
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            BroadcastRemoteMethod("Shuffle");
        }
    }
}

