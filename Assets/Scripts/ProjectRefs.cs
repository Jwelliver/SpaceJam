using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectRefs : MonoBehaviour
{
    public static ProjectRefs Instance;
    [SerializeField] PlayerInputManager _playerInputManager;
    [SerializeField] AmmoRef _ammoRef;
    [SerializeField] ShipCustomizationStore _shipCustomizationStore;
    public static AmmoRef ammoRef {get {return ProjectRefs.Instance._ammoRef;}}
    public static PlayerInputManager playerInputManager {get {return ProjectRefs.Instance._playerInputManager;}}
    public static ShipCustomizationStore shipCustomizationStore {get {return ProjectRefs.Instance._shipCustomizationStore;}}
    

    void Awake() {
        if(ProjectRefs.Instance!=null && ProjectRefs.Instance!=this) {
            Destroy(this);
        } else {
            ProjectRefs.Instance = this;
        }
    }


    void OnDisable() {
        if(ProjectRefs.Instance==this) {
            Destroy(this);
        }
    }
}
