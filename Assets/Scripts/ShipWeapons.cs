using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeapons : MonoBehaviour
{
    public Transform leftGun;
    public Transform rightGun;
    // public Transform projectilePrefab;
    public PrefabPool prefabPool;
    public ShipController shipController;
    public AudioClip projectileSound;
    public float projectileSpeed = 500;
    public float fireRate;
    private float lastFireTime = 0;
    private Transform activeWeapon;
    private bool wasLastFireLeft;
    private AudioSource rightGunAudioSource;
    private AudioSource leftGunAudioSource;


    void Start()
    {
        leftGunAudioSource = leftGun.GetComponent<AudioSource>();
        rightGunAudioSource = rightGun.GetComponent<AudioSource>();
        // prefabPool.OnReturnToPool = ResetProjectile;
        prefabPool.OnInstantiate = InitProjectile;
        // prefabPool.OnGet = OnGetProjectile;
    }

    public void TryFireWeapon()
    {
        if (Time.time - lastFireTime > fireRate)
        {
            FireWeapon();
        }
    }

    void InitProjectile(Transform projectileTransform)
    { //* This is run by the prefabPool when instantiating a new projectile
        projectileTransform.GetComponent<Projectile>().OnDisableAction = prefabPool.ReturnToPool;
    }
    // void ResetProjectile(Transform projectileTransform)
    // {
    //     projectileTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //     projectileTransform.rotation = Quaternion.identity;
    // }

    void FireWeapon()
    {
        activeWeapon = wasLastFireLeft ? rightGun : leftGun;
        AudioSource audioSource = wasLastFireLeft ? rightGunAudioSource : leftGunAudioSource;
        // audioSource.Stop();
        try
        {
            prefabPool.Get().GetComponent<Projectile>().OnFire(shipController.shipRb.velocity, activeWeapon);
            audioSource.PlayOneShot(projectileSound);
            wasLastFireLeft = !wasLastFireLeft;
            lastFireTime = Time.time;
            shipController.shipEnergy.OnWeaponFired();
        }
        catch (Exception e)
        {
            Debug.Log("Could Not fire." + e);
        }
    }

}
