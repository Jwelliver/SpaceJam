using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject prejoinUI;

    private PlayerInputManager _playerInputManager;
    public PlayerInputManager playerInputManager {
         get {
            if(_playerInputManager ==null) {
                _playerInputManager=FindObjectOfType<PlayerInputManager>();
            }
            return _playerInputManager;
        }
    }

    public List<Player> players = new List<Player>();

    Vector3 playerSpawnPoint = Vector3.zero;

    void Start() {
        // playerInputManager = ProjectRefs.playerInputManager;
        playerInputManager.onPlayerJoined += OnPlayerJoin;
        playerInputManager.onPlayerLeft += OnPlayerLeave;
    }

    void OnDisable() {
        playerInputManager.onPlayerJoined -= OnPlayerJoin;
        playerInputManager.onPlayerLeft -= OnPlayerLeave;
    }

    void OnPlayerJoin(PlayerInput playerInput) {
        // Make sure preJoinUI is hidden
        players.Add(playerInput.transform.GetComponentInParent<Player>());
        if(prejoinUI.activeSelf) { prejoinUI.SetActive(false);}
        playerInput.transform.root.position = playerSpawnPoint;
        playerSpawnPoint+=new Vector3(10,0,0);
        Debug.Log("Player Joined"+playerInput.name);
    }

    void OnPlayerLeave(PlayerInput playerInput) {

        players.Remove(playerInput.transform.GetComponentInParent<Player>()); //TODO: Not sure if this will work since the player object might be destroyed at this point? Try using a hashmap with player identifier (look into whether playerIndex changes)
        if(prejoinUI!=null && playerInputManager.playerCount==0) {
            prejoinUI.SetActive(true);
        }
    }


}
