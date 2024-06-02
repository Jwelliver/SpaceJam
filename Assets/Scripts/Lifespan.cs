using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Lifespan destroys object after given time
*/
public class Lifespan : MonoBehaviour
{

    public float timeUntilDestroy;

    private float startTime;


    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime >= timeUntilDestroy)
        {
            Destroy(gameObject);
        }
    }

}
