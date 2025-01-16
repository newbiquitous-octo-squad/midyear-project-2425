using System;
using deckSpace;
using UnityEngine;
using Unity.Netcode;

namespace Cards
{
     public class Card : NetworkBehaviour
     {
         public NetworkVariable<Suit> cardSuit = new();
         public NetworkVariable<int> cardNumber = new();
         private bool _hasInitializedTexture;

         public void InitializeCard(CardType card)
         {
             cardSuit.Value = card.Suit;
             cardNumber.Value = card.Number;
         }

         public static string GetSuitName(Suit suit)
         {
             return suit switch
             {
                 Suit.HEARTS => "Heart",
                 Suit.CLUBS => "Club",
                 Suit.SPADES => "Spade",
                 Suit.DIAMONDS => "Diamond",
                 _ => throw new Exception("hey the how did the happen??")
             };
         }
         
         public void UpdateTextures(Suit suit, int number)
         {
             transform.GetChild(1).GetComponent<MeshRenderer>().material =
                 Resources.Load<Material>($"Materials/{GetSuitName(suit)}{number}");
         }

         private void Update()
         {
             // initialize texture on client if not done already
             if (!_hasInitializedTexture && cardNumber.Value != 0) 
             {
                 UpdateTextures(cardSuit.Value, cardNumber.Value);
                 _hasInitializedTexture = true;
                 name = String.Concat(cardNumber.Value, " ", GetSuitName(cardSuit.Value));
             }
         }
     }
}