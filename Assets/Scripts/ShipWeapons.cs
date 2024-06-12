using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponGroupFireMode {
    ALTERNATING
}
// TODO Setup and implement weapon group class
public class ShipWeapons : MonoBehaviour
{
    [Header("Gun Transforms")]
    public Weapon primary_L;
    public Weapon primary_R;
    public WeaponGroupFireMode primaryWeaponGroupFireMode;
    public Weapon secondary;
    public ShipController shipController;
    public float primaryFireRate;
    private float primaryLastFireTime = 0;

    public void FirePrimary() {
        //!Don't delete this commented block; Just removing until we have other FireModes implemented
        // switch(primaryWeaponGroupFireMode) { //TODO: to Avoid switch statement on every fire, assign to delegate whenever mode is switched, then call delegate from here.
        //     case WeaponGroupFireMode.ALTERNATING: {
        //         FirePrimaryAlternating(); //TODO: If you make weapons groups, pass them in to an Alternating method (agnostic to group)
        //         break;
        //     }
        // } 
        FirePrimaryAlternating(); //*Skipping firemodes for now since we don't have em
    }

    void FirePrimaryAlternating()
    {
        float curTime = Time.time;
        if (curTime - primaryLastFireTime < primaryFireRate) return;
        bool wasLastFireLeft = curTime-primary_L.GetLastFireTime() < curTime-primary_R.GetLastFireTime();
        (wasLastFireLeft ? primary_R : primary_L).Fire(); // chose primary weapon based on lastFire
        primaryLastFireTime = curTime;
    }

    public void FireSecondary() {
        secondary.Fire();
    }
}
