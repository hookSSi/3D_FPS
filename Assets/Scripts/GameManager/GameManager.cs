using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager sigleton;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    void Awake()
    {
        if(sigleton != null)
        {
            Debug.LogError("More than one GamaManager in scene.");
        }
        else
        {
            sigleton = this;
        }  
    }

    public void SetSceneCameraActive(bool p_isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(p_isActive);
    }
    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    public static void RegisterPlayer(string p_netID, PlayerManager p_player)
    {
        string playerID = PLAYER_ID_PREFIX + p_netID;
        players.Add(playerID, p_player);
        p_player.transform.name = playerID;
    }

    public static void UnRegisterPlayer(string p_playerID)
    {
        players.Remove(p_playerID);
    }

    public static PlayerManager GetPlayer(string p_playerID)
    {
        return players[p_playerID];
    }

    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach (string p_playerID in players.Keys)
    //    {
    //        GUILayout.Label(p_playerID + "  -  " + players[p_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}

    #endregion
}
