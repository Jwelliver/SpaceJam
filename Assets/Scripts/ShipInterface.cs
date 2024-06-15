
using System;
using UnityEngine;

public class ShipInterface : MonoBehaviour
{
    public Rigidbody rb;
    public ShipController shipController;
    public ShipEnergy shipEnergy;
    public ShipWeapons shipWeapons;
    public Damageable shipDamage;
    public ShipCustomizer shipCustomizer;
    public Transform cameraParent;
    public Action<ShipInterface> OnShipObjectDisabled;
    private Player assignedPlayer;

    public Player GetAssignedPlayer() {
        return assignedPlayer;
    }

    public void AssignPlayer(Player player) {
        assignedPlayer = player;
        //Place camera
        Transform playerCam = player.cam.transform;
        playerCam.parent = cameraParent;
        playerCam.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        shipCustomizer.ApplyShipCustomizationSettings(player.playerData.shipCustomizationSettings);
        Debug.Log("Shipinterface.AssignPlayer() > \n"+player.playerData.shipCustomizationSettings.GetAsString());
    }

    void OnDisable() {
        OnShipObjectDisabled?.Invoke(this);
    }

    public Weapon[] GetAllWeapons() {
        return transform.GetComponentsInChildren<Weapon>();
    }


}
