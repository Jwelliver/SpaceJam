using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*
Lives on Player Object and sends inputs to ShipController
*/


/*
TODO:

 - Make input methods public in shipController
 - Call all the inputmethods from here
 - Assign this to Player/PlayerInput
 - Handle unassign via ship events;

 - Look into firing these using unity events?

*/

public class PlayerShipInputHandler : MonoBehaviour
{
    public Player player;
    public PlayerInput playerInput;
    private ShipController shipController;

    void Awake() {
        player.OnShipSpawned += OnPlayerShipSpawned;
    }

    void OnDisable() {
        player.OnShipSpawned -= OnPlayerShipSpawned;
    }

    public void OnPlayerShipSpawned(ShipInterface shipInterface) {
        shipController = shipInterface.shipController;
    }

    void Update()
    {
        if (playerInput.actions["FirePrimary"].IsPressed())
        {
            shipController?.OnFirePrimary();
        }
    }

    void OnSpawnShip() => player.SpawnShip(); //TODO:  Temp for testing; Should be somewhere else probs


    //TODO: Make these Unity events and assign directly to ship controller
    void OnRoll(InputValue v) => shipController?.OnRoll(v.Get<float>());
    void OnPitch(InputValue v) => shipController?.OnPitch(v.Get<float>());
    void OnYaw(InputValue v) => shipController?.OnYaw(v.Get<float>());
    void OnThrust(InputValue v) => shipController?.OnThrust(v.Get<float>());
    void OnToggleFlightAssist() => shipController?.OnToggleFlightAssist();

    // void OnFirePrimary() => shipWeapons.FirePrimary();
    void OnFireSecondary() => shipController?.OnFireSecondary();
}
