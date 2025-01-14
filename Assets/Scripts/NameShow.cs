using System;
using TMPro;
using UnityEngine;

public class NameShow : MonoBehaviour
{
    private GameObject playerObject;
    private TextMeshPro nameText;
    public string playerName;
    
    public JoinGame joinManager;

    private void Awake()
    {
         nameText = GetComponent<TextMeshPro>();
         playerObject = transform.parent.gameObject;

         joinManager = playerObject.transform.parent.GetComponentInParent<JoinGame>();
         playerName = joinManager.playerName;
    }

    private void Start()
    {
        nameText.text = playerName;
     }
}
