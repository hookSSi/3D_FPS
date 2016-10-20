using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour 
{
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrenGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon p_weapon)
    {
        currentWeapon = p_weapon;

        GameObject weaponInstance = (GameObject)Instantiate(p_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInstance.transform.SetParent(weaponHolder);

        currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();

        if (currentGraphics == null)
            Debug.LogError("No weaponGraphics component on the weapon object: " + weaponInstance.name);

        if (isLocalPlayer)
            Utill.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));
    }

    
}
