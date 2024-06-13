using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectRefs : MonoBehaviour
{
    public static ProjectRefs Instance;
    [SerializeField] PlayerInputManager _playerInputManager;
    [SerializeField] AmmoRef _ammoRef;
    public static AmmoRef ammoRef {get {return ProjectRefs.Instance._ammoRef;}}
    public static PlayerInputManager playerInputManager {get {return ProjectRefs.Instance._playerInputManager;}}
    

    void Awake() {
        if(ProjectRefs.Instance!=null) {
            Destroy(this);
        } else {
            ProjectRefs.Instance = this;
        }

    }
}
