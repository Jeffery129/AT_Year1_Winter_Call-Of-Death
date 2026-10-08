using System;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class HUDmanager : MonoBehaviour
{
    public static HUDmanager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;
    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    [Header("Empty")]
    public Sprite emptySlot;

    [Header("PlayerStatus")]
    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI playerTotalHP;

    [Header("CrossHair")]
    public GameObject WhenNotADS;
    public Image CrossHairWhenNotADS;

#region || Load from beginning ||

    // Ammo
    private Sprite M1911_Ammo;
    private Sprite M4_Ammo;
    private Sprite R870_Ammo;

    // Weapons
    private Sprite M1911_Weapon;
    private Sprite M4_Weapon;
    private Sprite R870_Weapon;

    // Throwables
    private Sprite Grenade;
    private Sprite Smoke;

    // CrossHair
    private Sprite crossHairWhenIdle;
    private Sprite crossHairWhenShoot;

#endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        M1911_Ammo = Resources.Load<Sprite>("M1911_Ammo");
        M4_Ammo = Resources.Load<Sprite>("M4_Ammo");
        R870_Ammo = Resources.Load<Sprite>("R870_Ammo");

        M1911_Weapon = Resources.Load<Sprite>("M1911_Weapon");
        M4_Weapon = Resources.Load<Sprite>("M4_Weapon");
        R870_Weapon = Resources.Load<Sprite>("R870_Weapon");

        Grenade = Resources.Load<Sprite>("Grenade");
        Smoke = Resources.Load<Sprite>("SmokeGrenade");

        crossHairWhenIdle = Resources.Load<Sprite>("CrossHairWhenIdle");
        crossHairWhenShoot = Resources.Load<Sprite>("CrossHairWhenShoot");
    }

    private void Update()
    {
        //Weapon Slot UI and Ammo UI
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        GameObject unActiveSlot = GetUnActiveWeaponSlot();
        Weapon unActiveWeapon = null;
        if (unActiveSlot != null)
        {
            unActiveWeapon = unActiveSlot.GetComponentInChildren<Weapon>();
        }

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            totalAmmoUI.text = $"/{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
            else
            {
                unActiveWeaponUI.sprite = emptySlot;
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            activeWeaponUI.sprite = emptySlot;
            
            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
            else
            {
                unActiveWeaponUI.sprite = emptySlot;
            }
        }

        //Lethal
        if (WeaponManager.Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = emptySlot;
        }

        //Tactical
        if (WeaponManager.Instance.tacticalCount <= 0)
        {
            tacticalUI.sprite = emptySlot;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                return M1911_Ammo;

            case Weapon.WeaponModel.M4:
                return M4_Ammo;

            case Weapon.WeaponModel.R870:
                return R870_Ammo;

            default:
                return null;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.M1911:
                return M1911_Weapon;

            case Weapon.WeaponModel.M4:
                return M4_Weapon;

            case Weapon.WeaponModel.R870:
                return R870_Weapon;

            default:
                return null;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }

    internal void UpdateThrowablesUI()
    {
        lethalAmountUI.text = $"{WeaponManager.Instance.lethalsCount}";
        tacticalAmountUI.text = $"{WeaponManager.Instance.tacticalCount}";

        switch (WeaponManager.Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Grenade;
                break;
        }

        switch (WeaponManager.Instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Smoke:
                tacticalUI.sprite = Smoke;
                break;
        }
    }

    internal void ChangeCrossHairWhenShooting(bool isShooting)
    {
        if (!isShooting)
        {
            CrossHairWhenNotADS.sprite = crossHairWhenIdle;
        }
        else
        {
            CrossHairWhenNotADS.sprite = crossHairWhenShoot;
        }
    }

    public void UpdatePlayerHP(int currentHP)
    {
        playerHP.text = $"{currentHP}";
    }

    public void UpdatePlayerTotalHP(int totalHP)
    {
        playerTotalHP.text = $"{totalHP}";
    }
}
