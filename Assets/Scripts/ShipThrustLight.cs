using UnityEngine;
using UnityEngine.InputSystem;

public class ShipThrustLight : MonoBehaviour
{
    public ShipInterface shipInterface;
    private ShipController shipController;
    public Light thrustLight;
    public float minIntensity;
    public float maxIntensity;

    void Awake() {
        shipController = shipInterface.shipController;
    }


    // Update is called once per frame
    void Update()
    {
        AdjustLight();
    }

    void AdjustLight() {
        thrustLight.intensity = Mathf.Lerp(minIntensity,maxIntensity,Mathf.Abs(shipController.thrustInput));
        //todo: adjust color based on pos/neg input (forward/backward thrust) e.g. light blue forward/orange backward
    }
}
