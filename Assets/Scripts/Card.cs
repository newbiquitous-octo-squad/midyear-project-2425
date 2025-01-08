using System;
using System.Collections;
using System.Collections.Generic;
using deckSpace;
using UnityEngine;
using Unity.Netcode;
//
namespace Cards
{
     public class Card : NetworkBehaviour
     {
         private NetworkVariable<Suit> _cardSuit = new();
         private NetworkVariable<int> _cardNumber = new();
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
             _cardSuit.Value = card.Suit;
             _cardNumber.Value = card.Number;
         }
         
         public void UpdateTextures(Suit suit, int number)
         {
             string suitName = suit switch
             {
                 Suit.HEARTS => "Heart",
                 Suit.CLUBS => "Club",
                 Suit.SPADES => "Spade",
                 Suit.DIAMONDS => "Diamond",
                 _ => throw new Exception("hey the how did the happen??")
             };

             // Construct the material path based on the suit and number
             var materialPath = $"Materials/{suitName}{number}";

             // Load the material from the Resources folder
             var material = Resources.Load<Material>(materialPath);
             if (material != null)
             {
                 // Find the child "FrontOfCard" and set its material
                 var cardFace = transform.Find("FrontOfCard")?.gameObject;
                 if (cardFace != null)
                 {
                     var meshRenderer = cardFace.GetComponent<MeshRenderer>();
                     if (meshRenderer != null)
                     {
                         meshRenderer.material = material;
                         Debug.Log($"Material {materialPath} successfully loaded and applied.");
                     }
                     else
                     {
                         Debug.LogError("MeshRenderer component not found on FrontOfCard!");
                     }
                 }
                 else
                 {
                     Debug.LogError("FrontOfCard child not found!");
                 }
             }
             else
             {
                 Debug.LogWarning($"Material {materialPath} not found in Resources folder.");
             }

         }

         private void Update()
         {
             // initialize texture on client if not done already
             if (!_hasInitializedTexture && _cardNumber.Value != 0) 
             {
                 UpdateTextures(_cardSuit.Value, _cardNumber.Value);
                 _hasInitializedTexture = true;
             }
         }
     }
}