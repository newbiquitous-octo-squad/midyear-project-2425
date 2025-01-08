using System;
using Unity.Netcode;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

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
        GUILayout.BeginArea(new Rect(Screen.width/2 - 200, Screen.height / 1.5f,400,400));
        var style = new GUIStyle();
        style.fontSize = 60;
        style.alignment = TextAnchor.MiddleCenter;
        
        
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
