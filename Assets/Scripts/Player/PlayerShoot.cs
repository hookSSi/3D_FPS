using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour 
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referrenced!");
            this.enabled = false;
        }

        weaponManager = this.GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.IsOn)
        {
            CancelInvoke("Shoot");
            return;
        }

        if(currentWeapon.fireRate <= 0f)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    // Is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    // Is called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrenGraphics().muzzleFlash.Play();
    }

    // Is called on the server when we hit something
    // Take in the hit point and the normal of the surface
    [Command]
    void CmdOnHit(Vector3 p_pos, Vector3 p_normal)
    {
        RpcDoHitEffect(p_pos, p_normal);
    }

    // Is called on all clients
    // Here we can spawn in cool effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 p_pos, Vector3 p_normal)
    {
       GameObject hitEffect = (GameObject)Instantiate(weaponManager.GetCurrenGraphics().hitEffectPrefab, p_pos, Quaternion.LookRotation(p_normal));
       Destroy(hitEffect, 2f);
       // 추가 오브젝트 풀링을 이용해서 퍼포먼스를 올리자 ex) swimming pool
    }

    [Client] // 클라이언트에서만 부름
    private void Shoot()
    {
        /* 클라이언트가 쏘는 총 이펙트도 보이게 하려고 구성함 */
        /* 클라이언트이면 총이 쏘여지지 않는다. */
        /* 하지만 로컬플레이어가 쏘면 총이펙트가 서버에서 불리기 때문에 총 이펙트는 보여진다 */
       
        if(!isLocalPlayer)
        {
            return;
        }

        // We are shooting, call the OnShoot method on the server
        CmdOnShoot();
        
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) 
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);
            }
           
            // We hit something, call the OnHit method on the server
            CmdOnHit(hit.point, hit.normal);
        }
    }

    [Command] // 서버에서만 부름
    void CmdPlayerShot(string p_playerID, int p_damage)
    {
        Debug.Log(p_playerID + " has been shot");

        PlayerManager player = GameManager.GetPlayer(p_playerID);
        player.RpcTakeDamage(p_damage);
    }
}
