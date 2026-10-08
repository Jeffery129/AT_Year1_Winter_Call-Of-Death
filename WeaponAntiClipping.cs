using UnityEngine;

public class WeaponAntiClipping : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform activeWeaponSlot;

    [Header("Raycast Settings")]
    private float currentCheckDistance;
    public LayerMask wallLayer;

    [Header("Local Positions")]
    public Vector3 pushedBackLocalPos;
    private Vector3 normalLocalPos = Vector3.zero;

    [Header("Smooth")]
    public float smoothSpeed = 8f;

    private bool allowAntiClip = true;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (activeWeaponSlot == null) return;

        Vector3 targetPos;

        if (allowAntiClip)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool hitWall = Physics.Raycast(ray, currentCheckDistance, wallLayer);

            targetPos = hitWall ? pushedBackLocalPos : normalLocalPos;
        }
        else
        {
            targetPos = normalLocalPos;
        }

        activeWeaponSlot.localPosition = Vector3.Lerp(
            activeWeaponSlot.localPosition,
            targetPos,
            Time.deltaTime * smoothSpeed
        );
    }

    public void SetAntiClipAllowed(bool allowed)
    {
        allowAntiClip = allowed;
    }

    public void OnWeaponEquipped(Weapon weapon)
    {
        currentCheckDistance = weapon.antiClipCheckDistance;
        pushedBackLocalPos = weapon.pushedBackOffset;
    }

    public void SetActiveSlot(GameObject newSlot)
    {
        activeWeaponSlot = newSlot.transform;
    }
}
