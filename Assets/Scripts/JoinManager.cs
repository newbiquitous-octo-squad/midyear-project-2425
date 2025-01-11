using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Screen = UnityEngine.Device.Screen;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;
    private ushort port;
    private string ipAddress;
    private string serverListenAddress;
    private bool showPanel;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();

        // Default port is 7777
        port = 7777;
        // Default IP address is 127.0.0.1
        ipAddress = "127.0.0.1";
        // Allow connections from everyone
        serverListenAddress = "0.0.0.0";
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
            }
            // TODO: DELETE THIS IN PRODUCTION 🙏
            if (GUILayout.Button("Server"))
            {
                UpdateConnectionData(ipAddress, port, serverListenAddress);
                _networkManager.StartServer();
            }
            if (GUILayout.Button("Show Panel"))
            {
                showPanel = true;
            }
        }
        GUILayout.EndArea();

        if (showPanel)
        {
            float panelWidth = Screen.width * 0.7f;
            float panelHeight = Screen.height * 0.8f;
            float panelX = (Screen.width - panelWidth) / 2;
            float panelY = (Screen.height - panelHeight) / 2;

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

            if (GUI.Button(new Rect(panelX + 25, panelY + 25, 75, 75), "X", style))
            {
                showPanel = false;
            }
        }
    }

    private void UpdateConnectionData(string ipAddress, ushort port, string serverListenAddress)
    {
        ((UnityTransport)_networkManager.NetworkConfig.NetworkTransport).SetConnectionData(ipAddress, port, serverListenAddress);
    }
}