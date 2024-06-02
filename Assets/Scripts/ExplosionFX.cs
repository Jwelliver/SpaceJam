using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    [SerializeField] Transform explosionParticlePrefab;

    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Explode(Vector3 basePosition, Material particleMaterial, AudioClip explosionClip)
    {
        //TODO: Get these values from a scriptableObj data store, or somewhere; Same with audioclip
        int nParticles = Random.Range(5, 20);
        float scale = 0.2f;//Mathf.Lerp(0, 1, nParticles / 20);

        audioSource.PlayOneShot(explosionClip);
        for (int i = 0; i < nParticles; i++)
        {
            Vector3 position = basePosition + new Vector3(Random.Range(0.05f, 1), Random.Range(0.05f, 1), Random.Range(0.05f, 1));
            Transform newParticle = Instantiate(explosionParticlePrefab, position, Quaternion.identity);
            newParticle.localScale = new Vector3(scale, scale, scale);
            if (particleMaterial != null)
            {
                newParticle.GetComponent<MeshRenderer>().material = particleMaterial;
            }
            newParticle.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position, scale * 10);
        }
    }
}
