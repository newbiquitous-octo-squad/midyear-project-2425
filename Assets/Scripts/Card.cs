using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//
namespace Cards
{


     public class Card : NetworkBehaviour
     {
         private NetworkVariable<string> _cardSuit;
         private NetworkVariable<int> _cardNumber;

         private void Awake()
         {
             // Find the child "FrontOfCard" and set its material
             var cardFace = transform.Find("FrontOfCard")?.gameObject;
             if (cardFace == null)
             {
                 Debug.LogError("FrontOfCard child not found!");
                 return;
             }

             Debug.Log(cardFace);
         }

         // Method to initialize the card
         public void InitializeCard(string suit, int number)
         {
             _cardSuit.Value = suit;
             _cardNumber.Value = number;

             // Construct the material path based on the suit and number
             var materialPath = $"Materials/{suit}{number}";

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
     }
}