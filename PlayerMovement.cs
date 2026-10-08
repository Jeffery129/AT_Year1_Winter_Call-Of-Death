using Microlight.MicroBar;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f * 2;
    public float jumpHeight = 0.5f;

    [Header("Stamina")]
    [SerializeField] MicroBar staminaBar;
    public float maxStamina = 100f;
    public float stamina;
    public float staminaDrainPerSecond = 20f;
    public float staminaRecoverPerSecond = 15f;

    Vector3 velocity;

    bool canRun = true;
    bool isGrounded;
    bool isMoving;
    bool isRunning;

    bool runLocked;
    bool shootLocked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        walkSpeed += PlayerData.speedLevel * 0.5f;
        runSpeed += PlayerData.speedLevel * 0.7f;

        controller = GetComponent<CharacterController>();

        stamina = maxStamina;
        staminaBar.Initialize(stamina);//MicroBar Asset
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Check
        isGrounded = controller.isGrounded;

        //Reset the default velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Get the inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Stamina
        //Running only when moving forward
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && z > 0 && canRun && !runLocked && !shootLocked;

        if (wantsToRun)
        {
            isRunning = true;
            stamina -= staminaDrainPerSecond * Time.deltaTime;

            if (stamina <= 0)
            {
                stamina = 0;
                canRun = false;
                isRunning = false;
            }
        }
        else
        {
            isRunning = false;
            stamina += staminaRecoverPerSecond * Time.deltaTime;

            if (stamina >= maxStamina * 0.2f)//Only when remaining more than 20% can run
            {
                canRun = true;
            }
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        staminaBar.UpdateBar(stamina);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * x + transform.forward * z;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * currentSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);

        isMoving = controller.velocity.magnitude > 0.1f && isGrounded;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetVelocityMagnitude()
    {
        return controller.velocity.magnitude;
    }

    public void LockRun(bool locked)
    {
        runLocked = locked;

        if (locked)
        {
            isRunning = false;
        }
    }

    public void LockRunByShooting(bool locked)
    {
        shootLocked = locked;

        if (locked)
        {
            isRunning = false;
        }
    }
}
