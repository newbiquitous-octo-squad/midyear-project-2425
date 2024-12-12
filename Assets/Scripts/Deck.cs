using UnityEngine;

public class Deck : MonoBehaviour
{
    public (string suit, int num)[] defaultDeck = new (string suit, int num)[52]
    {
        ("Heart", 1), ("Heart", 2), ("Heart", 3), ("Heart", 4), ("Heart", 5), ("Heart", 6), ("Heart", 7), ("Heart", 8), ("Heart", 9), ("Heart", 10), ("Heart", 11), ("Heart", 12), ("Heart", 13),
        ("Diamond", 1), ("Diamond", 2), ("Diamond", 3), ("Diamond", 4), ("Diamond", 5), ("Diamond", 6), ("Diamond", 7), ("Diamond", 8), ("Diamond", 9), ("Diamond", 10), ("Diamond", 11), ("Diamond", 12), ("Diamond", 13),
        ("Club", 1), ("Club", 2), ("Club", 3), ("Club", 4), ("Club", 5), ("Club", 6), ("Club", 7), ("Club", 8), ("Club", 9), ("Club", 10), ("Club", 11), ("Club", 12), ("Club", 13),
        ("Spade", 1), ("Spade", 2), ("Spade", 3), ("Spade", 4), ("Spade", 5), ("Spade", 6), ("Spade", 7), ("Spade", 8), ("Spade", 9), ("Spade", 10), ("Spade", 11), ("Spade", 12), ("Spade", 13)
    };

    void Start()
    {
        foreach (var card in defaultDeck)
        {
            Debug.Log($"Suit: {card.suit}, Number: {card.num}");
        }
    }

    void Update()
    {
        
    }
}
