using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    private GameObject objectHitByRay = null;

    public Weapon hoveredWeapon = null;
    public Weapon rayHitWeapon = null;

    public AmmoBox hoveredAmmoBox = null;

    public Throwable hoveredThrowable = null;

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

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        objectHitByRay = null;
        rayHitWeapon = null;

        if (Physics.Raycast(ray, out hit, 5))
        {
            objectHitByRay = hit.transform.gameObject;

            //Weapon
            Weapon weapon = objectHitByRay.GetComponent<Weapon>();
            if (weapon != null)
            {
                rayHitWeapon = weapon;
            }
        }

        if (hoveredWeapon != null && hoveredWeapon != rayHitWeapon)
        {
            hoveredWeapon.GetComponent<Outline>().enabled = false;
            hoveredWeapon = null;
        }

        if (rayHitWeapon != null && rayHitWeapon.isActiveWeapon == false)
        {
            hoveredWeapon = rayHitWeapon;
            hoveredWeapon.GetComponent<Outline>().enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                hoveredWeapon.GetComponent<Outline>().enabled = false;
                WeaponManager.Instance.PickupWeapon(hoveredWeapon.gameObject);
                hoveredWeapon = null;
            }
        }

        //Interaction for AmmoBox
        if (objectHitByRay != null && objectHitByRay.GetComponent<AmmoBox>() != null)
        {
            hoveredAmmoBox = objectHitByRay.GetComponent<AmmoBox>();
            hoveredAmmoBox.GetComponent<Outline>().enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);

                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                if (hoveredAmmoBox.GetComponent<AmmoBox>().ammoType != AmmoBox.Ammotype.AllType)
                {
                    Destroy(objectHitByRay.gameObject);

                    if(currentScene == "ShootingRange")
                    {
                        ShootingRange.Instance.RespawnAmmoBox(hoveredAmmoBox, 3f);
                    }
                }
            }
        }
        else
        {
            if (hoveredAmmoBox != null)
            {
                hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                hoveredAmmoBox = null;
            }
        }


        //Interaction for Throwables
        if (objectHitByRay != null && objectHitByRay.GetComponent<Throwable>() != null)
        {
            hoveredThrowable = objectHitByRay.GetComponent<Throwable>();
            hoveredThrowable.GetComponent<Outline>().enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                WeaponManager.Instance.PickupThrowable(hoveredThrowable);
            }
        }
        else
        {
            if (hoveredThrowable != null)
            {
                hoveredThrowable.GetComponent<Outline>().enabled = false;
                hoveredThrowable = null;
            }
        }
    }
}
