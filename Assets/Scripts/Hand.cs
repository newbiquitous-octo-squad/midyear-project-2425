using UnityEngine;
using System.Collections.Generic;
using Cards;
public class Hand : MonoBehaviour
{
    List<Card> hand;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(Card card)
    {
        hand.Add(card);
        hand.Sort(delegate(Card x, Card y)
        {
            return x.cardNumber - y.cardNumber;
        });
        
        
    }

    
}
