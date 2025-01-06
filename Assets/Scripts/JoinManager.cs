using System;
using Unity.Netcode;
using UnityEngine;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 300,800,800));
        var style = new GUIStyle();
        style.fontSize = 60;
        
        
        if (!_networkManager.IsClient && !_networkManager.IsServer)
        {
            if (GUILayout.Button("Join Game!", style))
            {
                _networkManager.StartClient();
            }
            // TODO: DELETE THIS IN PRODUCTION 🙏
            if (GUILayout.Button("Server"))
            {
                _networkManager.StartServer();
            }
        }
        GUILayout.EndArea();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
