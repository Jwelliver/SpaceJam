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
    [SerializeField] Transform defaultBlipPrefab;
    [SerializeField] float radarRadius;
    [SerializeField] float displayScale = 0.05f;
    [SerializeField] float refreshRate = -1; //-1=realtime
    [Header("Blip Settings")]
    [SerializeField] float minBlipScale = 0.0001f;
    [SerializeField] float maxBlipScale = 0.005f;
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

    public Dictionary<int, RadarTrackedObj> GetTrackedObjects() {
        return trackedObjectsById;
    }

    void OnRadarEnter(string triggerId, Collider col) {
        ActivateTrackedObject(col);
    }

    void OnRadarExit(string triggerId, Collider col) {
        DeactivateTrackedObject(col);
    }

    void ActivateTrackedObject(Collider col) {
        RadarTrackedObj trackedObj = FindRadarTrackedObject(col);
        if(trackedObj!=null) trackedObj.SetIsActive(true);
    }

    void DeactivateTrackedObject(Collider col) {
        RadarTrackedObj trackedObj = FindRadarTrackedObject(col, false);
        if(trackedObj!=null) trackedObj.SetIsActive(false);
    }

    RadarTrackedObj AddNewTrackedObject(Collider col) {
        if(isExcludedCollider(col)) {return null;}
        int id = col.transform.GetInstanceID();
        if(trackedObjectsById.ContainsKey(id)) { //return existing obj if found
            return trackedObjectsById[id];
        }
        Transform blip = Instantiate(defaultBlipPrefab, trackedObjectsContainer);
        blip.name = "Blip - "+ col.transform.name;
        RadarTrackedObj newTrackedObj = new RadarTrackedObj(col.transform, blip);
        trackedObjectsById.Add(id, newTrackedObj);
        return newTrackedObj;
    }

    void RefreshDisplay() {
        foreach(RadarTrackedObj trackedObj in trackedObjectsById.Values) {
            if(!trackedObj.GetIsActive()) {continue;}
            UpdateBlip(trackedObj);
        }
        lastRefreshTime = Time.time;
    }

    void UpdateBlip(RadarTrackedObj trackedObj) {
        // Set Position
        trackedObj.blip.position = GetBlipPosition(trackedObj);
        // Set Scale
        trackedObj.blip.localScale = GetBlipScale(trackedObj);
    }

    Vector3 GetBlipScale(RadarTrackedObj trackedObj) {
         //TODO: Implement a base scale (scale multiplier? on RadarTrackedObj) to maintain a representation of relative scale while still scaling based on distance
        float distance = Vector3.Distance(radar.position, trackedObj.obj.position);
        float pctOfMaxRange = distance/radarRadius;
        float scale = Mathf.Lerp(maxBlipScale, minBlipScale, pctOfMaxRange);
        // Debug.Log("GetBlipScale() > "+ "distance: "+distance+" pctMaxRng: "+pctOfMaxRange+" scale: "+scale);
        return new Vector3(scale,scale,scale);
    }

    Vector3 GetBlipPosition(RadarTrackedObj trackedObj) {
        Vector3 posDiff = trackedObj.obj.position - radar.position;
        float distance = posDiff.magnitude;
        float pctMaxRng = distance/radarRadius;
        float distanceFromCenterOfDisplay = displayScale*pctMaxRng;
        Vector3 dir = posDiff.normalized;
        return radarDisplay.position+dir*distanceFromCenterOfDisplay;
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

    bool isExcludedCollider(Collider col) {
        //Used to check if we should ignore this collider before adding it to trackedObjects

        //Exclude any collider that belongs to this ship;
        // if(col.attachedRigidbody==radarTriggerHandler.col.attachedRigidbody) {return true;}
        if(radar.transform.root==col.transform.root) {return true;}
        return false;

    }
}


public class RadarTrackedObj {

    public int rootId;
    public int colliderId;
    public Transform obj;
    public Transform blip;
    private bool isActive = true;

    public RadarTrackedObj(Transform obj, Transform blip) {
        this.obj = obj;
        this.blip = blip;
        this.rootId = obj.root.GetInstanceID();
        this.colliderId = obj.GetInstanceID(); //TODO: May want to utilize root instance ID for tracking
    }

    public bool GetIsActive() => this.isActive;

    public void SetIsActive(bool newValue) {
        this.isActive = newValue;
        this.blip.gameObject.SetActive(newValue);
    }
}

public class RadarBlipUpdater {
    RadarDisplay radarDisplay;
    public RadarBlipUpdater(RadarDisplay radarDisplay) {
        this.radarDisplay = radarDisplay;
    }

    public void UpdateBlips() {

    }

}