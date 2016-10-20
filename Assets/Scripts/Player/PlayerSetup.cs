using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour 
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;
    [HideInInspector]
    public GameObject PlayerUIInstance 
    { 
        get { return playerUIInstance; } 
    }

    Camera sceneCamera;

    void Start()
    {
        /* 다른 오브젝트가 입력되어 컨트롤 되는 걸 방지하기 위해 자신이 아닌 다른 오브젝트를 끈다. */
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else                                                                                                 
        {
            //Disable player graphics for local player
            Utill.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
        
            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            GetComponent<PlayerManager>().SetUpPlayer();
        }
    }

    public override void OnStartClient() // 클라이언트가 접속 했을 때 부르는 함수
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(netID, player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        if(isLocalPlayer)
            GameManager.sigleton.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }
}
