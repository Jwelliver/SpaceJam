using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;

    Vector3 playerSpawnPoint = Vector3.zero;

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
        playerInput.transform.root.position = playerSpawnPoint;
        playerSpawnPoint+=new Vector3(10,0,0);
        Debug.Log("Player Joined"+playerInput.name);
    }

    void OnPlayerLeave(PlayerInput playerInput) {

    }


}
