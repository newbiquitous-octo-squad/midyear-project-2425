using UnityEngine;
using System.Collections.Generic;
using Cards;
using NUnit.Framework.Internal;

public class Hand : MonoBehaviour
{
    List<Card> hand;
    private float shift = 0.1f;
    public GameObject cardPrefab;
    public GameObject handObject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddCard("Heart", 2);
        }

        if (Input.GetKeyDown(KeyCode.H)) ShiftLeft();
    }

    private void ShiftLeft()
    {
        foreach (var card in hand)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x - 0.1f, 0, 0);
        }
    }

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
