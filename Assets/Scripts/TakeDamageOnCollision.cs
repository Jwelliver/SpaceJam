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
        // Vector3 collisionForce = collision.impulse /Time.fixedDeltaTime;
        // float collisionStrength = collisionForce.magnitude;
        float overrideAmt = CollisionDamageOverride.CheckDamageOverrideByTag(collision.transform.tag);
        ImpactFX.CreateImpact(transform, collision);
        float damage = overrideAmt > -1 ? overrideAmt : collision.impulse.magnitude * damageMultiplier;
        damageable.TakeDamage(damage);
    }
}
