using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AmmoType {
    LASER=0,
    MISSILE=1,
}

public enum DamageType
{
    ENERGY=0,
    KINETIC=1
}

[Serializable]
public class AmmoDataEntry {
    public AmmoType ammoType;
    public string name;
    DamageType damageType;
    public float speed;
    public float fireRate; // ? Should be on Weapon? 
    public float energyCost;
    public float returnToPoolAfterTime; //time before selfdestruct
    public bool hasPropulsion;
    public Transform prefab;
    public AudioClip fireSound;
}


//TODO: You can define the ammo reset methods in here by type, which can be called by the projectile before returning to pool.
[CreateAssetMenu(menuName ="ScriptObjs/AmmoRef")]
public class AmmoRef : ScriptableObject
{
    // string[] ammoTypes = Enum.GetNames(typeof(AmmoType)); //TODO: Generate AmmoData list from AmmoTypes defined in Enum

    [SerializeField] List<AmmoDataEntry> ammoData = new List<AmmoDataEntry>();
    private Dictionary<AmmoType, AmmoDataEntry> ammoDataMap;

    void OnValidate() { //?Maybe better to place in Enable?
        BuildAmmoDataMap(true);
    }

    void BuildAmmoDataMap(bool resetMap) {
        if(resetMap) {ammoDataMap=null;}
        if(ammoDataMap==null) {ammoDataMap = new Dictionary<AmmoType, AmmoDataEntry>();}
        foreach(AmmoDataEntry data in ammoData) {
            ammoDataMap.Add(data.ammoType, data);
        }
    }

    public AmmoDataEntry GetAmmoData(AmmoType ammoType) {
        if(ammoDataMap==null||!ammoDataMap.ContainsKey(ammoType)) {BuildAmmoDataMap(false);}
        try {
            return ammoDataMap[ammoType];
        } catch {
            throw new Exception("AmmoType "+ ammoType.ToString()+" not found.");
        }
    }

}
