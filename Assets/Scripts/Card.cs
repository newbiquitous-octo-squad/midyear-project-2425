using System;
using System.Collections;
using System.Collections.Generic;
using deckSpace;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

//
namespace Cards
{
     public class Card : NetworkBehaviour
     {
         public NetworkVariable<Suit> cardSuit = new();
         public NetworkVariable<int> cardNumber = new();
         private bool _hasInitializedTexture = false;

         private void Awake()
         {
             // Find the child "FrontOfCard" and set its material
             var cardFace = transform.Find("FrontOfCard")?.gameObject;
             if (cardFace == null)
             {
                 Debug.LogError("FrontOfCard child not found!");
             }
         }

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
             }
         }
     }
}