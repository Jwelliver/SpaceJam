using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class HapticEvent { // TODO: Setup Haptic definitions by ID in a scriptableoboject; 
    string id;


}

public class ShipHaptics : MonoBehaviour
{

    public float testHapticLowFreq = 0.2f;
    public float testHpaticHighFreq = 0.4f;
    public float testHapticDuration = 0.2f;
    public float testHapticMultiplier= 0.1f;
    public bool stopMotorsFirst;

    ShipInterface _shipInterface;
    
    ShipInterface shipInterface {
        get { if(_shipInterface ==null) { _shipInterface=gameObject.GetComponentInParent<ShipInterface>(); } return _shipInterface; }
        // set { _shipInterface=value; }
    }
    
    PlayerInput _playerInput;
    PlayerInput playerInput {
        get { if(_playerInput ==null) { _playerInput=shipInterface?.GetAssignedPlayer()?.playerInput; } return _playerInput; }
        // set { _playerInput=value; }
    }

    Gamepad _gamepad;
    Gamepad gamepad {
        get { if(_gamepad ==null) { _gamepad=playerInput?.GetDevice<Gamepad>(); } return _gamepad; }
    }


    Rigidbody _rb;
    Rigidbody rb {
        get { if(_rb ==null) { _rb=shipInterface.rb; } return _rb; }
    }

    Vector3 lastAccumulatedForce; // stored to apply constant force-based haptics;
    Vector3 lastVelocity;
    Vector3 lastAcceleration;

    public void PlayTestHaptic() {
        if(stopMotorsFirst) {StopMotors();}
        PlayOneShot(testHapticLowFreq, testHpaticHighFreq, testHapticDuration);
    }

    public void Play_FireWeapon(AmmoType ammoType) {
        float lowFreq = 0.8f;
        float highFreq = 0.5f;
        float duration = 0.06f;
        switch(ammoType) {
            case(AmmoType.LASER): {
                lowFreq = 0.2f;
                highFreq = 0.3f;
                duration = 0.06f;
                break;
            }
            case(AmmoType.MISSILE): {
                lowFreq = 0.25f;
                highFreq = 0.75f;
                duration = 0.2f;
                break;
            }
        }
        PlayOneShot(lowFreq, highFreq, duration);
    }

    public void Play_Collision(float impulseMagnitude) {
        float multiplier = 0.1f;
        float duration = 0.2f;
        float motorSpeed = Mathf.Clamp01(impulseMagnitude *multiplier);
        PlayOneShot(motorSpeed,motorSpeed,duration);
    }

    void OnCollisionEnter(Collision collision) {
        Play_Collision(collision.impulse.magnitude);
    }


    // void FixedUpdate() {
    //     ForceBasedHaptics();
    // }

    void ForceBasedHaptics() { // * Not in use; Has potential
        // float forceMagnitude = rb.velocity.magnitude;
        // float motorSpeed = Mathf.Clamp01(forceMagnitude / 10.0f);
        
        // float accelerationMagnitude = rb.velocity.magnitude/Time.fixedDeltaTime;
        // float motorSpeed = Mathf.Clamp01(accelerationMagnitude / 100.0f);

        // float forceMagnitude = rb.GetAccumulatedForce().magnitude;

        // Vector3 force = rb.GetAccumulatedForce();
        // float changeInForce = (lastAccumulatedForce-force).magnitude *Time.fixedDeltaTime;
        // lastAccumulatedForce = force;
        // float motorSpeed = Mathf.Clamp01(changeInForce / 10.0f);

        Vector3 velocity = rb.velocity;
        Vector3 acceleration = (lastVelocity-velocity);
        float accelerationMagnitude = acceleration.magnitude;

        float motorSpeed = Mathf.Clamp01(accelerationMagnitude / 10.0f);

        lastVelocity = velocity;
        lastAcceleration = acceleration;

        Debug.Log("ShipHaptics: FixedUpdate MotorSpeeds: "+motorSpeed.ToString()+ " | "+accelerationMagnitude);
        // if(motorSpeed<0.1f) {gamepad.StopMotors(); return;}
        gamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
    }

    public void PlayOneShot(float lowFreq, float highFreq, float duration) {
        if(gamepad==null) { return; }
        if(lowFreq<0||lowFreq>1||highFreq<0||highFreq>1) { Debug.LogWarning("Frequences must be in range 0 to 1; Values Passed: lowFreq: "+lowFreq+" highFreq: "+highFreq); 
            lowFreq=Mathf.Clamp01(lowFreq);
            highFreq=Mathf.Clamp01(highFreq);
        }
        gamepad.SetMotorSpeeds(lowFreq, highFreq);
        StartCoroutine(StopHapticsAfterTime(duration));
    }

    IEnumerator StopHapticsAfterTime(float duration) {
        yield return new WaitForSecondsRealtime(duration);
        StopMotors();
    }

    void StopMotors() {
        gamepad.SetMotorSpeeds(0,0);
    }

    void OnDisable() {
        StopAllCoroutines();
        gamepad.SetMotorSpeeds(0,0);
    }


    void OnDestroy() {
        StopAllCoroutines();
        try{
            gamepad.SetMotorSpeeds(0,0);
        } catch {

        }
        
    }
}
