using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardSuit;
    public int cardNumber;

    // Method to initialize the card
    public void Initialize(string suit, int number)
    {
        cardSuit = suit;
        cardNumber = number;
        
    }

    // Method to display card information
    public void DisplayCardInfo()
    {
        Debug.Log("Card Name: " + cardSuit);
        Debug.Log("Card Value: " + cardNumber);
    }
}
