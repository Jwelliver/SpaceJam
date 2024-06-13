using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CockpitUI : MonoBehaviour
{

    public ShipInterface shipInterface;
    [SerializeField] TMPro.TextMeshProUGUI speedTextMesh;
    [SerializeField] TMPro.TextMeshProUGUI energyTextMesh;
    [SerializeField] TMPro.TextMeshProUGUI damagePctTextMesh;
    [SerializeField] Light warningLight;
    [SerializeField] Color goodColor = Color.green;
    [SerializeField] Color badColor = Color.red;
    [SerializeField] AudioSource warningAudioSource;

    private Damageable shipDamage;
    private ShipController shipController;
    private ShipEnergy shipEnergy;
    private bool isWarningLightActive = true;
    private float initialWarningLightIntensity;
    public float warningLightBrightnessRate = 10;

    void Awake() {
        shipController = shipInterface.shipController;
        shipEnergy = shipInterface.shipEnergy;
        shipDamage = shipInterface.shipDamage;
    }


    void Start()
    {
        warningLight.enabled = false;
        initialWarningLightIntensity = warningLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        //Update speed
        speedTextMesh.SetText("" + shipController.GetSpeed());

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
        energyTextMesh.SetText("" + Mathf.Round(shipEnergy.currentEnergy));
        energyTextMesh.color = Color.Lerp(goodColor, badColor, 1 - (shipEnergy.currentEnergy / shipEnergy.maxEnergy));
    }

    void UpdateDamageUI()
    {
        float damagePct = shipDamage.damageTaken / shipDamage.maxDamage;
        damagePctTextMesh.SetText(Mathf.Round(damagePct * 100) + "%");
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
        float damagePct = shipDamage.damageTaken / shipDamage.maxDamage;
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
        // Debug.Log("Handling Warning Light: intensity: " + newIntensity);
        warningLight.intensity = newIntensity;
    }

}
