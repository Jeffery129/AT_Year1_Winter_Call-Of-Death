using System;
using UnityEngine;
using static HitBox;
using static UnityEngine.Rendering.DebugUI;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectHit)
    {
        //Enemy
        if (objectHit.gameObject.TryGetComponent<HitBox>(out HitBox hitbox))
        {
            int finalDamage = bulletDamage;

            if (hitbox.hitPart == HitPart.Head)
            {
                finalDamage *= 2;
            }

            hitbox.enemy.TakeDamage(finalDamage);

            CreateBloodEffect(objectHit);
            Destroy(gameObject);
            return;
        }

        //Wall and for metal Wall (Change for different material)
        if (objectHit.gameObject.CompareTag("Wall"))
        {
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
            return;
        }

        if (objectHit.gameObject.CompareTag("Sand"))
        {
            CreateSandEffect(objectHit);
            Destroy(gameObject);
            return;
        }

        if (objectHit.gameObject.CompareTag("Tree"))
        {
            CreateTreeEffect(objectHit);
            Destroy(gameObject);
            return;
        }


        //Test Session
        //if (objectHit.gameObject.CompareTag("Beer"))
        //{
        //    objectHit.gameObject.GetComponent<BeerBottle>().Shatter();
        //}
    }

    void CreateBloodEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject blood = Instantiate(
            GlobalReferences.Instance.bloodEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        blood.transform.SetParent(objectHit.gameObject.transform);

        Destroy(blood, 2f);
    }

    void CreateBulletImpactEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(objectHit.gameObject.transform);

        Destroy(hole, 3f);
    }

    void CreateSandEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject sand = Instantiate(
            GlobalReferences.Instance.sandEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        sand.transform.SetParent(objectHit.gameObject.transform);

        Destroy(sand, 3f);
    }

    void CreateTreeEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject tree = Instantiate(
            GlobalReferences.Instance.treeEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        tree.transform.SetParent(objectHit.gameObject.transform);

        Destroy(tree, 3f);
    }
}
