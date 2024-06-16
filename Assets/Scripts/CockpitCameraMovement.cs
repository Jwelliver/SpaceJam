using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//! Not in use
public class CockpitCameraMovement : MonoBehaviour
{
    public float accelerationEffect = 0.1f;
    public float angularEffect = 0.1f;
    public float smoothTime = 0.3f;
    public float maxOffset = 1.0f;

    ShipInterface _shipInterface;
    ShipInterface shipInterface {  get { if(_shipInterface==null ) { _shipInterface = GetComponentInParent<ShipInterface>();  }  return _shipInterface; } }

    Rigidbody _rb;
    Rigidbody rb  { get { if(_rb==null) _rb=shipInterface.rb; return _rb; }}

    Transform ship {get {return rb.transform;}}

    private Vector3 velocity = Vector3.zero;
    private Vector3 angularVelocity = Vector3.zero;
    private Vector3 prevVelocity = Vector3.zero;
    private Vector3 prevAngularVelocity = Vector3.zero;

    // void FixedUpdate()
    // {
    //     Vector3 acceleration = (rb.velocity - prevVelocity) / Time.deltaTime;
    //     Vector3 angularAcceleration = (rb.angularVelocity - prevAngularVelocity) / Time.deltaTime;
    //     prevVelocity = rb.velocity;
    //     prevAngularVelocity = rb.angularVelocity;

    //     Vector3 targetPosition = -ship.InverseTransformDirection(acceleration) * accelerationEffect -
    //                              -ship.InverseTransformDirection(angularAcceleration) * angularEffect;

    //     targetPosition = Vector3.ClampMagnitude(targetPosition, maxOffset);

    //     transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime);
    // }

    void FixedUpdate()
    {
        Vector3 acceleration = (rb.velocity - prevVelocity) / Time.deltaTime;
        Vector3 angularAcceleration = (rb.angularVelocity - prevAngularVelocity) / Time.deltaTime;
        prevVelocity = rb.velocity;
        prevAngularVelocity = rb.angularVelocity;

        Vector3 targetPosition = -ship.InverseTransformDirection(acceleration) * accelerationEffect -
                                 -ship.InverseTransformDirection(angularAcceleration) * angularEffect;

        targetPosition = Vector3.ClampMagnitude(targetPosition, maxOffset);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime);
    }
}


// public class CockpitCameraMovement : MonoBehaviour
// {
//     public float accelerationEffect = 0.1f;
//     public float angularEffect = 0.1f;
//     public float smoothTime = 0.3f;

//     ShipInterface _shipInterface;
//     ShipInterface shipInterface {  get { if(_shipInterface==null ) { _shipInterface = GetComponentInParent<ShipInterface>();  }  return _shipInterface; } }

//     Rigidbody _rb;
//     Rigidbody rb  { get { if(_rb==null) _rb=shipInterface.rb; return _rb; }}

//     Transform ship {get {return rb.transform;}}

//     private Vector3 velocity = Vector3.zero;
//     private Vector3 prevVelocity = Vector3.zero;

//     void FixedUpdate()
//     {
//         Vector3 acceleration = ship.InverseTransformDirection(rb.velocity - prevVelocity) / Time.deltaTime;
//         prevVelocity = rb.velocity;

//         Vector3 targetPosition = ship.TransformDirection(acceleration) * accelerationEffect +
//                                  ship.TransformDirection(rb.angularVelocity) * angularEffect;
        
//         transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smoothTime);
//     }   
// }
