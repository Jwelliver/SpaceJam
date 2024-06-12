using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectRefs : MonoBehaviour
{

    [SerializeField] AmmoRef _ammoRef;
    public static AmmoRef ammoRef {get {return ProjectRefs.Instance._ammoRef;}}
    public static ProjectRefs Instance;

    void Start() {
        if(ProjectRefs.Instance!=null) {
            Destroy(this);
        } else {
            ProjectRefs.Instance = this;
        }

    }
}
