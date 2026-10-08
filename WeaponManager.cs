#if UNITY_EDITOR
using NUnit.Framework;
#endif
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public Transform worldWeaponRoot;

    public WeaponAntiClipping weaponClipper;

    public GameObject activeWeaponSlot;

    public List<GameObject> weaponSlots;

    [Header("Ammo")]
    public int MaxRifleAmmo = 180;
    public int MaxPistolAmmo = 70;
    public int MaxShotGunAmmo = 28;

    private int totalRifleAmmo = 0;
    private int totalPistolAmmo = 0;
    private int totalShotGunAmmo = 0;

    [Header("Throwables General")]
    public float throwForce = 40f;
    public GameObject throwableSpawn;
    public float throwforceMultiplier = 0;
    public float multiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int tacticalCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;


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

        totalPistolAmmo = MaxPistolAmmo;
        totalRifleAmmo = MaxRifleAmmo;
        totalShotGunAmmo = MaxShotGunAmmo;
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;

        //For Anticlip
        weaponClipper.SetActiveSlot(activeWeaponSlot);
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        //Switch Weapon Slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        //Throwables//
        //As long as player hold the G key,force will be stronger
        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            throwforceMultiplier += Time.deltaTime;

            if (throwforceMultiplier > multiplierLimit)
            {
                throwforceMultiplier = multiplierLimit;
            }
        }

        //Lethals//
        //Throw Throwable Lethal Objects
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }

            throwforceMultiplier = 0;
        }

        //Tacticals//
        //Throw Throwable Tactical Objects
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalCount > 0)
            {
                ThrowTactical();
            }

            throwforceMultiplier = 0;
        }
    }

    #region || Weapon ||

    public void PickupWeapon(GameObject PickedupWeapon)
    {
        AddWeaponIntoActiveSlot(PickedupWeapon);

        weaponClipper.OnWeaponEquipped(PickedupWeapon.GetComponent<Weapon>());
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.localPosition = new Vector3(weapon.spawnInHandPosition.x, weapon.spawnInHandPosition.y, weapon.spawnInHandPosition.z);
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnInHandRotation.x, weapon.spawnInHandRotation.y, weapon.spawnInHandRotation.z);
        //
        pickedupWeapon.transform.localScale = weapon.BaseLocalScale;

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
            Weapon weapon = weaponToDrop.GetComponent<Weapon>();

            weapon.isActiveWeapon = false;
            weapon.animator.enabled = false;

            Vector3 dropPosition = pickedupWeapon.transform.position;
            Quaternion dropRotation = pickedupWeapon.transform.rotation;
            //
            weaponToDrop.transform.SetParent(worldWeaponRoot, true);

            weaponToDrop.transform.position = dropPosition;
            weaponToDrop.transform.rotation = dropRotation;
            //
            weaponToDrop.transform.localScale = weapon.BaseLocalScale;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        weaponClipper.SetActiveSlot(activeWeaponSlot);

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;

            weaponClipper.OnWeaponEquipped(newWeapon);
        }
    }

    // =====================
    // Pause Control
    // =====================
    public void SetAllWeaponsShooting(bool allow)
    {
        foreach (GameObject slot in weaponSlots)
        {
            if (slot.transform.childCount > 0)
            {
                Weapon weapon = slot.transform.GetChild(0).GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.allowShooting = allow;
                }
            }
        }
    }

    #endregion



    #region || Ammo ||
    internal void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.Ammotype.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount_Pistol;
                totalPistolAmmo = CalculateTrueTotalAmmo(totalPistolAmmo, MaxPistolAmmo);
                break;

            case AmmoBox.Ammotype.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount_Rifle;
                totalRifleAmmo = CalculateTrueTotalAmmo(totalRifleAmmo, MaxRifleAmmo);
                break;

            case AmmoBox.Ammotype.ShotGunAmmo:
                totalShotGunAmmo += ammo.ammoAmount_ShotGun;
                totalShotGunAmmo = CalculateTrueTotalAmmo(totalShotGunAmmo, MaxShotGunAmmo);
                break;

            case AmmoBox.Ammotype.AllType:
                totalPistolAmmo += ammo.ammoAmount_Pistol;
                totalPistolAmmo = CalculateTrueTotalAmmo(totalPistolAmmo, MaxPistolAmmo);

                totalRifleAmmo += ammo.ammoAmount_Rifle;
                totalRifleAmmo = CalculateTrueTotalAmmo(totalRifleAmmo, MaxRifleAmmo);

                totalShotGunAmmo += ammo.ammoAmount_ShotGun;
                totalShotGunAmmo = CalculateTrueTotalAmmo(totalShotGunAmmo, MaxShotGunAmmo);
                break;
        }
    }

    private int CalculateTrueTotalAmmo(int totalAmmo, int maxAmmo)
    {
        if (totalAmmo > maxAmmo)
        {
            totalAmmo = maxAmmo;
        }
        return totalAmmo;
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M4:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.M1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.R870:
                totalShotGunAmmo -= bulletsToDecrease;
                break;
        }
    }

    // Check AmmoLeft by modeltype
    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M4:
                return totalRifleAmmo;

            case Weapon.WeaponModel.M1911:
                return totalPistolAmmo;

            case Weapon.WeaponModel.R870:
                return totalShotGunAmmo;

            default:
                return 0;
        }
    }

    #endregion

    #region || Throwables ||
    internal void PickupThrowable(Throwable hoveredThrowable)
    {
        switch (hoveredThrowable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke);
                break;

        }
    }

    #region || Throwable_Lethal ||

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethals)
            {
                lethalsCount++;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDmanager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals Limit reached");
            }
        }
        else
        {
            // cannot pickup different lethal
            // swap lethals
        }
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        Vector3 throwDir = Camera.main.transform.forward + Vector3.up * 0.35f;

        rb.AddForce(throwDir.normalized * (throwForce * throwforceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount--;

        if (lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDmanager.Instance.UpdateThrowablesUI();
    }

    #endregion

    #region || Throwable_Tactical ||

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalCount < maxTacticals)
            {
                tacticalCount++;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDmanager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Tacticals Limit reached");
            }
        }
        else
        {
            // cannot pickup different tactical
            // swap tacticals
        }
    }
    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        Vector3 throwDir = Camera.main.transform.forward + Vector3.up * 0.35f;

        rb.AddForce(throwDir.normalized * (throwForce * throwforceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalCount -= 1;

        if (tacticalCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDmanager.Instance.UpdateThrowablesUI();
    }

    #endregion

    private GameObject GetThrowablePrefab(Throwable.ThrowableType equippedThrowableType)
    {
        switch (equippedThrowableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;

            case Throwable.ThrowableType.Smoke:
                return smokeGrenadePrefab;
        }

        return new();
    }

    #endregion
}
