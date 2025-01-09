using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Screen = UnityEngine.Device.Screen;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;
    private ushort port;
    
    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();

        // Default port is 7777
        port = 12345;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(Screen.width/2 - 200, Screen.height / 1.5f,400,400));
        var style = new GUIStyle();
        style.fontSize = 60;
        style.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular");        style.alignment = TextAnchor.MiddleCenter;
        style.normal.background = Texture2D.grayTexture;
        style.normal.textColor = Color.white;
        style.active.background = Texture2D.whiteTexture;
        
        
        if (!_networkManager.IsClient && !_networkManager.IsServer)
        {
            if (GUILayout.Button("Join Game!", style))
            {
                UpdatePort(port);
                _networkManager.StartClient();
            }
            // TODO: DELETE THIS IN PRODUCTION 🙏
            if (GUILayout.Button("Server"))
            {   
                UpdatePort(port);
                _networkManager.StartServer();
            }
        }
        GUILayout.EndArea();
    }

    private void UpdatePort(ushort port) {
        var unityTransport = (UnityTransport) _networkManager.NetworkConfig.NetworkTransport;
        unityTransport.ConnectionData.Port = port;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
