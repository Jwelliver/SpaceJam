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

        InitTrackedObjects();
    }

    void OnDisable() {
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
        Transform blip = InitBlip(col);
        RadarTrackedObj newTrackedObj = new RadarTrackedObj(col.transform, blip);
        trackedObjectsById.Add(id, newTrackedObj);
        return newTrackedObj;
    }

    Transform InitBlip(Collider col) {
        Transform newBlip = Instantiate(defaultBlipPrefab, trackedObjectsContainer);
        newBlip.name = "Blip - "+ col.transform.name;
        //!Crude approach; Make RadarTrackable with types and place that on each gameobject; Have them register to a static list and radars will reference that to check position and get trackedObject data; Blip presets can be made on SO w/ blipPrefab, material, and scale multiplier
        float scaleMultiplier = 0.3f;
        Color blipColor = Color.grey;
        
        string tag = col.attachedRigidbody.transform.tag.ToLower();
        switch(tag) {
            case "asteroid": {
                scaleMultiplier= 0.2f;
                blipColor = Color.green;
                break;
            }
            case "ship": {
                scaleMultiplier= 0.7f;
                blipColor = Color.red;
                break;
            }
            case "missile": {
                scaleMultiplier= 0.5f;
                blipColor = new Color(255,0,255);
                break;
            }
        }
        newBlip.localScale *= scaleMultiplier;
        newBlip.GetComponent<MeshRenderer>().material.color = blipColor;
        return newBlip;
    }

    void RefreshDisplay() {
        // List<RadarTrackedObj> toRemove = new List<RadarTrackedObj>(); //Todo after running trackedObj.Validate(), if obj.isDestroyed, add it to a toRemove list, then remove from trackedObjectsById after loop; Also, could probably still remove in place by using for loop.
        foreach(RadarTrackedObj trackedObj in trackedObjectsById.Values) {
            UpdateBlip(trackedObj);
        }
        lastRefreshTime = Time.time;
    }

    void UpdateBlip(RadarTrackedObj trackedObj) {
        trackedObj.Validate();
        if(trackedObj.isDestroyed||!trackedObj.GetIsActive()) {return;}
        // Set Position
        UpdateBlipPosition(trackedObj);
        // Set Scale
        // UpdateBlipScale_ByDistance(trackedObj);
    }

    void UpdateBlipMaterial() {
        //TODO: When vertical angle between trackedObj and player is less than some value (e.g. 5 degrees) (i.e. is level with the horizon field), increase brightness on the obj; this should help make it clear when you're on the same plane
    }


    void UpdateBlipScale_ByDistance(RadarTrackedObj trackedObj) {
         //TODO: Implement a base scale (scale multiplier? on RadarTrackedObj) to maintain a representation of relative scale while still scaling based on distance
        float distance = Vector3.Distance(radar.position, trackedObj.obj.position);
        float pctOfMaxRange = distance/radarRadius;
        float scale = Mathf.Lerp(maxBlipScale, minBlipScale, pctOfMaxRange);
        // Debug.Log("GetBlipScale() > "+ "distance: "+distance+" pctMaxRng: "+pctOfMaxRange+" scale: "+scale);
        trackedObj.blip.localScale = new Vector3(scale,scale,scale);
    }

    void UpdateBlipPosition(RadarTrackedObj trackedObj) {
        Vector3 posDiff = trackedObj.obj.position - radar.position;
        float distance = posDiff.magnitude;
        float pctMaxRng = distance/radarRadius;
        float distanceFromCenterOfDisplay = displayScale*pctMaxRng; //todo: displayScale should be equal to the scale of the outerring; Then you will want to change this to (displayScale/2)*pctMaxRng;
        Vector3 dir = posDiff.normalized;
        trackedObj.blip.position = radarDisplay.position+dir*distanceFromCenterOfDisplay;
    }

    void InitTrackedObjects() {
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
        if(col.attachedRigidbody.transform.tag.ToLower() == "laser") return true;
        return false;

    }
}


public class RadarTrackedObj {

    public int rootId;
    public int colliderId;
    public Transform obj;
    public Transform blip;
    public bool isDestroyed;
    private bool isActive = true;


    public RadarTrackedObj(Transform obj, Transform blip) {
        this.obj = obj;
        this.blip = blip;
        this.rootId = obj.root.GetInstanceID();
        this.colliderId = obj.GetInstanceID(); //TODO: May want to utilize root instance ID for tracking
    }

    ~RadarTrackedObj() {
        Debug.Log(obj.name+ "_TrackedObj.Deconstructor()");
        CleanupSelf();
    }

    public bool GetIsActive() => this.isActive;

    public void SetIsActive(bool newValue) {
        this.isActive = newValue;
        this.blip.gameObject.SetActive(newValue);
    }

    public void Validate(){
        if(isDestroyed) return; // Return if already destroyed and cleanedup
        if(this.obj==null) {
            CleanupSelf();
        }
    }

    void CleanupSelf() {
        GameObject.Destroy(this.blip.gameObject);
        this.obj = null;
        isDestroyed = true;
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