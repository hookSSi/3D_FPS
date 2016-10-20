using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;


public class RoomListItem : MonoBehaviour 
{
    public delegate void JoinRoomDelgate(MatchInfoSnapshot p_match);
    private JoinRoomDelgate joinRoomCallBack;

    [SerializeField]
    private Text roomNameText;

    // MatchInfoSnapshot은 방에 관한 모든 정보를 가지고 다님
    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot p_match, JoinRoomDelgate p_joinRoomCallBack)
    {
        match = p_match;
        joinRoomCallBack = p_joinRoomCallBack;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallBack.Invoke(match);
    }
}
