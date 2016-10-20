using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour 
{
    [SyncVar]
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar] // 모든 클라이언트에게 정보 전달
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    /* 나중에 다시 들어서 왜 이렇게 분리 하는 지 정확히 알아두자 */
    public void SetUpPlayer()
    {       
        if(isLocalPlayer)
        {
            //Switch cameras
            GameManager.sigleton.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().PlayerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(999);
    //    }
    //}

    [ClientRpc] // 이벤트를 모든 클라이언트에게 전달
    public void RpcTakeDamage(int p_amount)
    {
        if (IsDead)
            return;

        currentHealth -= p_amount;

        if(currentHealth <= 0)
        {
            Die();
        }

        Debug.Log(transform.name + " now has " + currentHealth + " health ");
    }

    public void SetDefaults()
    {
        IsDead = false;

        currentHealth = maxHealth;

        //Enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable the gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // 콜라이더는 껏다 켰다 할 수 있지만 실제로는 Behavior를 상속 받은 게 아닌
        // Component에서 상속 받았기 때문에 Behavior가 아니므로 따로 처리를 해준다.
        //Enable the colliders
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
        
        //Create spawn effect
        GameObject gfxInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(gfxInstance, 3f);
    }

    private void Die()
    {
        IsDead = true;

        // DISABLE COMPONENTS
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //Diable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        //Spawn a death effect
        GameObject gfxInstance =  (GameObject)Instantiate(deathEffect, this.transform.position, Quaternion.identity);
        Destroy(gfxInstance, 3f);

        //Switch cameras
        if(isLocalPlayer)
        {
            GameManager.sigleton.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().PlayerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD!");

        //CALL RESPAWN METHOD
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.sigleton.matchSettings.reaspawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetUpPlayer();

        Debug.Log(transform.name + " respawned: " + transform.position);
    }
}
