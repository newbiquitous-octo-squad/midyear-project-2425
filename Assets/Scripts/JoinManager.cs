using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Screen = UnityEngine.Device.Screen;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;
    private ushort port;
    private string ipAddress;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();

        // Default port is 7777
        port = 12345;
        // Default IP address is 127.0.0.1
        ipAddress = "127.0.0.1";
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 1.5f, 400, 400));
        var style = new GUIStyle();
        style.fontSize = 60;
        style.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular"); style.alignment = TextAnchor.MiddleCenter;
        style.normal.background = Texture2D.grayTexture;
        style.normal.textColor = Color.white;
        style.active.background = Texture2D.whiteTexture;

        if (!_networkManager.IsClient && !_networkManager.IsServer)
        {
            if (GUILayout.Button("Join Game!", style))
            {
                UpdateConnectionData(ipAddress, port);
                _networkManager.StartClient();
            }
            // TODO: DELETE THIS IN PRODUCTION 🙏
            if (GUILayout.Button("Server"))
            {
                UpdateConnectionData(ipAddress, port);
                _networkManager.StartServer();
            }
        }
        GUILayout.EndArea();
    }

    private void UpdateConnectionData(string ipAddress, ushort port)
    {
        ((UnityTransport) _networkManager.NetworkConfig.NetworkTransport).SetConnectionData(ipAddress, port);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
