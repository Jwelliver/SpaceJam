using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodeOnDestroy : MonoBehaviour
{

    public int nParticles;
    public float scale;
    public Transform particle;
    public bool useMyMaterial;
    public Material particleMaterial;

    private bool isQuitting;

    void Start()
    {
        if (useMyMaterial)
        {
            particleMaterial = GetComponent<MeshRenderer>().material;
        }
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting)
        {
            Debug.Log("destroyed");
            Explode();
        }
    }

    public void Explode()
    {
        for (int i = 0; i < nParticles; i++)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(0.05f, 1), Random.Range(0.05f, 1), Random.Range(0.05f, 1));
            Transform newParticle = Instantiate(particle, position, Quaternion.identity);
            newParticle.localScale = new Vector3(scale, scale, scale);
            if (particleMaterial != null)
            {
                newParticle.GetComponent<MeshRenderer>().material = particleMaterial;
            }
            newParticle.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position, scale * 10);
        }

    }



}
