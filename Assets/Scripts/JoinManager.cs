using System;
using System.Linq;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Random = UnityEngine.Random;
using Screen = UnityEngine.Device.Screen;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private string ipAddress;
    private ushort port;
    private string serverListenAddress;
    private bool showPanel;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
        _transport = GetComponent<UnityTransport>();

        // localhost
        ipAddress = "127.0.0.1";
        port = 7777;

        serverListenAddress = "0.0.0.0";
        
        _transport.OnTransportEvent += OnTransportEvent;

        // Wait time for connecting to the server is the heartbeat interval * max connect attempts
        // Default heartbeat interval is 1 second, default max connect attempts is 60
        _transport.MaxConnectAttempts = 10;
    }

    private void OnDestroy()
    {
        _transport.OnTransportEvent -= OnTransportEvent;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(Screen.width / 2f - 200, Screen.height / 1.5f, 400, 400));
        var style = new GUIStyle();
        style.fontSize = 60;
        style.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular");
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.background = Texture2D.grayTexture;
        style.normal.textColor = Color.white;
        style.active.background = Texture2D.whiteTexture;

        if (!_networkManager.IsClient && !_networkManager.IsServer)
        {
            
            if (GUILayout.Button("Join Game!", style))
            {
                UpdateConnectionData(ipAddress, port, serverListenAddress);
                _networkManager.StartClient();
                Debug.Log("Connecting to server...");
            }
            
            // TODO: DELETE THIS IN PRODUCTION 🙏
            if (GUILayout.Button("Server"))
            {
                UpdateConnectionData(ipAddress, port, serverListenAddress);
                _networkManager.StartServer();
            }

            if (GUILayout.Button("Show Panel")) showPanel = true;
        }

        GUILayout.EndArea();

        if (showPanel)
        {
            var panelWidth = Screen.width * 0.7f;
            var panelHeight = Screen.height * 0.8f;
            var panelX = (Screen.width - panelWidth) / 2;
            var panelY = (Screen.height - panelHeight) / 2;

            GUI.color = Color.gray;
            GUI.DrawTexture(new Rect(panelX, panelY, panelWidth, panelHeight), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var textStyle = new GUIStyle();
            textStyle.fontSize = 20;
            textStyle.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular");
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.normal.textColor = Color.cyan;

            GUILayout.BeginArea(new Rect(panelX, panelY, panelWidth, panelHeight));
            GUILayout.Label("Lobby joining system to be implemented here:", textStyle);
            GUILayout.Label("Format & do the client (maybe host) handling within this panel", textStyle);
            GUILayout.EndArea();

            if (GUI.Button(new Rect(panelX + 25, panelY + 25, 75, 75), "X", style)) showPanel = false;
        }
    }

    private void OnTransportEvent(NetworkEvent networkEvent, ulong clientId, ArraySegment<byte> payload,
        float receiveTime)
    {
        if (networkEvent == NetworkEvent.Disconnect) ConnectionFailPopup();
    }

    private void UpdateConnectionData(string ipAddress, ushort port, string serverListenAddress)
    {
        ((UnityTransport)_networkManager.NetworkConfig.NetworkTransport).SetConnectionData(ipAddress, port,
            serverListenAddress);
    }

    private void ConnectionFailPopup()
    {
        // TODO: Add popup UI depending on connection failure or forced disconnect? might need another method
    }
}