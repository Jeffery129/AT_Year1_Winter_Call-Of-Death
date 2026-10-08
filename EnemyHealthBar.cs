using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public float hideDelay = 2f;

    float hideTimer;
    Transform lookTarget;

    Enemy enemy;
    private bool isDead;

    void Awake()
    {
        gameObject.SetActive(false);

        if (Camera.main != null)
            lookTarget = Camera.main.transform;
    }

    private void Start()
    {
        enemy = gameObject.GetComponentInParent<Enemy>();
    }

    void Update()
    {
        if (enemy != null)
        {
            isDead = enemy.isDead;
        }

        if (lookTarget != null)
        {
            Vector3 dir = transform.position - lookTarget.position;

            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (!gameObject.activeSelf) return;

        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDamaged()
    {
        if (!isDead)
        {
            gameObject.SetActive(true); 
        }
        hideTimer = hideDelay;
    }
}

