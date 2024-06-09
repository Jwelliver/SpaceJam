using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Radar minimap:
// - use sphere trigger
// - exit/enter events search for radar trackable and add to list of tracked objects
// - refresh method uses sphere overlap to find those that may be in trigger.. use this on init
// - radar trackables define their type, which defines their associated minimap prefab (in scriptable object)
// .. this is used for objects like ships, so we don't need to recreate every collider.
// - by default, or for unknowns, we get the mesh filter from the object to use as the mini
// -- the mini obj prefab is instantiated and the mesh is applied. Hologram shader used.
// -- the scale is made to be x times the object scale 
// -- the position relative to the player is scaled down from the position within the tracking sphere relative to player ship dir
// - diff colors can be used
// - use a set of faint rings to represent horizon relative to ship and give a sense of distance.

// Optionally:
// - to allow line of site only, for each tracked obj, cast a ray and see if it hits

// - allow energy distribution sys to govern how 


public class RadarDisplay : MonoBehaviour
{
    [SerializeField] Transform radar;
    [SerializeField] Transform trackedObjectsContainer;
    [SerializeField] Transform defaultSignaturePrefab;
    [SerializeField] float radarRadius;
    [SerializeField] float displayScale = 0.05f;
    [SerializeField] float refreshRate = -1; //-1=realtime
    private Transform radarDisplay;
    private Dictionary<int, RadarTrackedObj> trackedObjectsById = new Dictionary<int, RadarTrackedObj>(); //stores RadarTrackedObj by transform's InstanceID
    private float lastRefreshTime;
    private TriggerColliderHandler radarTriggerHandler;
    private SphereCollider radarSphere;

    void Awake(){
        radarDisplay = transform;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Setup Radar Sphere
        radarTriggerHandler = radar.GetComponent<TriggerColliderHandler>();
        radarSphere = (SphereCollider)radarTriggerHandler.col;
        radarSphere.radius = radarRadius;
        // Subscribe to events
        radarTriggerHandler.OnEnter+=OnRadarEnter;
        radarTriggerHandler.OnExit+=OnRadarExit;

        ValidateTrackedObjects();
    }

    void OnDestroy() {
        //Unsub from trigger handler events
        radarTriggerHandler.OnEnter-=OnRadarEnter;
        radarTriggerHandler.OnExit-=OnRadarExit;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastRefreshTime > refreshRate) {
            RefreshDisplay();
            
        }
    }

    void OnRadarEnter(string triggerId, Collider col) {
        ActivateTrackedObject(col);
    }

    void OnRadarExit(string triggerId, Collider col) {
        DeactivateTrackedObject(col);
    }

    void ActivateTrackedObject(Collider col) {
        FindRadarTrackedObject(col).SetIsActive(true);
    }

    void DeactivateTrackedObject(Collider col) {
        FindRadarTrackedObject(col).SetIsActive(false);
    }

    RadarTrackedObj AddNewTrackedObject(Collider col) {
        int id = col.transform.GetInstanceID();
        if(trackedObjectsById.ContainsKey(id)) { //return existing obj if found
            return trackedObjectsById[id];
        }
        Transform radarSignatureTransform = Instantiate(defaultSignaturePrefab, trackedObjectsContainer);
        RadarTrackedObj newTrackedObj = new RadarTrackedObj(col.transform, radarSignatureTransform);
        trackedObjectsById.Add(id, newTrackedObj);
        return newTrackedObj;
    }

    void RefreshDisplay() {
        foreach(RadarTrackedObj obj in trackedObjectsById.Values) {
            obj.radarSignatureTransform.position = GetMinimapPosition(obj.actualTransform.position);
        }
        lastRefreshTime = Time.time;
    }

    Vector3 GetMinimapPosition(Vector3 objectPosition) {
        Vector3 localPos = objectPosition - radar.position;
        Vector3 forwardProj = Vector3.ProjectOnPlane(radar.forward, Vector3.up);
        Quaternion rotation = Quaternion.LookRotation(forwardProj, Vector3.up);
        Vector3 rotatedPos = rotation * localPos;
        return radarDisplay.position + rotatedPos.normalized * displayScale;//radius;
    }

    void ValidateTrackedObjects() {
        //use sphereoverlap to find all objects (without relying on trigger); 
        //TODO: Compare to trackedObjects list and update as needed; Add missing, remove stales.
        Collider[] found = Physics.OverlapSphere(radar.position, radarRadius);
        foreach(Collider c in found) { //Add missing objs using find with create enabled
            FindRadarTrackedObject(c, createIfNotFound:true);
        }
    }

    RadarTrackedObj FindRadarTrackedObject(Collider col, bool createIfNotFound=true) {
        int id = col.transform.GetInstanceID();
        if(trackedObjectsById.ContainsKey(id)) {
            return trackedObjectsById[id];
        }
        if(createIfNotFound) {
            return AddNewTrackedObject(col);
        }
        return null;
    }
}


public class RadarTrackedObj {

    public int rootId;
    public int colliderId;
    public Transform actualTransform;
    public Transform radarSignatureTransform;
    private bool isActive = true;

    public RadarTrackedObj(Transform actualTransform, Transform radarSignatureTransform) {
        this.actualTransform = actualTransform;
        this.radarSignatureTransform = radarSignatureTransform;
        this.rootId = actualTransform.root.GetInstanceID();
        this.colliderId = actualTransform.GetInstanceID(); //TODO: May want to utilize root instance ID for tracking
    }

    public bool GetIsActive() => this.isActive;

    public void SetIsActive(bool newValue) {
        this.isActive = newValue;
        this.radarSignatureTransform.gameObject.SetActive(newValue);
    }
}