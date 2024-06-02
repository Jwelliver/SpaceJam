using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxDamage;
    public float damageTaken = 0;
    public bool moveAudioSourceToImpact;
    public Transform onDestroyAudioPrefab;
    public AudioSource audioSource;
    public AudioClip onDamageClip;
    public AudioClip onDestroyedClip;
    public void TakeDamage(float amount)
    {
        damageTaken += amount;
        if (audioSource != null && onDamageClip != null) audioSource.PlayOneShot(onDamageClip);
        if (damageTaken >= maxDamage)
        {
            if (audioSource != null && onDestroyedClip != null) audioSource.PlayOneShot(onDestroyedClip);
            Transform t = Instantiate(onDestroyAudioPrefab, transform.position, Quaternion.identity);
            t.GetComponent<AudioSource>().PlayOneShot(onDestroyedClip);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(Collision col, float amount)
    {
        if (moveAudioSourceToImpact && audioSource != null)
        {
            audioSource.transform.position = col.contacts[0].point;
        }
        TakeDamage(amount);
    }
}
