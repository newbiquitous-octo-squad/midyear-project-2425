using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

public class JoinGame : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private string ipAddress;
    private ushort port;

    private string serverListenAddress;

    // private bool showPanel;
    private bool showDefaultGui = true;
    private bool showJoinGameOptions;
    private bool showServerHostScreen;
    private bool showServerOptions;
    private bool showWebServerIncompatiblePopup;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
        _transport = GetComponent<UnityTransport>();

        // localhost
        ipAddress = "127.0.0.1";
        port = 7777;

        serverListenAddress = "0.0.0.0";

        _transport.OnTransportEvent += OnTransportEvent;
        // _networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;      

        // Wait time for connecting to the server is the heartbeat interval * max connect attempts
        // Default heartbeat interval is 1 second, default max connect attempts is 60
        _transport.MaxConnectAttempts = 10;
        showDefaultGui = true;
    }

    private void OnDestroy()
    {
        _transport.OnTransportEvent -= OnTransportEvent;
        // _networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    private void OnGUI()
    {
        if (showDefaultGui) ShowDefaultGui();
        // if (showPanel) ShowPanel();
        if (showJoinGameOptions) ShowJoinGameOptions();
        if (showServerOptions) ShowServerOptions();
        if (showServerHostScreen) ShowServerHostScreen();
        if (showWebServerIncompatiblePopup) ShowWebServerIncompatiblePopup();
    }

    private void ShowDefaultGui()
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
                showDefaultGui = false;
                showJoinGameOptions = true;
            }

            if (GUILayout.Button("Server"))
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                    showServerOptions = true;
                else
                    showWebServerIncompatiblePopup = true;
            }
        }

        GUILayout.EndArea();
    }

    private void ShowJoinGameOptions()
    {
        var validPort = true;

        var areaStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(20, 20, 20, 20),
            normal = { background = Texture2D.whiteTexture }
        };

        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

        var textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white, background = Texture2D.grayTexture },
            active = { textColor = Color.black, background = Texture2D.whiteTexture },
            padding = new RectOffset(10, 10, 10, 10)
        };

        GUILayout.BeginArea(new Rect(Screen.width / 2f - 225, Screen.height / 2f - 125, 450, 250), areaStyle);

        GUILayout.Label("Enter IP Address:", labelStyle);
        ipAddress = GUILayout.TextField(ipAddress, textFieldStyle);

        GUILayout.Label("Enter Port Number:", labelStyle);
        var portString = GUILayout.TextField(port.ToString(), 5, textFieldStyle);
        try
        {
            port = ushort.Parse(portString);
        }
        catch (Exception)
        {
            validPort = false;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK", buttonStyle) && validPort)
        {
            showJoinGameOptions = false;
            UpdateConnectionData(ipAddress, port, serverListenAddress);
            _networkManager.StartClient();
            Debug.Log("Connecting to server...");
        }

        if (GUILayout.Button("Cancel", buttonStyle))
        {
            showJoinGameOptions = false;
            showDefaultGui = true;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void ShowServerOptions()
    {
        showDefaultGui = false;
        var validPort = true;

        var areaStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(20, 20, 20, 20),
            normal = { background = Texture2D.whiteTexture }
        };

        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

        var textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white, background = Texture2D.grayTexture },
            active = { textColor = Color.black, background = Texture2D.whiteTexture },
            padding = new RectOffset(10, 10, 10, 10)
        };

        GUILayout.BeginArea(new Rect(Screen.width / 2f - 150, Screen.height / 2f - 75, 300, 150), areaStyle);

        GUILayout.Label("Enter Port Number:", labelStyle);
        var portString = GUILayout.TextField(port.ToString(), 5, textFieldStyle);
        try
        {
            port = ushort.Parse(portString);
        }
        catch (Exception)
        {
            validPort = false;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK", buttonStyle) && validPort)
        {
            showServerOptions = false;
            UpdateConnectionData(ipAddress, port, serverListenAddress);
            _networkManager.StartServer();
            showServerHostScreen = true;
        }

        if (GUILayout.Button("Cancel", buttonStyle))
        {
            showServerOptions = false;
            showDefaultGui = true;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void ShowWebServerIncompatiblePopup()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2f - 200, Screen.height / 2f - 100, 400, 200));
        GUILayout.Label("Server hosting is not supported on WebGL builds.");
        if (GUILayout.Button("OK")) showWebServerIncompatiblePopup = false;
        GUILayout.EndArea();
    }

    private void ShowServerHostScreen()
    {
        var areaStyle = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(20, 20, 20, 20),
            normal = { background = Texture2D.whiteTexture }
        };

        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white, background = Texture2D.grayTexture },
            active = { textColor = Color.black, background = Texture2D.whiteTexture },
            padding = new RectOffset(10, 10, 10, 10)
        };

        GUILayout.BeginArea(new Rect(Screen.width / 2f - 200, Screen.height / 2f - 100, 400, 200), areaStyle);

        GUILayout.Label("Server is running on port: " + port, labelStyle);

        if (GUILayout.Button("Stop Server", buttonStyle))
        {
            _networkManager.Shutdown();
            showServerHostScreen = false;
            showDefaultGui = true;
        }

        GUILayout.EndArea();
    }

    // private void ShowPanel()
    // {
    //     if (showPanel)
    //     {
    //         var panelWidth = Screen.width * 0.7f;
    //         var panelHeight = Screen.height * 0.8f;
    //         var panelX = (Screen.width - panelWidth) / 2;
    //         var panelY = (Screen.height - panelHeight) / 2;

    //         GUI.color = Color.gray;
    //         GUI.DrawTexture(new Rect(panelX, panelY, panelWidth, panelHeight), Texture2D.whiteTexture);
    //         GUI.color = Color.white;

    //         var textStyle = new GUIStyle();
    //         textStyle.fontSize = 20;
    //         textStyle.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular");
    //         textStyle.alignment = TextAnchor.MiddleCenter;
    //         textStyle.normal.textColor = Color.cyan;

    //         GUILayout.BeginArea(new Rect(panelX, panelY, panelWidth, panelHeight));
    //         GUILayout.Label("Lobby joining system to be implemented here:", textStyle);
    //         GUILayout.Label("Format & do the client (maybe host) handling within this panel", textStyle);
    //         GUILayout.EndArea();

    //         if (GUI.Button(new Rect(panelX + 25, panelY + 25, 75, 75), "X", textStyle)) showPanel = false;
    //     }
    // }

    private void OnTransportEvent(NetworkEvent networkEvent, ulong clientId, ArraySegment<byte> payload,
        float receiveTime)
    {
        if (networkEvent == NetworkEvent.Disconnect && _networkManager.IsClient) ConnectionFailPopup();
    }

    // private void OnClientDisconnectCallback(ulong clientId)
    // {
    //     Debug.Log("disconnect callback i guess");
    //     ConnectionFailPopup();
    // }

    private void UpdateConnectionData(string ipAddress, ushort port, string serverListenAddress)
    {
        ((UnityTransport)_networkManager.NetworkConfig.NetworkTransport).SetConnectionData(ipAddress, port,
            serverListenAddress);
    }

    private void ConnectionFailPopup()
    {
        Debug.Log("brocken");
        _networkManager.Shutdown();
        showDefaultGui = true;
        // UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}