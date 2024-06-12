using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnergy : MonoBehaviour
{

    public float maxEnergy;
    public float currentEnergy;
    public float regenRate;
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
            AddEnergy(regenRate*Time.deltaTime);
        }
    }

    public bool CheckEnergy(float hasAmount) {
        return currentEnergy>=hasAmount;
    }

    public bool CanThrust()
    {
        return currentEnergy > 10;
    }

    public bool CanManeuver()
    {
        return currentEnergy > 10;
    }

    public void OnManeuver(float amt)
    {
        ConsumeEnergy(amt * maneuverDrainRate * Time.deltaTime);
    }

    public void OnThrust(float amt)
    {
        ConsumeEnergy(amt * thrustDrainRate * Time.deltaTime);
    }

    public void AddEnergy(float amt) {
        if(amt<0) {throw new System.Exception("Cannot Add negative energy");}
        AdjustEnergy(amt);
    }

    public void ConsumeEnergy(float amt)
    {
        if(amt<0) {throw new System.Exception("Cannot consume negative energy");}
        AdjustEnergy(-amt);
    }

    public void AdjustEnergy(float amount) {
        currentEnergy = Mathf.Clamp(currentEnergy+amount, 0, maxEnergy);
    }

}
