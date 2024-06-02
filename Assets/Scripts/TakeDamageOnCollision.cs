using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class TakeDamageOnCollision : MonoBehaviour
{
    public float damageMultiplier = 1;
    Damageable damageable;
    void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    void OnCollisionEnter(Collision collision)
    {
        ImpactFX.CreateImpact(transform, collision);
        float damage = collision.impulse.magnitude * damageMultiplier;
        damageable.TakeDamage(collision, damage);
    }
}
