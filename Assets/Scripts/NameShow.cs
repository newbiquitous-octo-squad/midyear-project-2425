using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NameShow : NetworkBehaviour
{
    private GameObject playerObject;
    private TextMeshPro nameText;
    public NetworkList<char> playerName = new();
    public string namefr = "";
    
    private void Awake()
    {
         nameText = GetComponent<TextMeshPro>();
         playerObject = transform.parent.gameObject;
    }

    public override void OnNetworkSpawn()
    {
        transform.parent.Find("Crosshair").gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        base.OnNetworkSpawn();
    }

    private void OnGUI()
    {
        if (IsOwner && playerName.Count == 0)
        {
            var style = new GUIStyle
            {
                fontSize = 100,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                }
                
            };
            GUILayout.BeginArea(new Rect(Screen.width / 2f - 200, Screen.height / 1.5f, 400, 400));
            
            GUILayout.Label("Enter name: ", style: style);
            var name = GUILayout.TextField("", style);
            

            if (GUILayout.Button("Go!", style))
            {
                foreach (var character in name)
                {
                    playerName.Add(character);
                }

                transform.parent.position = new Vector3(0, 0, 0);
                transform.parent.Find("Crosshair").gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        GUILayout.EndArea();
    }

    private void Update()
    {
        if (namefr.Length != 0) return;

        foreach (var character in playerName)
        {
            namefr += character;
        }

        nameText.text = namefr;
    }
}
