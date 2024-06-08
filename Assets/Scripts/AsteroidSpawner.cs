using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AsteroidType {
    ICE,
    LAVA,
    RANDOM
}

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] Transform prefab;
    [SerializeField] List<Mesh> meshes;
    [SerializeField] List<Material> materials;
    [SerializeField] float defaultMass;
    

    void Start() {
        SpawnField(new Vector3(0,0,0), new Vector3(Random.Range(20,50),Random.Range(20,50),Random.Range(20,50)), 0.001f, 20,0.5f, 2);
    }

    // public void SpawnOne(Vector3 position, float minSize, float maxSize, AsteroidType asteroidType , bool applyRandomRotation=true) {
    //     Quaternion randomRotation = Quaternion.Euler(Random.Range(0,90),Random.Range(0,90),Random.Range(0,90));
    //     Transform newAsteroid = Instantiate(prefab, position, randomRotation );
    // }

    public void SpawnRandom(Vector3 position, float minScale=0.5f, float maxScale=2, float maxAngularVelocity=0.1f) {
        Quaternion randomRotation = Quaternion.Euler(Random.Range(0,90),Random.Range(0,90),Random.Range(0,90));
        Transform newAsteroid = Instantiate(prefab, position, randomRotation, transform );
        newAsteroid.GetComponent<MeshRenderer>().material = materials[Random.Range(0,materials.Count)];
        newAsteroid.GetComponent<MeshFilter>().mesh = meshes[Random.Range(0,materials.Count)];
        float rndScale = Random.Range(minScale,maxScale);
        newAsteroid.localScale = new Vector3(rndScale,rndScale, rndScale);
        Rigidbody _rb = newAsteroid.GetComponent<Rigidbody>();
        _rb.mass = defaultMass * rndScale;
        _rb.angularVelocity = new Vector3(Random.Range(0,maxAngularVelocity),Random.Range(0,maxAngularVelocity),Random.Range(0,maxAngularVelocity));
    }

    public void SpawnField(Vector3 center, Vector3 dimensions, float density=0.001f, int maxCount=-1,float minScale=0.5f, float maxScale=2) {
        int asteroidCount = (int)(dimensions.x * dimensions.y * dimensions.z * density);
        if(maxCount>0 && asteroidCount>maxCount) {asteroidCount=maxCount;}

        for (int i = 0; i < asteroidCount; i++) {
            Vector3 randomPosition = new Vector3(
                Random.Range(center.x - dimensions.x / 2, center.x + dimensions.x / 2),
                Random.Range(center.y - dimensions.y / 2, center.y + dimensions.y / 2),
                Random.Range(center.z - dimensions.z / 2, center.z + dimensions.z / 2)
            );

            SpawnRandom(randomPosition, minScale, maxScale);
        }
    }

}
