using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnergy : MonoBehaviour
{

    public float maxEnergy;
    public float currentEnergy;
    public float regenRate;
    public float weaponDrainAmount;
    public float thrustDrainRate;
    public float maneuverDrainRate;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += regenRate * Time.deltaTime;
        }
    }

    public bool CanFireWeapon()
    {
        return currentEnergy > weaponDrainAmount;
    }

    public bool CanThrust()
    {
        return currentEnergy > 10;
    }

    public bool CanManeuver()
    {
        return currentEnergy > 10;
    }

    public void OnWeaponFired()
    {
        ExpendEnergy(weaponDrainAmount);
    }

    public void OnManeuver(float amt)
    {
        ExpendEnergy(amt * maneuverDrainRate * Time.deltaTime);
    }

    public void OnThrust(float amt)
    {
        ExpendEnergy(amt * thrustDrainRate * Time.deltaTime);
    }

    void ExpendEnergy(float amt)
    {
        if (currentEnergy - amt < 0) { currentEnergy = 0; return; }
        currentEnergy -= amt;

    }

}
