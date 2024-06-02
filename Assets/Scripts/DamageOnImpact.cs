using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnImpact : MonoBehaviour
{
    public bool useImpactForceBasedDamage;
    public bool destroyOnImpact;
    public float damageAmount;
    public string ownerTag;
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.root.tag != ownerTag)
        {
            if (other.transform.TryGetComponent<Damageable>(out Damageable damageable))
            {
                float damage = useImpactForceBasedDamage ? other.impulse.magnitude : damageAmount;
                // Debug.Log(damage);
                damageable.TakeDamage(other, damage);
                if (destroyOnImpact) DestroySelf();
            }
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.transform.root != owner)
    //     {
    //         Damageable damageable;
    //         if (other.TryGetComponent<Damageable>(out damageable))
    //         {
    //             damageable.TakeDamage(damageAmount);
    //             DestroySelf();
    //         }
    //     }
    // }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
