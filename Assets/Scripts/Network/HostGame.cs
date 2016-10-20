using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour 
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string p_name)
    {
        roomName = p_name;
    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);﻿
        }
    }
}
