using System;
using TMPro;
using UnityEngine;

public class NameShow : MonoBehaviour
{
    private TextMeshPro nameText;

    private void Awake()
    {
         nameText = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        nameText.text = "jet skibidi";
     }
}
