using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Aim check
    [SerializeField] private Weapon weaponScript;

    //Bools
    private bool isAiming;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Hipfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //ADS Recoil
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>())
        {
            weaponScript = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
            isAiming = weaponScript.isADS;
        }

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if (isAiming)
        {
            targetRotation += new Vector3(
                aimRecoilX,
                UnityEngine.Random.Range(-aimRecoilY, aimRecoilY),
                UnityEngine.Random.Range(-aimRecoilZ, aimRecoilZ)
            );
        }
        else
        {
            targetRotation += new Vector3(
                recoilX,
                UnityEngine.Random.Range(-recoilY, recoilY),
                UnityEngine.Random.Range(-recoilZ, recoilZ)
            );
        }
    }
}
