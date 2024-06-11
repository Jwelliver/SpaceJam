using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyHandler : MonoBehaviour
{
    public Action<Transform> onDestroy;


    void OnDestroy(){
        onDestroy?.Invoke(transform);
        onDestroy = null;
    }
}
