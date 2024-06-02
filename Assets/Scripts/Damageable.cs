using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxDamage;
    public float damageTaken = 0;
    public void TakeDamage(float amount)
    {
        damageTaken += amount;
        if (damageTaken >= maxDamage)
        {
            Destroy(gameObject);
        }
    }
}
