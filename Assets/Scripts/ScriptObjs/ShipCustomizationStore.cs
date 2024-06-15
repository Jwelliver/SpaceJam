using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptObjs/ShipCustomizationStore")]
public class ShipCustomizationStore : ScriptableObject
{

    public List<Material> shipMaterials = new List<Material>();
    public List<Color> laserColors;

    public Material defaultBodyMaterial;
    public Material defaultWingMaterial;
    public Material defaultWeaponMaterial;
    public Material defaultLaserMaterial;
    public Color defaultLaserColor;

    // private static ShipCustomizationStore _Instance;
    public static ShipCustomizationStore Instance;


    void OnEnable() {
        if(Instance!=null) {
            Destroy(this);
        } else {
            Instance=this;
        }

        if(laserColors==null) {GenerateLaserColors();}
        defaultLaserColor = laserColors[0];
    }

    void OnDisable() {
        if(Instance==this) {
           Instance=null; 
        }
    }

    void GenerateLaserColors(int steps = 20) {
        List<Color> newLaserColors = new List<Color>();
        for (int i = 0; i < steps; i++) {
            float hue = i / (float)steps;
            newLaserColors.Add(Color.HSVToRGB(hue, 1f, 1f));
        }
        laserColors = newLaserColors;
    }

    public static Material UpdateLaserMaterial(Material laserMaterial, ShipCustomizationSettings shipCustomizationSettings) {
        if(laserMaterial==null) {
            laserMaterial = new Material(Instance.defaultLaserMaterial);
        }
        Color laserColor = Instance.laserColors[shipCustomizationSettings.laserColorIndex];
         //TODO: Place code elsewhere for updating the color of a URP Lit Shader
        laserMaterial.SetColor("_BaseColor", laserColor);
        laserMaterial.SetColor("_EmissionColor", laserColor);// * Mathf.LinearToGammaSpace(2f));
        laserMaterial.SetColor("_SpecColor", laserColor);
        return laserMaterial;
    }

    public static ShipCustomizationSettings GetDefaultShipCustomizationSettings() {
        ShipCustomizationSettings defaults = new ShipCustomizationSettings();
        defaults.bodyMaterialIndex = Instance.shipMaterials.IndexOf(Instance.defaultBodyMaterial);
        defaults.wingMaterialIndex= Instance.shipMaterials.IndexOf(Instance.defaultWingMaterial);
        defaults.weaponMaterialIndex = Instance.shipMaterials.IndexOf(Instance.defaultWeaponMaterial);
        defaults.laserColorIndex = 0; //! TODO: Ensure the default laser material matches the default color
        return defaults;
    }


    //* Moved to ShipCustomizer
    // public static void CustomizeProjectile(Projectile projectile) { //called by weapons as part of init method in their ammo pools; Applies customization based on the projectile type and the projectile's player data.
    //     ShipCustomizationSettings shipCustomizationSettings = projectile.sourceWeapon.shipInterface.GetAssignedPlayer().playerData.shipCustomizationSettings;
    //     switch(projectile.ammoType) {
    //         case AmmoType.LASER: {
    //             Color laserColor = shipCustomizationSettings.laserColor;
    //             //Set Material //TODO: Place code elsewhere for updating the color of a URP Lit Shader
    //             Material material = projectile.transform.GetComponentInChildren<MeshRenderer>().material;
    //             material.SetColor("_BaseColor", laserColor);
    //             material.SetColor("_EmissionColor", laserColor);// * Mathf.LinearToGammaSpace(2f));
    //             material.SetColor("_SpecColor", laserColor);
    //             break;
    //         }
    //     }
    // }

    // public static void ApplyShipCustomization(PlayerShipCustomizationSettings playerShipCustomizationSettings, ShipInterface shipInterface) {
    //     Transform hull = shipInterface.transform.Find("Hull")
    // }

}
