using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum CustomizableComponentID { //TODO Move to shipcustomizationstore
    Body=0,
    Wing=1,
    Weapons=2,
    Laser=3
}

public class ShipSelectScreen : MonoBehaviour
{

    public Player player;
    public PlayerInput playerInput;
    public ShipInterface displayShipInterface;
    public Transform displayLaser;
    public TextMeshProUGUI selectedComponentText;

    private ShipCustomizationSettings shipCustomizationSettings;

    private int maxComponentId = 3; // TODO: Get Dynamically


    private CustomizableComponentID selectedComponentId = CustomizableComponentID.Body;

    private List<Material> shipMaterials;
    private List<Color> laserColors;


    void Start() {
        shipCustomizationSettings = player.playerData.shipCustomizationSettings;//ShipCustomizationStore.GetDefaultShipCustomizationSettings();
        shipMaterials = ShipCustomizationStore.Instance.shipMaterials;
        laserColors = ShipCustomizationStore.Instance.laserColors;
        SetSelectedComponentId(0);
    }

    void AdjustSelectedComponent(int adjustment) {
        int newValue = (int)selectedComponentId+adjustment;
        if(newValue< 0) { newValue=maxComponentId;
        } else if(newValue>maxComponentId) {newValue = 0;}
        SetSelectedComponentId(newValue);
    }

    void SetSelectedComponentId(int newValue) {
        selectedComponentId = (CustomizableComponentID)newValue;
        selectedComponentText.text = selectedComponentId.ToString();
        displayLaser.gameObject.SetActive(selectedComponentId==CustomizableComponentID.Laser);
    }

    void AdjustSelectedColorIndex(int adjustment) {
        switch(selectedComponentId) {
            case(CustomizableComponentID.Body): {
                shipCustomizationSettings.bodyMaterialIndex=WrapIndex(shipCustomizationSettings.bodyMaterialIndex+adjustment,shipMaterials);
                break;
            }
            case(CustomizableComponentID.Wing): {
                shipCustomizationSettings.wingMaterialIndex=WrapIndex(shipCustomizationSettings.wingMaterialIndex+adjustment,shipMaterials);
                break;
            }
            case(CustomizableComponentID.Weapons): {
                shipCustomizationSettings.weaponMaterialIndex=WrapIndex(shipCustomizationSettings.weaponMaterialIndex+adjustment,shipMaterials);
                break;
            }
            case(CustomizableComponentID.Laser): {
                shipCustomizationSettings.laserColorIndex=WrapIndex(shipCustomizationSettings.laserColorIndex+adjustment,laserColors);
                break;
            }
        }
        displayShipInterface.shipCustomizer.ApplyShipCustomizationSettings(shipCustomizationSettings);
        displayShipInterface.shipCustomizer.CustomizeProjectile(displayLaser);
    }

    int WrapIndex(int requestedIndex, ICollection collection) { //constrains index and wraps overflow
        int index = requestedIndex;
        if(requestedIndex<0) {
            index = collection.Count-1;
        } else if(requestedIndex>collection.Count-1) {index = 0;}
        return index;
    }

    void Update() {
        if(playerInput.actions["Navigate"].WasPressedThisFrame()) {
            OnNavigate(playerInput.actions["Navigate"].ReadValue<Vector2>());
        }
        if(playerInput.actions["FirePrimary"].IsPressed()) {
            displayShipInterface.shipWeapons.FirePrimary();
        }
    }

    void OnNavigate(Vector2 navValue){
        if(navValue==Vector2.up) {OnNavigateUp();}
        if(navValue==Vector2.down) {OnNavigateDown();}
        if(navValue==Vector2.left) {OnNavigateLeft();}
        if(navValue==Vector2.right) {OnNavigateRight();}
    }

    void OnNavigateLeft()=>AdjustSelectedComponent(-1);
    void OnNavigateRight()=>AdjustSelectedComponent(1);
    void OnNavigateUp()=>AdjustSelectedColorIndex(1);
    void OnNavigateDown()=>AdjustSelectedColorIndex(-1);


}
