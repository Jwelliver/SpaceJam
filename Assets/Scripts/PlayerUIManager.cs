using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




public class PlayerUIManager : MonoBehaviour
{
    public Player player;
    public GameObject playerUIRootCanvas;
    public GameObject shipCustomizationUI;

    void Start() {
        player.OnBeforeSpawnShip+=HideAllUI;
    }

    void OnDisable() {
        player.OnBeforeSpawnShip-=HideAllUI;
    }


    public void HideAllUI() {
        playerUIRootCanvas.SetActive(false);
    }

    public void EnableUIRoot() {
        if(!playerUIRootCanvas.activeSelf) {
            playerUIRootCanvas.SetActive(true);
        }
    }

    public void SetShipCustomizationUI(bool show) {
        if(show) { EnableUIRoot(); }
        shipCustomizationUI.SetActive(show);
    }

}
