using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;


    void Start() {
        playerInputManager = ProjectRefs.playerInputManager;
        playerInputManager.onPlayerJoined += OnPlayerJoin;
        playerInputManager.onPlayerLeft += OnPlayerLeave;
    }

    void OnDisable() {
        playerInputManager.onPlayerJoined -= OnPlayerJoin;
        playerInputManager.onPlayerLeft -= OnPlayerLeave;
    }

    void OnPlayerJoin(PlayerInput playerInput) {
        Debug.Log("Player Joined"+playerInput.name);
    }

    void OnPlayerLeave(PlayerInput playerInput) {

    }


}
