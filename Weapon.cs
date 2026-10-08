using System;
using System.Collections;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Profiling.Memory.Experimental;
#endif
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Weapon : MonoBehaviour
{
    //Pickable or not
    public bool isActiveWeapon;
    public int weaponDamage;

    //ADS animation
    public bool isADS;
    bool wantsToADSAfterReload;
    bool adsInputDuringReload;

    //ADS Camera Fov
    public float hipFOV = 58.71f;
    public float adsFOV = 45f;
    public float fovLerpSpeed = 10f;
    private Camera playerCamera;

    //ShowCrossHair
    bool readyToShowCrossHair;

    //Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Spread
    public float spreadIntensity;
    //Recoil
    [SerializeField] private Recoil recoilScript;

    //Bullet
    bool emptySoundPlayed = false;//lock for emptyMagazineSound
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    //Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    //In hand position & rotation
    public Vector3 spawnInHandPosition;
    public Vector3 spawnInHandRotation;
    //Transform Base Scale
    [SerializeField] private Vector3 baseLocalScale;
    public Vector3 BaseLocalScale => baseLocalScale;

    [Header("Pause Control")]
    public bool allowShooting = true;

    public enum ShootingMode
    {
        Single,
        Auto,
    }
    public ShootingMode currentShootingMode;

    public enum WeaponModel
    {
        M1911,
        M4,
        R870
    }
    public WeaponModel thisWeaponModel;

    [Header("Pellets For R870")]
    public int pelletsPerShot = 8;

    [Header("Anti Clipping")]
    public float antiClipCheckDistance;
    public Vector3 pushedBackOffset;

    [Header("Shell Ejection")]
    public ShellEjector shellEjector;

    private void Awake()
    {
        if (baseLocalScale == Vector3.zero)
        {
            baseLocalScale = transform.localScale;
        }

        readyToShoot = true;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

    }
    private void Start()
    {
        if (recoilScript == null)
        {
            Debug.LogWarning($"{name} no reference for recoil");
        }

        playerCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!allowShooting)
        {
            isShooting = false;
            return;
        }

        if (isActiveWeapon)
        {
            if (bulletsLeft == 0 && isShooting && isReloading == false)
            {
                if (emptySoundPlayed == false)
                {
                    SoundManager.Instance.PlayEmptyMagazineSound();
                    emptySoundPlayed = true;
                }
            }
            else
            {
                emptySoundPlayed = false;
            }

            //Aim Down Shooting
            if (isReloading)
            {
                //Record the input when reloading
                if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
                {
                    adsInputDuringReload = true;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    animator.SetTrigger("enterADS");
                    isADS = true;

                    HUDmanager.Instance.WhenNotADS.SetActive(false);

                    PlayerMovement player = GetComponentInParent<PlayerMovement>();
                    if (player != null)
                    {
                        player.LockRun(true);
                    }

                    WeaponAntiClipping antiClip = GetComponentInParent<WeaponAntiClipping>();
                    if (antiClip != null)
                    {
                        antiClip.SetAntiClipAllowed(false);
                    }
                }

                if (Input.GetMouseButtonUp(1) && isADS)
                {
                    animator.ResetTrigger("RECOIL_ADS");
                    animator.SetTrigger("exitADS");
                    isADS = false;

                    readyToShowCrossHair = true;

                    PlayerMovement player = GetComponentInParent<PlayerMovement>();
                    if (player != null)
                    {
                        player.LockRun(false);
                    }
                }
            }

            // isShooting bool check
            if (currentShootingMode == ShootingMode.Auto)
            {
                // Shoot When Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single)
            {
                // Clicking Left Mouse Button Once
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            PlayerMovement movement = GetComponentInParent<PlayerMovement>();
            if (movement != null)
            {
                movement.LockRunByShooting(isShooting);
            }

            // Main Reloading
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0 && readyToShoot == true)
            {
                if (thisWeaponModel == WeaponModel.R870)
                {
                    if (bulletsLeft == 0)
                    {
                        Reload();
                    }
                }
                else
                {
                    Reload();
                }
            }

            ////////////////////////////////////////////////////////////////////////////////
            // When magazine <= 0 mouse left button click will reload (Maybe used in future)
            if (readyToShoot && isShooting && isReloading == false && bulletsLeft <= 0)
            {
                //Reload();
            }
            ////////////////////////////////////////////////////////////////////////////////

            if (readyToShoot && isShooting && bulletsLeft > 0 && isReloading == false)
            {
                FireWeapon();
            }

            HUDmanager.Instance.ChangeCrossHairWhenShooting(isShooting);

            // Handle crosshair restore after exiting ADS
            if (readyToShowCrossHair && !isADS && !isReloading)
            {
                HUDmanager.Instance.WhenNotADS.SetActive(true);
                readyToShowCrossHair = false;
            }

            // ADS FOV Control
            if (playerCamera != null)
            {
                float targetFOV = isADS ? adsFOV : hipFOV;

                playerCamera.fieldOfView = Mathf.Lerp(
                    playerCamera.fieldOfView,
                    targetFOV,
                    Time.deltaTime * fovLerpSpeed
                );
            }

            // Allow anticlip
            if (!isADS)
            {
                WeaponAntiClipping antiClip = GetComponentInParent<WeaponAntiClipping>();
                if (antiClip != null)
                {
                    antiClip.SetAntiClipAllowed(true);
                }
            }
        }
    }

    public int GetFinalDamage()
    {
        return weaponDamage + PlayerData.attackLevel * 2;
    }

    private void FireWeapon()
    {
        PlayerMovement movement = GetComponentInParent<PlayerMovement>();
        if (movement != null)
        {
            movement.LockRunByShooting(true);
        }

        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (recoilScript != null)
        {
            recoilScript.RecoilFire();
        }

        if (isADS)
        {
            animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }

        //SoundManager.Instance.shootingSound1911.Play();
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        // Dir for M4 and M1911
        Vector3 shootingDirection = CalculateFireDirection(isADS).normalized;

        //ShotGun
        if (thisWeaponModel == WeaponModel.R870)
        {
            for (int i = 0; i < pelletsPerShot; i++)
            {
                Vector3 dir = CalculateFireDirection(isADS);

                GameObject pellet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

                //Pellet Damage
                Bullet pel = pellet.GetComponent<Bullet>();
                pel.bulletDamage = GetFinalDamage();

                pellet.transform.forward = dir;

                pellet.GetComponent<Rigidbody>().AddForce(dir * bulletVelocity, ForceMode.Impulse);

                StartCoroutine(DestroyBulletAfterTime(pellet, bulletPrefabLifeTime));
            }
            //Shell Reject
            StartCoroutine(InstantiateShellAfterTime(0.52f));
        }
        else //Rifle and Pistol
        {
            //Instantiate the bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);//identity is default rotation

            //Bullet Damage
            Bullet bul = bullet.GetComponent<Bullet>();
            bul.bulletDamage = GetFinalDamage();

            bullet.transform.forward = shootingDirection;
            //Shoot the bullet
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

            //Shell Reject
            StartCoroutine(InstantiateShellAfterTime(0.1f));

            //Destroy the bullet after time
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        }

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
    }

    private void Reload()
    {
        wantsToADSAfterReload = isADS;
        adsInputDuringReload = false;

        SetADSAllowed(false);

        if (isADS)
        {
            animator.ResetTrigger("RECOIL_ADS");
            animator.SetTrigger("exitADS");
            isADS = false;
        }

        //SoundManager.Instance.reloadingSound1911.Play();
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        PlayerMovement player = GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            player.LockRun(true);
        }

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        int ammoNeeded = magazineSize - bulletsLeft;

        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) >= ammoNeeded)
        {
            bulletsLeft += ammoNeeded;
            WeaponManager.Instance.DecreaseTotalAmmo(ammoNeeded, thisWeaponModel);
        }
        else
        {
            bulletsLeft += WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel); ;
            WeaponManager.Instance.DecreaseTotalAmmo(WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel), thisWeaponModel);
        }

        isReloading = false;

        PlayerMovement player = GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            player.LockRun(false);
        }

        SetADSAllowed(true);

        animator.ResetTrigger("exitADS");

        //if player wants to ADS after reload
        if (wantsToADSAfterReload && Input.GetMouseButton(1) && adsInputDuringReload == false)
        {
            animator.SetTrigger("enterADS");
            isADS = true;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    void SetADSAllowed(bool allowed)
    {
        animator.SetBool("canADS", allowed);
    }

    private Vector3 CalculateFireDirection(bool isADS)
    {
        Camera cam = Camera.main;

        float spread = isADS ? spreadIntensity * 0.25f : spreadIntensity;

        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spread;

        //Normalize the camera aspect
        randomOffset.x /= cam.aspect;

        Vector3 viewportPoint = new Vector3(
            0.5f + randomOffset.x,
            0.5f + randomOffset.y,
            0f
        );

        Ray ray = cam.ViewportPointToRay(viewportPoint);

        Vector3 aimPoint = ray.GetPoint(100f);

        return (aimPoint - bulletSpawn.position).normalized;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private IEnumerator InstantiateShellAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        //Bullet Shell Eject
        shellEjector.Eject();
    }
}
