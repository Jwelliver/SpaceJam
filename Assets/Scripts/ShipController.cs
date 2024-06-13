
using UnityEngine;

public class ShipController : MonoBehaviour

{
    //TODO: Rename to ShipInputHandler; Extract movement, "Engine Audio", and camera handling;
    // Public variables to tweak spaceship controls behavior

    [Header("Ship Controls")]

    public float thrustPower = 50f;
    public float pitchSpeed = 2f;
    public float rollSpeed = 2f;
    public float yawSpeed = 2f;
    public float maxSpeed = 20f;
    public float dampingRate = 0.1f;
    public ShipInterface shipInterface;
    public AudioSource engineAudio1;
    public AudioSource engineAudio2;
    private ShipEnergy shipEnergy;
    private ShipWeapons shipWeapons;
    private Rigidbody rb;
    public float pitchInput;
    public float rollInput;
    public float yawInput;
    public float thrustInput;
    // bool isFiring
    public bool isFreeFlightEnabled;
    Vector3 prevAngularVelocity = new Vector3(0, 0, 0);

    void Awake() {
        shipEnergy = shipInterface.shipEnergy;
        shipWeapons = shipInterface.shipWeapons;
        rb = shipInterface.rb;
    }


    public void OnRoll(float v) => rollInput = v;
    public void OnPitch(float v) => pitchInput = v;
    public void OnYaw(float v) => yawInput = v;
    public void OnThrust(float v) => thrustInput = v;
    public void OnToggleFreeFlight() => isFreeFlightEnabled = !isFreeFlightEnabled;
    public void OnFirePrimary() => shipWeapons.FirePrimary();
    public void OnFireSecondary() => shipWeapons.FireSecondary();


    public float GetSpeed()
    {
        Vector3 forwardDirection = transform.forward;
        return Mathf.Floor(Vector3.Dot(forwardDirection, rb.velocity));
    }


    void FixedUpdate()
    {
        HandleShipMovement();
        // HandleWeapons();
    }

    // void HandleWeapons() {

    // }


    void HandleShipMovement()
    {
        //Handle Thrust
        HandleEngineAudio();
        if (shipEnergy.CanThrust())
        {
            Vector3 thrustForce = transform.forward * thrustInput * thrustPower;
            if (GetSpeed() < maxSpeed)
            {
                rb.AddForce(thrustForce);
                float amt = Mathf.Abs(thrustForce.magnitude);
                if (amt > 0)
                {
                    shipEnergy.OnThrust(1);
                }
            }
        }


        //Handle Maneuvering
        if (shipEnergy.CanManeuver())
        {
            float pitch = pitchInput * pitchSpeed * Time.fixedDeltaTime;
            float roll = rollInput * rollSpeed * Time.fixedDeltaTime;
            float yaw = yawInput * yawSpeed * Time.fixedDeltaTime;
            Quaternion rotation = Quaternion.Euler(pitch, yaw, -roll);
            rb.MoveRotation(rb.rotation * rotation);

            prevAngularVelocity.x = pitch;
            prevAngularVelocity.y = yaw;
            prevAngularVelocity.z = roll;

            float amt = Mathf.Abs(pitch) + Mathf.Abs(roll) + Mathf.Abs(yaw);
            if (amt > 0) { shipEnergy.OnManeuver(amt); }

            //Handle Damping
            if (!isFreeFlightEnabled) ApplyDamping();
        }
        else
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(prevAngularVelocity));
        }
    }

    void ApplyDamping()
    {
        ApplyLinearDamping();
        ApplyAngularDamping();
    }

    void ApplyLinearDamping()
    {
        // Get the spaceship's forward direction
        Vector3 forwardDirection = transform.forward;

        // Project the current velocity onto the forward direction
        Vector3 forwardVelocity = Vector3.Project(rb.velocity, forwardDirection);

        // Calculate the sideways velocity (velocity that is not in the forward direction)
        Vector3 sidewaysVelocity = rb.velocity - forwardVelocity;

        // Gradually reduce the sideways velocity by applying a damping factor
        Vector3 newSidewaysVelocity = Vector3.Lerp(sidewaysVelocity, Vector3.zero, dampingRate * Time.fixedDeltaTime);

        // The new velocity is the forward velocity plus the dampened sideways velocity
        rb.velocity = forwardVelocity + newSidewaysVelocity;
    }

    void ApplyAngularDamping()
    {
        // Get current angular velocity
        Vector3 currentAngularVelocity = rb.angularVelocity;

        // Gradually reduce the angular velocity by applying a damping factor
        Vector3 newAngularVelocity = Vector3.Lerp(currentAngularVelocity, Vector3.zero, dampingRate * Time.fixedDeltaTime);

        // Assign the dampened angular velocity back to the rigidbody
        rb.angularVelocity = newAngularVelocity;
    }

    void HandleEngineAudio()
    {
        //Engine audio 2
        float neutralPitch1 = 0.87f;
        float lowEnergyPitch1 = 0.5f;
        float thrustFullPitch1 = 1.1f;
        //engine audio 2
        float neutralPitch2 = 1f;
        float lowEnergyPitch2 = 0.87f;
        float thrustFullPitch2 = 1.3f;

        float newPitch1 = neutralPitch1;
        float newPitch2 = neutralPitch2;

        // float pitchPct = 
        // newPitch = neutralPitch + (thrustFullPitch - neutralPitch)/


        if (!shipEnergy.CanThrust())
        {
            newPitch1 = lowEnergyPitch1;
            newPitch2 = lowEnergyPitch2;
        }
        else if (thrustInput == 0)
        {
            newPitch1 = neutralPitch1;
            newPitch2 = neutralPitch2;
        }
        else if (thrustInput > 0)
        {
            newPitch1 = thrustFullPitch1;
            newPitch2 = thrustFullPitch2;
        }
        engineAudio1.pitch = newPitch1;
        engineAudio2.pitch = newPitch2;
    }
}

