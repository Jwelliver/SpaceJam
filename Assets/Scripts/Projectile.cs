using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum ProjectileType
{
    ENERGY,
    KINETIC
}

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileType projectileType;
    [SerializeField] float speed;
    public Action<Transform> OnDisableAction;
    Rigidbody rb;
    private Vector3 inheritedVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity = inheritedVelocity + (transform.forward * speed);
    }

    public void OnFire(Vector3 shipVelocity, Transform weapon)
    {
        inheritedVelocity = shipVelocity;
        transform.rotation = weapon.rotation;
        transform.position = weapon.position;
        // rb.velocity = shipVelocity + weapon.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.name + " Collided with " + collision.transform.name);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        OnDisableAction?.Invoke(transform);
    }

}
