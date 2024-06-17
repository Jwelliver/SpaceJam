using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CockpitUI : MonoBehaviour
{

    public ShipInterface shipInterface;
    [SerializeField] ProgressBar3D energyProgressBar;
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

    Material[] _energyProgressBarSegmentMaterials;
    Material[] energyProgressBarSegmentMaterials {
        get {
        if(_energyProgressBarSegmentMaterials==null) {
            List<Material> mats = new List<Material>();
            foreach(Transform _t in energyProgressBar.GetSegments()) {
                mats.Add(_t.GetComponent<MeshRenderer>().material);
            }
            _energyProgressBarSegmentMaterials = mats.ToArray();
        }
        return _energyProgressBarSegmentMaterials;
        }
    }

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
        //TODOS:
        // - Instead of lerping, check for pct ranges and set the color directly. (e.g. green 50-100%, yellow 25-49%, red 0-49%)
        // - Set alpha of each segment based on the pct range it represents. This will fade out each bar instead of them just popping in and out.
        float energyPct = shipEnergy.currentEnergy / shipEnergy.maxEnergy;
        energyProgressBar.SetProgress(energyPct);
        Color color =  Color.Lerp(goodColor, badColor, 1 - energyPct);
        for(int i=0; i<energyProgressBar.GetNActiveSegments(); i++) {
            energyProgressBarSegmentMaterials[i].color = color;
        }
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
