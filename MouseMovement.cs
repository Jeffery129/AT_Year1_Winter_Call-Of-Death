using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public bool allowLook = true;

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 currentRotation = transform.localEulerAngles;

        xRotation = currentRotation.x;
        yRotation = currentRotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowLook) return;

        //Getting the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Rotation around the x axis (Look up and down)
        xRotation -= mouseY;

        //Clamp the rotationsローテーションを制限する
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        //Rotation around the x axis (Look left and right)
        yRotation += mouseX;

        //Apply rotations to transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
