using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    [SerializeField] public AmmoType ammoType;
    // public Action<Transform> OnDisableAction;
    public Weapon sourceWeapon;

    // public AmmoPool ammoPool; //TODO: Set this up, and have it assigned by the pool on instatiate.
    private AmmoDataEntry ammoData;
    // private float speed;
    protected Rigidbody rb;
    private Vector3 inheritedVelocity;
    protected float timeFired;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ammoData = ProjectRefs.ammoRef.GetAmmoData(ammoType);
    }

    protected void FixedUpdate()
    {
        if(Time.time-timeFired >= ammoData.returnToPoolAfterTime) {OnTimeout();}
        if(ammoData.hasPropulsion) rb.velocity = inheritedVelocity + (transform.forward * ammoData.speed);
    }


    public void OnFire()
    {
        timeFired = Time.time;
        inheritedVelocity = sourceWeapon.shipController.shipRb.velocity;
        rb.velocity = inheritedVelocity + (transform.forward * ammoData.speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.name + " Collided with " + collision.transform.name);
        OnImpact(collision);
    }

    protected void PlayImpactFX() {

    }

    protected void OnImpact(Collision col) {
        PlayImpactFX();
        OnLifeOver();
    }

    protected void OnTimeout() {
        OnLifeOver();
    }

    protected void OnLifeOver() {
        gameObject.SetActive(false);
        ResetSelf();
        sourceWeapon.ammoPool.ReturnToPool(transform);
    }

    void ResetSelf() { // ? this can be handled by static or ammoRef; pass in this and have it perform the relevant reset.
        // gameObject.SetActive(false);
    }

    // void OnDisable() {
    //     sourceWeapon.ammoPool.ReturnToPool(transform);
    // }




}
