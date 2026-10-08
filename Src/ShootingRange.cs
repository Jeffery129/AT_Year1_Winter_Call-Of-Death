using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootingRange : MonoBehaviour
{
    public static ShootingRange Instance { get; set; }

    // =====================
    // Ammo Boxes
    // =====================

    [Header("Ammo Parent Object")]
    public Transform boxsParent;

    [Header("Pistol Box")]
    public GameObject pistolBoxPrefab;
    public Vector3 PistolBox;

    [Header("Rifle Box")]
    public GameObject rifleBoxPrefab;
    public Vector3 RifleBox;

    [Header("ShotGun Box")]
    public GameObject shotGunBoxPrefab;
    public Vector3 ShotGunBox;

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
    }


    public void RespawnAmmoBox(AmmoBox ammoBox, float time)
    {
        StartCoroutine(InstantiateAmmoBoxAfterDestroyed(ammoBox, time));
    }

    internal IEnumerator InstantiateAmmoBoxAfterDestroyed(AmmoBox ammoBox, float time)
    {
        yield return new WaitForSeconds(time);

        GameObject prefab = null;
        Vector3 localPos = Vector3.zero;

        switch (ammoBox.ammoType)
        {
            case AmmoBox.Ammotype.PistolAmmo:
                prefab = pistolBoxPrefab;
                localPos = PistolBox;
                break;

            case AmmoBox.Ammotype.RifleAmmo:
                prefab = rifleBoxPrefab;
                localPos = RifleBox;
                break;

            case AmmoBox.Ammotype.ShotGunAmmo:
                prefab = shotGunBoxPrefab;
                localPos = ShotGunBox;
                break;
        }

        if (prefab == null) yield break;

        GameObject box = Instantiate(prefab, boxsParent);
        box.transform.localPosition = localPos;
        box.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
