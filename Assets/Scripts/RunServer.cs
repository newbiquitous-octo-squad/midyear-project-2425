using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class RunServer : MonoBehaviour
{
    private NetworkManager _networkManager;
    void Start()
    {
        _networkManager = GetComponentInParent<NetworkManager>();
        
        if (Application.isEditor) return;

        _networkManager.StartServer();
    }
    
}
