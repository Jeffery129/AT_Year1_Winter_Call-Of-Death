using Microlight.MicroBar;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("HitStunned")]
    public bool isStunned;
    public float hitStunTime = 0.5f;

    [Header("EnemyStatus")]
    public int enemyDamage = 25;
    [SerializeField] private int HP = 100;
    public bool isDead = false;
    public bool isAlerted = false;

    [SerializeField] MicroBar HealthBar;
    public EnemyHealthBar healthBar;

    [SerializeField] GameObject healthOrbPrefab;
    [SerializeField] GameObject coinOrbPrefab;

    [Header("Audio State")]
    public bool chaseGrowlPlayed = false;
    public float chaseGrowlResetDistance = 25f;

    [Header("Hurt Audio Cooldown")]
    public float hurtSoundCooldown = 1f;
    private float lastHurtSoundTime = -999f;

    [HideInInspector] public AudioSource audioSource;

    private Animator animator;

    private NavMeshAgent navAgent;
    private GameObject player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource = GetComponent<AudioSource>();

        HealthBar.Initialize(HP);

        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    private void Update()
    {
        if (chaseGrowlPlayed && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist > chaseGrowlResetDistance)
            {
                chaseGrowlPlayed = false;
            }
        }
    }

    public void TakeDamage(int damageAmout)
    {
        HP -= damageAmout;

        TryPlayHurtSound();

        if (healthBar != null)
            healthBar.OnDamaged();

        HealthBar.UpdateBar(HP);

        if (isDead) return;

        if (HP <= 0 && !isDead)
        {
            Die();
            return;
        }

        if (isStunned) return;

        StartCoroutine(HitStunCoroutine());
    }

    IEnumerator HitStunCoroutine()
    {
        isStunned = true;

        animator.SetTrigger("DAMAGE"); 

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        yield return new WaitForSeconds(hitStunTime);

        agent.isStopped = false;
        isStunned = false;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= 2.2)
        {
            animator.SetTrigger("HitToAttack");

            int randomValue = UnityEngine.Random.Range(0, 2);
            if (randomValue == 0)
            {
                animator.SetTrigger("ATTACK1");
            }
            else
            {
                animator.SetTrigger("ATTACK2");
            }
        }
        else if (distance > 2.2 && distance <= 18)
        {
            animator.SetTrigger("HitToChase");
        }
        else if (distance > 18)
        {
            animator.SetTrigger("ALERT");
            isAlerted = true;
        }
    }

    private void Die()
    {
        isDead = true;
        navAgent.isStopped = true;

        audioSource.PlayOneShot(SoundManager.Instance.zombieDeath);

        if (MainCanvasUI.Instance != null)
        {
            MainCanvasUI.Instance.AddKill();
        }

        int randomValue = UnityEngine.Random.Range(0, 2);
        if (randomValue == 0)
            animator.SetTrigger("DIE1");
        else
            animator.SetTrigger("DIE2");

        SpawnLoot();

        StartCoroutine(DestroyAfterDelay(4f));
    }

    private void SpawnLoot()
    {
        if (healthOrbPrefab != null)
        {
            GameObject healthOrb = Instantiate(healthOrbPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            healthOrb.GetComponent<CollectibleOrb>().orbType = CollectibleOrb.OrbType.Health;
        }

        if (coinOrbPrefab != null)
        {
            GameObject coinOrb = Instantiate(coinOrbPrefab, transform.position + Vector3.up * 0.5f + Vector3.right * 0.5f, Quaternion.identity);
            coinOrb.GetComponent<CollectibleOrb>().orbType = CollectibleOrb.OrbType.Coin;
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.mediumVioletRed;
        Gizmos.DrawWireSphere(transform.position, 1.5f); //Stop Attack

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 15f); //Detection when idle

        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(transform.position, 18f); //Detection when patroling

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 18f); //Stop Chasing

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 25f); //Alerted Chasing distance
    }

    public void AttackHit()
    {
        if (isDead) return;

        Vector3 hitCenter = transform.position + transform.forward * 1.3f;
        float hitRadius = 0.8f;

        Collider[] hits = Physics.OverlapSphere(hitCenter, hitRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerStatus>().TakeDamage(enemyDamage);
            }
        }
    }

    public void TryPlayChaseGrowl(Transform player)
    {
        if (chaseGrowlPlayed) return;

        audioSource.PlayOneShot(
            SoundManager.Instance.zombieChase
        );

        chaseGrowlPlayed = true;
    }

    void TryPlayHurtSound()
    {
        if (Time.time - lastHurtSoundTime < hurtSoundCooldown)
            return;

        audioSource.PlayOneShot(
            SoundManager.Instance.zombieHurt
        );

        lastHurtSoundTime = Time.time;
    }
}
