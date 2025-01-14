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
    private string enteredName = "";
    private bool showGUI = true;

    private void Awake()
    {
        nameText = GetComponent<TextMeshPro>();
        playerObject = transform.parent.gameObject;

        // Random guest names for lazies
        // We should probably make a check for people who purposely put their names as blank
        System.Random random = new System.Random();
        int randomNumber = random.Next(10000, 99999);
        enteredName = "Guest" + randomNumber;
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
    if (IsOwner && playerName.Count == 0 && showGUI)
    {
        var style = new GUIStyle
        {
            fontSize = 50,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState
            {
                textColor = Color.white,
            },
            font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular")
        };

        var backgroundStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = Texture2D.whiteTexture },
            fontSize = 50,
            alignment = TextAnchor.MiddleCenter
        };

        var textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 50,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState
            {
                textColor = Color.black,
                background = Texture2D.whiteTexture
            },
            border = new RectOffset(4, 4, 4, 4)
        };

        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 50,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState
            {
                textColor = Color.white,
            },
            active = new GUIStyleState
            {
                textColor = Color.black,
                background = Texture2D.whiteTexture
            },
            font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular")
        };

        Color originalColor = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.75f); // semi-transparent black background :)
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height), backgroundStyle);
        GUI.color = originalColor;

        GUILayout.BeginArea(new Rect(Screen.width / 2f - 200, Screen.height / 2f - 100, 400, 300));

        GUILayout.Label("Enter name:", style);
        enteredName = GUILayout.TextField(enteredName, textFieldStyle);

        GUILayout.Space(50);

        if (GUILayout.Button("Go!", buttonStyle))
        {
            SubmitNameServerRpc(enteredName);

            transform.parent.position = new Vector3(0, 0, 0);
            transform.parent.Find("Crosshair").gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            showGUI = false;
        }

        GUILayout.EndArea();
        GUILayout.EndArea();
    }
}

    [ServerRpc]
    private void SubmitNameServerRpc(string name)
    {
        playerName.Clear();
        foreach (var character in name)
        {
            playerName.Add(character);
        }
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