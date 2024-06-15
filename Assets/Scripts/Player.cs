using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform shipPrefab;
    public Camera cam;
    public PlayerShipInputHandler playerShipInputHandler;
    public PlayerData playerData;


    // ACTIONS
    public Action OnBeforeSpawnShip;
    public Action<ShipInterface> OnShipSpawned;
    public Action OnShipDespawned;

    // Private
    private ShipInterface shipInterface;

    void Awake() {
        playerData = new PlayerData(); //TODO: Attempt to load saved data; Or load From PlayerManager and set here.
    }

    public void DespawnShip() {
        cam.transform.parent =  transform;
        OnShipDespawned?.Invoke();
        shipInterface = null;
    }

    public void SpawnShip() {
        if(shipInterface!=null) {
            Debug.LogWarning("Attempting to spawn ship when existing shipInterface is not null.");
            return;
        }
        OnBeforeSpawnShip?.Invoke();
        //Create Ship
        Vector3 spawnLocation = GetSpawnLocation();
        Transform newShip = Instantiate(shipPrefab, spawnLocation, Quaternion.identity);
        //Get reference to shipInterface
        shipInterface = newShip.GetComponent<ShipInterface>();
        shipInterface.AssignPlayer(this);
        shipInterface.OnShipObjectDisabled += OnShipObjectDisabled;
        // shipInterface.shipCustomizer.ApplyShipCustomizationSettings(playerData.shipCustomizationSettings);
        OnShipSpawned?.Invoke(shipInterface);
    }

    Vector3 GetSpawnLocation() {//TODO: Handle SpawnLocation elsewhere
        float r = 50;
        float GetRandom() => UnityEngine.Random.Range(-r,r);
        return new Vector3(GetRandom(), GetRandom(), GetRandom());
    }

    void OnShipObjectDisabled(ShipInterface shipInterface) {
        DespawnShip();
    }

    void OnDisable() {
        if(shipInterface!=null) {
            shipInterface.OnShipObjectDisabled-=OnShipObjectDisabled;
        }
    }

}


public class PlayerData {

    public ShipCustomizationSettings shipCustomizationSettings = ShipCustomizationStore.GetDefaultShipCustomizationSettings();

}

public class ShipCustomizationSettings {

    public int bodyMaterialIndex;
    public int wingMaterialIndex;
    public int weaponMaterialIndex;
    public int noseMaterialIndex;
    public int laserColorIndex;

    public string GetAsString() {
        return "ShipCustomizationSettings:"+"\nNoseMatIndex"+noseMaterialIndex+"\nBodyMatIndex: "+bodyMaterialIndex+"\nWingMatIndex: "+wingMaterialIndex+"\nWeaponMatIndex: "+weaponMaterialIndex+"\nLaserColorIndex: "+laserColorIndex;
    }

    // public Material hullMaterial;
    // public Material wingMaterial;
    // public Material weaponMaterial;
    // public Material laserMaterial;
    // private Color _laserColor;
    // public Color laserColor {
    //     get {
    //         return _laserColor;
    //     }
    //     set {
    //         _laserColor = value;
    //         laserMaterial = ShipCustomizationStore.GetLaserMaterial(this);
    //     }
    // }

}