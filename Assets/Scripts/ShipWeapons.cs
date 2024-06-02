using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeapons : MonoBehaviour
{
    public Transform leftGun;
    public Transform rightGun;
    public Transform projectilePrefab;
    public ShipController shipController;
    public AudioClip projectileSound;
    public float projectileSpeed = 500;
    public float fireRate;
    private float lastFireTime = 0;
    private bool wasLastFireLeft;
    private AudioSource rightGunAudioSource;
    private AudioSource leftGunAudioSource;


    void Start()
    {
        leftGunAudioSource = leftGun.GetComponent<AudioSource>();
        rightGunAudioSource = rightGun.GetComponent<AudioSource>();
    }

    public void TryFireWeapon()
    {
        if (Time.time - lastFireTime > fireRate)
        {
            FireWeapon();
        }
    }

    void FireWeapon()
    {
        Transform weapon = wasLastFireLeft ? rightGun : leftGun;
        AudioSource audioSource = wasLastFireLeft ? rightGunAudioSource : leftGunAudioSource;
        audioSource.Stop();
        Transform newProjectile = Instantiate(projectilePrefab, weapon.position, weapon.rotation);
        newProjectile.GetComponent<DamageOnImpact>().ownerTag = transform.root.tag;
        newProjectile.GetComponent<Rigidbody>().velocity = shipController.shipRb.velocity + transform.forward * projectileSpeed;
        audioSource.PlayOneShot(projectileSound);
        wasLastFireLeft = !wasLastFireLeft;
        lastFireTime = Time.time;
        shipController.shipEnergy.OnWeaponFired();
    }

}
