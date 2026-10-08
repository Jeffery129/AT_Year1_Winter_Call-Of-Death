using UnityEngine;

public class WeaponHolderAnimation : MonoBehaviour
{
    public PlayerMovement player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("isMoving", PlayerIsMoving());
        animator.SetBool("isRunning", PlayerIsRunning());
    }

    bool PlayerIsMoving()
    {
        return player != null && PlayerIsGrounded() && PlayerIsMovingRaw();
    }

    bool PlayerIsMovingRaw()
    {
        return player != null && player.GetVelocityMagnitude() > 0.1f;
    }

    bool PlayerIsRunning()
    {
        return player != null && player.IsRunning();
    }

    bool PlayerIsGrounded()
    {
        return player != null && player.IsGrounded();
    }
}
