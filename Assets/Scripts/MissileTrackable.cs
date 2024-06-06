using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //*This is only here so that missle.PerformTracking() can get the velocity; May want to track velocity manually to all non rb trackables
public class MissileTrackable : MonoBehaviour
{
    public float signatureStrength;
}
