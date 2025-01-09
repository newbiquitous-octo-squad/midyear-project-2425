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
        style.font = Resources.Load<Font>("Fonts/casino.3d-marquee-regular");        style.alignment = TextAnchor.MiddleCenter;
        style.normal.background = Texture2D.grayTexture;
        style.normal.textColor = Color.white;
        style.active.background = Texture2D.whiteTexture;
        
        
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
