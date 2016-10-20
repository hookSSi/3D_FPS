using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour 
{
    public static bool IsOn = false;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom()
    {
        MatchInfo matchInfo = networkManager.matchInfo;

        // 제대로 이 부분이 작동하지 않는데 3명이상부터는 작동이 되서 유니티 자체의 버그인지 의심이다.
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }
}
