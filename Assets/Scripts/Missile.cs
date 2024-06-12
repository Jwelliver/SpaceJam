using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
TODO:
    - missile should be a projectile with a separate objects in charge of tracking, and exploding.
        - separate tracking (could use radar), steering, and detonating into extracted components.
*/
public class Missile : Projectile
{
    public SphereCollider trackingTrigger;
    public float turnRate;
    public float trackingRange = 20;
    public float detonateRange = 2;
    public float damageRange;
    public float explosiveForce;
    public float armDelay;
    public int ownerId;
    public AudioClip detonation;
    private List<TrackedTarget> trackedTargets = new List<TrackedTarget>();
    private TrackedTarget currentTarget;

    private bool isArmed;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        
       trackingTrigger.radius = trackingRange;
        damageRange = explosiveForce * 0.01f;

    }

    void Start() {
        ownerId = sourceWeapon.transform.root.GetInstanceID(); //TODO: Use MissileTrackable Id;
    }

    void CheckArm()
    {
        if (Time.time - armDelay > timeFired)
        {
            isArmed = true;
        }
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        // rb.velocity = transform.forward * speed;
        if (!isArmed)
        {
            CheckArm();
        }
        // if (Time.time - timeFired > lifeTime) { Detonate(); }
        if (currentTarget != null)
        {
            if (!ValidateTarget()) { RecalculateCurrentTarget(); return; }
            if (isArmed)
            {
                PerformTracking();
                CheckDetonateRange();
            }
        }
    }

    bool ValidateTarget()
    {
        try { if (currentTarget._transform != null) { return true; } }
        catch { currentTarget = null; Debug.Log(transform.name + ": Target Lost: No longer valid."); }
        return false;
    }

    void PerformTracking()
    {
        Vector3 targetPos = currentTarget.position;
        Vector3 targetVel = currentTarget.velocity;
        // Vector3 positionDiff = currentTarget.transform.position - transform.position;
        float interceptTime = Vector3.Distance(targetPos, transform.position) / rb.velocity.magnitude;
        Vector3 predictedTargetPos = targetPos + (targetVel * interceptTime);

        // Debug.Log("Missile: Tracking Target " + currentTarget.transform.name);
        Vector3 directionToTarget = (predictedTargetPos - transform.position).normalized;

        // Calculate the rotation needed to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotate the missile towards the target
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, turnRate * Time.deltaTime));
    }

    void CheckDetonateRange()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) < detonateRange)
        {
            Detonate();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<MissileTrackable>(out MissileTrackable potentialTarget))
        {
            if (potentialTarget.transform.root.GetInstanceID() == ownerId) { return; }
            trackedTargets.Add(new TrackedTarget(potentialTarget.transform));
            RecalculateCurrentTarget();
        }
    }

    void RecalculateCurrentTarget()
    {
        // If no targets exist, remove currentTarget if it exists and exit.
        if (trackedTargets.Count == 0) { currentTarget = null; return; }
        //Sort by signalStrength //*Note: if you change this, you may want to change how this method is called. Right now, just called when target enters or exits the trackingSphere
        trackedTargets.Sort((TrackedTarget a, TrackedTarget b) => { return (int)b.signatureStrength - (int)a.signatureStrength; });
        //Select top target
        currentTarget = trackedTargets[0];
    }

    void OnTriggerExit(Collider col)
    {
        if (col.TryGetComponent<MissileTrackable>(out MissileTrackable outOfRangeTarget))
        {
            TrackedTarget targetToRemove = trackedTargets.Find((TrackedTarget i) => { return i._transform == outOfRangeTarget.transform; });
            try { trackedTargets.Remove(targetToRemove); }
            catch { throw new System.Exception("Tried to remove TrackedTarget, but it wasn't found in the TrackedTarget list. Find out why it wasn't added."); }
        }
    }

    new void OnTimeout() {
        base.OnTimeout();
    }

    new void OnLifeOver() {
        Detonate();
        base.OnLifeOver();
    }

    void Detonate()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, damageRange);
        Debug.Log("Missile: Detonate: "+nearby);
        foreach (Collider c in nearby)
        {
            if(c.isTrigger) {continue;}
            Transform root = c.transform.root;
            if (root == transform) { continue; }

            Rigidbody foundRb = c.attachedRigidbody;
            if (foundRb != null) {
                foundRb.AddExplosionForce(explosiveForce, transform.position, detonateRange); 
                Damageable foundDamagable = foundRb.GetComponentInChildren<Damageable>();
                if (foundDamagable != null)
            {
                float distance = Vector3.Distance(transform.position, foundDamagable.transform.position);
                float damage = explosiveForce * (1-(distance / damageRange));
                Debug.Log("Detonate > Add Damage | root: "+root.name+" name: "+c.name+ " | distance: "+ distance + " | damage: "+ damage+ " | missilePos: " + transform.position + " | damageablePos: "+foundDamagable.transform.position+ " | ");
                foundDamagable.TakeDamage(damage);
            }
                }
            
        }
        //TODO: send back to pool;
        //TODO: Create explosionFx at this position
    }
}

class TrackedTarget
{

    public Transform _transform;
    Rigidbody rb;
    public float signatureStrength;
    public Vector3 position { get { return _transform.position; } }
    public Vector3 velocity { get { return rb.velocity; } }
    //TODO: Store time when made current target; then use the (currentTime to intercept/InitialTimeToIntercept.. (?or max time to intercept? Just set it if it grows larger?) )  as the pct to use as a controlling variable to lerp between predicted position and actual position
    public TrackedTarget(Transform target)
    {
        this._transform = target;
        this.rb = target.GetComponent<Rigidbody>();
        this.signatureStrength = target.GetComponent<MissileTrackable>().signatureStrength;
    }
}

//========= 061124 Orig before refactor with projectile
// public class Missile : MonoBehaviour
// {
//     public SphereCollider trackingTrigger;
//     public float speed;
//     public float turnRate;
//     public float trackingRange = 20;
//     public float detonateRange = 2;
//     public float damageRange;
//     public float explosiveForce;
//     public float lifeTime;
//     public float armDelay;
//     public int ownerId;
//     public AudioClip detonation;
//     private Rigidbody rb;
//     private List<TrackedTarget> trackedTargets = new List<TrackedTarget>();
//     private TrackedTarget currentTarget;
//     private float startTime; // time that the missile was instantiated.
//     private bool isArmed;

//     // Start is called before the first frame update
//     void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         trackingTrigger.radius = trackingRange;
//         damageRange = explosiveForce * 0.01f;
//         startTime = Time.time;
//     }

//     void CheckArm()
//     {
//         if (Time.time - armDelay > startTime)
//         {
//             isArmed = true;
//         }
//     }

//     // Update is called once per frame
//     void FixedUpdate()
//     {
//         rb.velocity = transform.forward * speed;
//         if (!isArmed)
//         {
//             CheckArm();
//         }
//         if (Time.time - startTime > lifeTime) { Detonate(); }
//         if (currentTarget != null)
//         {
//             if (!ValidateTarget()) { RecalculateCurrentTarget(); return; }
//             if (isArmed)
//             {
//                 PerformTracking();
//                 CheckDetonateRange();
//             }
//         }
//     }

//     bool ValidateTarget()
//     {
//         try { if (currentTarget._transform != null) { return true; } }
//         catch { currentTarget = null; Debug.Log(transform.name + ": Target Lost: No longer valid."); }
//         return false;
//     }

//     void PerformTracking()
//     {
//         Vector3 targetPos = currentTarget.position;
//         Vector3 targetVel = currentTarget.velocity;
//         // Vector3 positionDiff = currentTarget.transform.position - transform.position;
//         float interceptTime = Vector3.Distance(targetPos, transform.position) / rb.velocity.magnitude;
//         Vector3 predictedTargetPos = targetPos + (targetVel * interceptTime);

//         // Debug.Log("Missile: Tracking Target " + currentTarget.transform.name);
//         Vector3 directionToTarget = (predictedTargetPos - transform.position).normalized;

//         // Calculate the rotation needed to look at the target
//         Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

//         // Smoothly rotate the missile towards the target
//         rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, turnRate * Time.deltaTime));
//     }

//     void CheckDetonateRange()
//     {
//         if (Vector3.Distance(transform.position, currentTarget.position) < detonateRange)
//         {
//             Detonate();
//         }
//     }

//     void OnTriggerEnter(Collider col)
//     {
//         if (col.TryGetComponent<MissileTrackable>(out MissileTrackable potentialTarget))
//         {
//             if (potentialTarget.transform.root.GetInstanceID() == ownerId) { return; }
//             trackedTargets.Add(new TrackedTarget(potentialTarget.transform));
//             RecalculateCurrentTarget();
//         }
//     }

//     void RecalculateCurrentTarget()
//     {
//         // If no targets exist, remove currentTarget if it exists and exit.
//         if (trackedTargets.Count == 0) { currentTarget = null; return; }
//         //Sort by signalStrength //*Note: if you change this, you may want to change how this method is called. Right now, just called when target enters or exits the trackingSphere
//         trackedTargets.Sort((TrackedTarget a, TrackedTarget b) => { return (int)b.signatureStrength - (int)a.signatureStrength; });
//         //Select top target
//         currentTarget = trackedTargets[0];
//     }

//     void OnTriggerExit(Collider col)
//     {
//         if (col.TryGetComponent<MissileTrackable>(out MissileTrackable outOfRangeTarget))
//         {
//             TrackedTarget targetToRemove = trackedTargets.Find((TrackedTarget i) => { return i._transform == outOfRangeTarget.transform; });
//             try { trackedTargets.Remove(targetToRemove); }
//             catch { throw new System.Exception("Tried to remove TrackedTarget, but it wasn't found in the TrackedTarget list. Find out why it wasn't added."); }
//         }
//     }

//     void OnCollisionEnter(Collision collision)
//     {
//         Detonate();
//     }

//     void Detonate()
//     {
//         Collider[] nearby = Physics.OverlapSphere(transform.position, damageRange);
//         Debug.Log("Missile: Detonate: "+nearby);
//         foreach (Collider c in nearby)
//         {
//             if(c.isTrigger) {continue;}
//             Transform root = c.transform.root;
//             if (root == transform) { continue; }

//             Rigidbody foundRb = c.attachedRigidbody;
//             if (foundRb != null) {
//                 foundRb.AddExplosionForce(explosiveForce, transform.position, detonateRange); 
//                 Damageable foundDamagable = foundRb.GetComponentInChildren<Damageable>();
//                 if (foundDamagable != null)
//             {
//                 float distance = Vector3.Distance(transform.position, foundDamagable.transform.position);
//                 float damage = explosiveForce * (1-(distance / damageRange));
//                 Debug.Log("Detonate > Add Damage | root: "+root.name+" name: "+c.name+ " | distance: "+ distance + " | damage: "+ damage+ " | missilePos: " + transform.position + " | damageablePos: "+foundDamagable.transform.position+ " | ");
//                 foundDamagable.TakeDamage(damage);
//             }
//                 }
            
//         }
//         //TODO: send back to pool;
//         //TODO: Create explosionFx at this position
//         Destroy(gameObject);
//     }
// }

// class TrackedTarget
// {

//     public Transform _transform;
//     Rigidbody rb;
//     public float signatureStrength;
//     public Vector3 position { get { return _transform.position; } }
//     public Vector3 velocity { get { return rb.velocity; } }
//     //TODO: Store time when made current target; then use the (currentTime to intercept/InitialTimeToIntercept.. (?or max time to intercept? Just set it if it grows larger?) )  as the pct to use as a controlling variable to lerp between predicted position and actual position
//     public TrackedTarget(Transform target)
//     {
//         this._transform = target;
//         this.rb = target.GetComponent<Rigidbody>();
//         this.signatureStrength = target.GetComponent<MissileTrackable>().signatureStrength;
//     }


// }