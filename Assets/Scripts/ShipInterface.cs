
using System;
using UnityEngine;

public class ShipInterface : MonoBehaviour
{
    public Rigidbody rb;
    public ShipController shipController;
    public ShipEnergy shipEnergy;
    public ShipWeapons shipWeapons;
    public Damageable shipDamage;
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
    }

    void OnDisable() {
        OnShipObjectDisabled?.Invoke(this);
    }

}
