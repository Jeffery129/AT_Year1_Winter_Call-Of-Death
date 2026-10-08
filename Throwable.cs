using System;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [Header("Throwables General")]
    [SerializeField] float delay = 3f;

    [Header("Grenade")]
    public int grenadeDamage = 100;
    public LayerMask explosionAffectLayers;
    [SerializeField] float damageRadius = 50f;

    float countdown;
    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        None,
        Grenade,
        Smoke
    }
    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if(hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();

        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch(throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke:
                SmokeEffect();
                break;
        }
    }

    private void SmokeEffect()
    {
        //Visual Effect
        GameObject smokeEffect = GlobalReferences.Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);

        //Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.smokeSoundClip);

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // blind the enemies
            }

            //Apply damage to enemy
        }
    }

    private void GrenadeEffect()
    {
        //Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSoundClip);

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            damageRadius,
            explosionAffectLayers
        );

        //DamagedEnemies Recorded
        HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

        foreach (Collider objectInRange in colliders)
        {
            //Apply damage to enemy
            if (objectInRange.TryGetComponent<HitBox>(out HitBox hitBox))
            {
                Enemy enemy = hitBox.enemy;

                if (enemy != null && !damagedEnemies.Contains(enemy))
                {
                    enemy.TakeDamage(grenadeDamage);
                    damagedEnemies.Add(enemy);
                }
            }
        }
    }
}
