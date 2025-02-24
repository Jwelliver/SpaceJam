using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    PRIMARY,
    SECONDARY
}

public class Weapon : MonoBehaviour
{
    public ShipInterface shipInterface; //TODO: Set privately
    public WeaponType weaponType;
    public AmmoType ammoType;
    public Vector3 ammoPlacementOffset;
    private AmmoDataEntry ammoData;
    private AudioSource audioSource;
    public PrefabPool ammoPool; //TODO: Setup to be AmmoPool or ProjectilePool - anything so we can avoid calling getComponent on fire.

    ShipHaptics _shipHaptics;
    ShipHaptics shipHaptics {
        get { if(_shipHaptics ==null) { _shipHaptics=shipInterface.shipHaptics; } return _shipHaptics; }
        set { _shipHaptics=value; }
    }
    private float _lastFireTime;

    void Awake() {
        shipInterface = GetComponentInParent<ShipInterface>();
        ammoPool = GetComponentInChildren<PrefabPool>(); // TODO: Set this up so ammoTypes can be changed and we use a different pool.
        audioSource = GetComponent<AudioSource>();
        //TODO: Setup methods for prefab pool before initing; (unless you build projectilepool)
        ammoPool.OnInstantiate+=InitProjectile;
        ammoPool.OnGet+=OnGetProjectile;
        // OnAmmoTypeChanged();
    }

    void Start() {
        OnAmmoTypeChanged();
    }

    void OnDestroy() {
        ammoPool.OnInstantiate -=InitProjectile;
        ammoPool.OnGet -= OnGetProjectile;
    }

    void InitProjectile(Transform _t) { //TODO: Setup in projectile pool
        Projectile p = _t.GetComponent<Projectile>();
        p.sourceWeapon = this;
        // ShipCustomizationStore.CustomizeProjectile(p);
    }

    void OnGetProjectile(Transform _t) {
        _t.SetPositionAndRotation(transform.TransformPoint(ammoPlacementOffset), transform.rotation);
    }

    void OnAmmoTypeChanged() {
        ammoData = ProjectRefs.ammoRef.GetAmmoData(ammoType);
        audioSource.clip = ammoData.fireSound;
        ammoPool.SetPrefab(ammoData.prefab);
    }

    public float GetLastFireTime() {
        return _lastFireTime;
    }

    public void Fire() {
        float curTime = Time.time;
        if(curTime - _lastFireTime <ammoData.fireRate || !shipInterface.shipEnergy.CheckEnergy(ammoData.energyCost)) {return;}
        ammoPool.Get().GetComponent<Projectile>().OnFire(); //!Remove need to getComponent; See notes about ComponentPools
        audioSource.Play();
        shipHaptics.Play_FireWeapon(ammoType);
        shipInterface.shipEnergy.ConsumeEnergy(ammoData.energyCost);
        _lastFireTime = curTime;
    }



}
