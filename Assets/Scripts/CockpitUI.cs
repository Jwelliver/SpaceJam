using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CockpitUI : MonoBehaviour
{

    public TMPro.TextMeshProUGUI speedTextMesh;
    public TMPro.TextMeshProUGUI energyTextMesh;
    public TMPro.TextMeshProUGUI damagePctTextMesh;
    public ShipController shipController;
    public Damageable damageable;
    public ShipEnergy shipEnergy;
    public Light warningLight;
    public Color goodColor = Color.green;
    public Color badColor = Color.red;
    public AudioSource warningAudioSource;

    private bool isWarningLightActive = true;
    private float initialWarningLightIntensity;
    public float warningLightBrightnessRate = 10;


    void Start()
    {
        warningLight.enabled = false;
        initialWarningLightIntensity = warningLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        //Update speed
        speedTextMesh.text = "" + shipController.GetSpeed();

        //Update Damage
        UpdateDamageUI();

        //Update energy
        UpdateEnergyUI();

        CheckWarningLight();
        if (isWarningLightActive)
        {
            HandleWarningLight();
        }
    }

    void UpdateEnergyUI()
    {
        energyTextMesh.text = "" + Mathf.Round(shipEnergy.currentEnergy);
        energyTextMesh.color = Color.Lerp(goodColor, badColor, 1 - (shipEnergy.currentEnergy / shipEnergy.maxEnergy));
    }

    void UpdateDamageUI()
    {
        float damagePct = damageable.damageTaken / damageable.maxDamage;
        damagePctTextMesh.text = Mathf.Round(damagePct * 100) + "%";
        damagePctTextMesh.color = Color.Lerp(goodColor, badColor, damagePct);
    }


    void SetWarningLightActive(bool isActive)
    {
        warningLight.enabled = isActive;
        isWarningLightActive = isActive;
        if (isActive)
        {
            warningAudioSource.Play();
        }
        else
        {
            warningAudioSource.Stop();
        }
    }

    void CheckWarningLight()
    {
        float damagePct = damageable.damageTaken / damageable.maxDamage;
        float energyPct = shipEnergy.currentEnergy / shipEnergy.maxEnergy;
        bool warningLightShouldBeActive = (damagePct >= 0.9f) || energyPct < 0.1f;

        if (isWarningLightActive && !warningLightShouldBeActive)
        {
            SetWarningLightActive(false);
        }
        else if (!isWarningLightActive && warningLightShouldBeActive)
        {
            SetWarningLightActive(true);
        }
    }

    void HandleWarningLight()
    {
        float newIntensity = initialWarningLightIntensity * (0.5f + 0.5f * Mathf.Sin(Time.time * warningLightBrightnessRate));
        Debug.Log("Handling Warning Light: intensity: " + newIntensity);
        warningLight.intensity = newIntensity;
    }

}
