using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Mouse Sway")]
    [SerializeField] private float mouseSmooth = 8f;
    [SerializeField] private float swayMultiplier = 1f;

    [Header("Move Tilt")]
    [SerializeField] private float moveTiltAmount = 10f;
    [SerializeField] private float moveTiltSmooth = 6f;

    float currentZTilt;

    void Update()
    {
        // ===== Mouse Input =====
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion mouseRotation = rotationX * rotationY;

        // ===== Move Input (A/D) =====
        float moveX = Input.GetAxisRaw("Horizontal"); // A / D

        float targetZTilt = -moveX * moveTiltAmount;
        currentZTilt = Mathf.Lerp(currentZTilt, targetZTilt, moveTiltSmooth * Time.deltaTime);

        Quaternion moveRotation = Quaternion.AngleAxis(currentZTilt, Vector3.forward);

        // ===== Final Rotation =====
        Quaternion targetRotation = mouseRotation * moveRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            mouseSmooth * Time.deltaTime
        );
    }
}
