using System;
using UnityEngine;

public class TriggerColliderHandler : MonoBehaviour
{
    public string triggerId;
    public Collider col;
    public Action<string, Collider> OnEnter;
    public Action<string, Collider> OnExit;
    public Action<string, Collider> OnStay;

    void Awake() {
        col = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider col) {
        OnEnter?.Invoke(triggerId, col);
    }
        
    void OnTriggerExit(Collider col) {
        OnExit?.Invoke(triggerId, col);
    }

    void OnTriggerStay(Collider col) {
        OnStay?.Invoke(triggerId, col);
    }

}
