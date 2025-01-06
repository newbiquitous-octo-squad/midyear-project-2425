using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//
namespace Cards
{


     public class Card : NetworkBehaviour
     {
//         public string cardSuit;
//         public int cardNumber;
//
//         void Start()
//         {
//             // Find the child "FrontOfCard" and set its material
//             GameObject cardFace = transform.Find("FrontOfCard")?.gameObject;
//             if (cardFace == null)
//             {
//                 Debug.LogError("FrontOfCard child not found!");
//                 return;
//             }
//
//             Debug.Log(cardFace);
//         }
//
//         // Method to initialize the card
//         public void InitializeCard(string suit, int number)
//         {
//             cardSuit = suit;
//             cardNumber = number;
//
//             // Construct the material path based on the suit and number
//             string materialPath = $"Materials/{suit}{number}";
//
//             // Load the material from the Resources folder
//             Material material = Resources.Load<Material>(materialPath);
//             if (material != null)
//             {
//                 // Find the child "FrontOfCard" and set its material
//                 GameObject cardFace = transform.Find("FrontOfCard")?.gameObject;
//                 if (cardFace != null)
//                 {
//                     MeshRenderer meshRenderer = cardFace.GetComponent<MeshRenderer>();
//                     if (meshRenderer != null)
//                     {
//                         meshRenderer.material = material;
//                         Debug.Log($"Material {materialPath} successfully loaded and applied.");
//                     }
//                     else
//                     {
//                         Debug.LogError("MeshRenderer component not found on FrontOfCard!");
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError("FrontOfCard child not found!");
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning($"Material {materialPath} not found in Resources folder.");
//             }
//
//             // Synchronize the card initialization across clients
//             SynchronizeCardInitialization(suit, number);
//         }
//
//         // Method to display card information
//         public void DisplayCardInfo()
//         {
//             Debug.Log("Card Name: " + cardSuit);
//             Debug.Log("Card Value: " + cardNumber);
//         }
//
//         // Method to synchronize card initialization across clients
//         private void SynchronizeCardInitialization(string suit, int number)
//         {
//             cardSuit = suit;
//             cardNumber = number;
//             Debug.Log($"Card initialized with suit: {suit}, number: {number}");
//         }
     }
}