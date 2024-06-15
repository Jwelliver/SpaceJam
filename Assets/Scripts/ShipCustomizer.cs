using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



//TODO: Refactor: Setup CustomizableComponent component, place this on each component that can be customized; ShipCustomizer will accumulate a list of these on start, then call them when settings are changed; Or they can subscribe to changes via ShipInterface
public class ShipCustomizer : MonoBehaviour
{
    [SerializeField] ShipInterface shipInterface;
    [SerializeField] Transform nose;
    [SerializeField] Transform body;
    [SerializeField] Transform wing;
    ShipCustomizationSettings shipCustomizationSettings;
    Weapon[] weapons;

    Material laserMaterial;


    void Awake() {
        laserMaterial = new Material(ShipCustomizationStore.Instance.defaultLaserMaterial);
    }

    void Start() {
        weapons = shipInterface.GetAllWeapons();
        foreach(Weapon w in weapons) {
            w.ammoPool.OnInstantiate+=CustomizeProjectile;
        }
        shipCustomizationSettings = shipInterface.GetAssignedPlayer()==null ? ShipCustomizationStore.GetDefaultShipCustomizationSettings() : shipInterface.GetAssignedPlayer().playerData.shipCustomizationSettings; //init with defaults here until player's customization settings are assigned
    }


    void OnDestroy() {
        foreach(Weapon w in weapons) {
            w.ammoPool.OnInstantiate-=CustomizeProjectile;
        }
    }

    void ApplyMaterial(Transform _t, Material mat) {
        _t.GetComponentInChildren<MeshRenderer>().material = mat;
    }

    public void ApplyShipCustomizationSettings(ShipCustomizationSettings _shipCustomizationSettings) {
        Debug.Log("ShipCustomizer > \n"+_shipCustomizationSettings.GetAsString());
        shipCustomizationSettings = _shipCustomizationSettings; //update local ref
        Material noseMaterial = ShipCustomizationStore.Instance.shipMaterials[shipCustomizationSettings.noseMaterialIndex];
        Material bodyMaterial = ShipCustomizationStore.Instance.shipMaterials[shipCustomizationSettings.bodyMaterialIndex];
        Material wingMaterial = ShipCustomizationStore.Instance.shipMaterials[shipCustomizationSettings.wingMaterialIndex];
        Material weaponMaterial = ShipCustomizationStore.Instance.shipMaterials[shipCustomizationSettings.weaponMaterialIndex];
        ApplyMaterial(nose, noseMaterial);
        ApplyMaterial(body, bodyMaterial);
        ApplyMaterial(wing, wingMaterial);
        laserMaterial=ShipCustomizationStore.UpdateLaserMaterial(laserMaterial, shipCustomizationSettings);
        if(weapons==null) {weapons = shipInterface.GetAllWeapons();}
        foreach(Weapon w in weapons) {
            ApplyMaterial(w.transform,weaponMaterial);
            w.ammoPool.Apply(CustomizeProjectile);
        }
    }

    public void CustomizeProjectile(Transform projectileTransform) {
        Projectile projectile = projectileTransform.GetComponent<Projectile>();
        CustomizeProjectile(shipCustomizationSettings, projectile );
    }

    public void CustomizeProjectile(ShipCustomizationSettings shipCustomizationSettings, Projectile projectile) { //called by weapons as part of init method in their ammo pools; Applies customization based on the projectile type and the projectile's player data.
        switch(projectile.ammoType) {
            case AmmoType.LASER: {
                Color laserColor = ShipCustomizationStore.Instance.laserColors[shipCustomizationSettings.laserColorIndex];
                //Set Material
                projectile.transform.GetComponentInChildren<MeshRenderer>().material = laserMaterial;
                //Set Light
                projectile.transform.GetComponent<Light>().color = laserColor;
                //Set Trail
                TrailRenderer trailRenderer = projectile.GetComponentInChildren<TrailRenderer>();
                trailRenderer.material=laserMaterial;
                trailRenderer.startColor = laserColor;
                trailRenderer.endColor = laserColor;
                break;
            }
        }
    }
}
