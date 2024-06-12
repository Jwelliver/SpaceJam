using UnityEngine;
using UnityEngine.InputSystem;

public class ShipThrustLight : MonoBehaviour
{
    public PlayerInput playerInput;
    public Light thrustLight;
    public float minIntensity;
    public float maxIntensity;


    // Update is called once per frame
    void Update()
    {
        AdjustLight();
    }

    void AdjustLight() {
        float thrustInput = playerInput.actions["Thrust"].ReadValue<float>();
        thrustLight.intensity = Mathf.Lerp(minIntensity,maxIntensity,Mathf.Abs(thrustInput));
        //todo: adjust color based on pos/neg input (forward/backward thrust) e.g. light blue forward/orange backward
    }
}
