using System;
using UnityEngine;
using Cards;
using Unity.Netcode;
using Random = System.Random;

namespace deckSpace
{

    public enum Suit
    {
        HEARTS, 
        DIAMONDS,
        SPADES,
        CLUBS
    }

    public struct CardType : INetworkSerializable, IEquatable<CardType>
    {
        public Suit Suit;
        public int Number;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out Suit);
                reader.ReadValueSafe(out Number);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(Suit);
                writer.WriteValueSafe(Number);
            }
        }

        public bool Equals(CardType other)
        {
            return Suit == other.Suit && Number == other.Number;
        }
    }

     public class Deck : NetworkBehaviour
     {
         public GameObject cardPrefab;

         private int _textureFrames;
         
         private Rigidbody _rigidbody;

         private (Suit suit, int number)[] _defaultDeck = {
             (Suit.HEARTS, 1), (Suit.HEARTS, 2), (Suit.HEARTS, 3), (Suit.HEARTS, 4), (Suit.HEARTS, 5), (Suit.HEARTS, 6), (Suit.HEARTS, 7),
             (Suit.HEARTS, 8), (Suit.HEARTS, 9), (Suit.HEARTS, 10), (Suit.HEARTS, 11), (Suit.HEARTS, 12), (Suit.HEARTS, 13),
             (Suit.DIAMONDS, 1), (Suit.DIAMONDS, 2), (Suit.DIAMONDS, 3), (Suit.DIAMONDS, 4), (Suit.DIAMONDS, 5), (Suit.DIAMONDS, 6),
             (Suit.DIAMONDS, 7), (Suit.DIAMONDS, 8), (Suit.DIAMONDS, 9), (Suit.DIAMONDS, 10), (Suit.DIAMONDS, 11), (Suit.DIAMONDS, 12),
             (Suit.DIAMONDS, 13),
             (Suit.CLUBS, 1), (Suit.CLUBS, 2), (Suit.CLUBS, 3), (Suit.CLUBS, 4), (Suit.CLUBS, 5), (Suit.CLUBS, 6), (Suit.CLUBS, 7), (Suit.CLUBS, 8),
             (Suit.CLUBS, 9), (Suit.CLUBS, 10), (Suit.CLUBS, 11), (Suit.CLUBS, 12), (Suit.CLUBS, 13),
             (Suit.SPADES, 1), (Suit.SPADES, 2), (Suit.SPADES, 3), (Suit.SPADES, 4), (Suit.SPADES, 5), (Suit.SPADES, 6), (Suit.SPADES, 7),
             (Suit.SPADES, 8), (Suit.SPADES, 9), (Suit.SPADES, 10), (Suit.SPADES, 11), (Suit.SPADES, 12), (Suit.SPADES, 13)
         };
         private NetworkList<CardType> _deckList = new();

         void Awake()
         {
             _rigidbody = gameObject.GetComponent<Rigidbody>();
             if (_rigidbody == null)
             {
                 _rigidbody = gameObject.AddComponent<Rigidbody>();
             }

             _rigidbody.useGravity = true;

             _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
         }

         private Index LastIndex()
         {
             return transform.rotation.z == 0f ? ^1 : 0;
         }

         public override void OnNetworkSpawn()
         {
             // server adds default cards to initial deck
             if (IsServer)
             {
                 ResetDeck();
                 UpdateTexture();
             }
             
             base.OnNetworkSpawn();
         }

         public CardType? DrawCard()
         {
             if (_deckList.Count > 0)
             {
                 var toReturn = _deckList[LastIndex()];
                 RemoveCard();

                 return toReturn;
             }
             else
             {
                 Debug.LogWarning("The deck is empty!");
                 return null;
             }
         }

         private void RemoveCard()
         {
             if (_deckList.Count <= 0) return;
             
             _deckList.Remove(_deckList[LastIndex()]);
             UpdateTexture();

         }

         public void Shuffle()
         {
             Random r = new();
             for (var i = 0; i < _deckList.Count; i++)
             {
                 var n = r.Next(i, _deckList.Count);
                 (_deckList[i], _deckList[n]) = (_deckList[n], _deckList[i]);
             }
             UpdateTexture();

         }

         public void ResetDeck()
         {
             foreach (var card in _defaultDeck)
             {
                 _deckList.Add(new CardType {Number = card.number, Suit = card.suit});
             }
         }

         void UpdateTexture(bool checkIfRemove = true)
         {
             transform.localScale = _deckList.Count > 0 ? new Vector3(0.05f, _deckList.Count / 1040f, 0.05f) : Vector3.zero;
             if (_deckList.Count == 0 && checkIfRemove)
             {
                 GetComponent<NetworkObject>().Despawn();
                 return;
             }

             if (_deckList.Count == 1 && checkIfRemove)
             {
                 var card = NetworkManager.SpawnManager.InstantiateAndSpawn(cardPrefab.GetComponent<NetworkObject>(), position: transform.position, rotation: Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, 0, 180)));
                 card.GetComponent<Card>().InitializeCard(_deckList[0]);
                 GetComponent<NetworkObject>().Despawn();
                 return;
             }

             var bottomCard = _deckList[0];
             transform.GetChild(0).GetComponent<MeshRenderer>().material =
                     Resources.Load<Material>($"Materials/{Card.GetSuitName(bottomCard.Suit)}{bottomCard.Number}");
         }

         public void AddCard(CardType card)
         {
             _deckList.Insert(LastIndex().Value == 0 ? 0 : _deckList.Count, card);
             UpdateTexture(false);
         }

         public void Empty()
         {
             while (_deckList.Count > 0)
             {
                 _deckList.RemoveAt(0);
             }
         }

         void Update()
         {
             if (_textureFrames < 10)
             {
                 _textureFrames++;
                 UpdateTexture(false);
             }
         }
     }

}