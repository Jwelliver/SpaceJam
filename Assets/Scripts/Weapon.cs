using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    PRIMARY,
    SECONDARY
}

public class Weapon : MonoBehaviour
{
    public ShipController shipController; //TODO: Set privately
    public WeaponType weaponType;
    public AmmoType ammoType;
    public Vector3 ammoPlacementOffset;
    private AmmoDataEntry ammoData;
    private AudioSource audioSource;
    public PrefabPool ammoPool; //TODO: Setup to be AmmoPool or ProjectilePool - anything so we can avoid calling getComponent on fire.
    private float _lastFireTime;

    void Awake() {
        ammoPool = GetComponentInChildren<PrefabPool>(); // TODO: Set this up so ammoTypes can be changed and we use a different pool.
        audioSource = GetComponent<AudioSource>();
        //TODO: Setup methods for prefab pool before initing; (unless you build projectilepool)
        ammoPool.OnInstantiate+=InitProjectile;
        ammoPool.OnGet+=OnGetProjectile;
        OnAmmoTypeChanged();
    }

    void OnDestroy() {
        ammoPool.OnInstantiate -=InitProjectile;
        ammoPool.OnGet -= OnGetProjectile;
    }

    void InitProjectile(Transform _t) { //TODO: Setup in projectile pool
        Projectile p = _t.GetComponent<Projectile>();
        p.sourceWeapon = this;
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
        if(curTime - _lastFireTime <ammoData.fireRate || !shipController.shipEnergy.CheckEnergy(ammoData.energyCost)) {return;}
        ammoPool.Get().GetComponent<Projectile>().OnFire(); //!Remove need to getComponent; See notes about ComponentPools
        audioSource.Play();
        shipController.shipEnergy.ConsumeEnergy(ammoData.energyCost);
        _lastFireTime = curTime;
    }



}
