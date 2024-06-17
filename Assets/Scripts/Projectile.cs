using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] public AmmoType ammoType;
    // public Action<Transform> OnDisableAction;
    public Weapon sourceWeapon;

    // public AmmoPool ammoPool; //TODO: Set this up, and have it assigned by the pool on instatiate.

    private AmmoDataEntry _ammoData;
    private AmmoDataEntry ammoData { get {if(_ammoData==null) _ammoData=ProjectRefs.ammoRef.GetAmmoData(ammoType); return _ammoData;}}
    // private float speed;
    private Rigidbody _rb;
    protected Rigidbody rb { get {if(_rb==null)_rb=GetComponent<Rigidbody>(); return _rb;}}
    private Vector3 inheritedVelocity;
    protected float timeFired;

    protected virtual void FixedUpdate()
    {
        if(Time.time-timeFired >= ammoData.returnToPoolAfterTime) {OnTimeout();}
        if(ammoData.hasPropulsion) rb.velocity = inheritedVelocity + (transform.forward * ammoData.speed);
    }


    public void OnFire()
    {
        timeFired = Time.time;
        inheritedVelocity = sourceWeapon.shipInterface.rb.velocity;
        rb.velocity = inheritedVelocity + (transform.forward * ammoData.speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.name + " Collided with " + collision.transform.name);
        OnImpact(collision);
    }

    protected virtual void PlayImpactFX() {

    }

    protected virtual void OnImpact(Collision col) {
        PlayImpactFX();
        OnLifeOver();
    }

    protected virtual void OnTimeout() {
        OnLifeOver();
    }

    protected virtual void OnLifeOver() {
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
