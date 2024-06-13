using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform shipPrefab;
    public Camera cam;
    public PlayerShipInputHandler playerShipInputHandler;
    private ShipInterface shipInterface;
    public Action<ShipInterface> OnShipSpawned;
    public Action OnShipDespawned;

    public void DespawnShip() {
        cam.transform.parent =  transform;
        OnShipDespawned?.Invoke();
        shipInterface = null;
    }

    public void SpawnShip() {
        if(shipInterface!=null) {
            Debug.LogWarning("Attempting to spawn ship when existing shipInterface is not null.");
            return;
        }
        //Create Ship
        Vector3 spawnLocation = GetSpawnLocation();
        Transform newShip = Instantiate(shipPrefab, spawnLocation, Quaternion.identity);
        //Get reference to shipInterface
        shipInterface = newShip.GetComponent<ShipInterface>();
        shipInterface.AssignPlayer(this);
        shipInterface.OnShipObjectDisabled += OnShipObjectDisabled;
        OnShipSpawned?.Invoke(shipInterface);
    }

    Vector3 GetSpawnLocation() {//TODO: Handle SpawnLocation elsewhere
        float r = 50;
        float GetRandom() => UnityEngine.Random.Range(-r,r);
        return new Vector3(GetRandom(), GetRandom(), GetRandom());
    }

    void OnShipObjectDisabled(ShipInterface shipInterface) {
        DespawnShip();
    }

    void OnDisable() {
        if(shipInterface!=null) {
            shipInterface.OnShipObjectDisabled-=OnShipObjectDisabled;
        }
    }


    
}
