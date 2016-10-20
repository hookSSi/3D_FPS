using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour 
{
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;
    
    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";

        if(!success || matchList == null)
        {
            status.text = "Couldn't get room list.";
            return;
        }
  
        foreach(MatchInfoSnapshot match in matchList)
        {
            GameObject roomListItemGo = Instantiate(roomListItemPrefab);
            roomListItemGo.transform.SetParent(roomListParent);

            RoomListItem roomListItem = roomListItemGo.GetComponent<RoomListItem>();

            if (roomListItem != null)
            {
                roomListItem.Setup(match, JoinRoom);
            }
            
            // as well as setting up a callback function that will join the game.

            roomList.Add(roomListItemGo);
        }

        if(roomList.Count == 0)
        {
            status.text = "No room at the moment";
        }
    }

    private void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        // 오브젝트를 Destroy한다고 해도 리스트에서 제거 되는 것이 아니기 때문에
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot p_match)
    {
        networkManager.matchMaker.JoinMatch(p_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text ="Joining...";
    }
}
