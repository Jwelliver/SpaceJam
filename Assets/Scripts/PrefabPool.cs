using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// extract Prefab pool to native class
// extract AutoReturnAfterTime to subclass of PrefabPool
// make subclass component pool that works with type T on prefab
// Make subclass of ComponentPool, AmmoPool, that can store ref to 

public class PrefabPool : MonoBehaviour
{
    [SerializeField] Transform prefab;
    [SerializeField] Transform poolContainer;
    [SerializeField] bool initOnStart=true;
    [SerializeField] int initAmount = 10;
    [SerializeField] int maxAmount = 1000;
    [SerializeField] int secondsBeforeReturnToPool = -1; // -1 = don'Transform return to pool;
    [SerializeField] Queue<Transform> queue = new Queue<Transform>();
    [SerializeField] Dictionary<Transform, float> pendingReturn = new Dictionary<Transform, float>();

    //*NOTE: These actions should be assigned by the consumer on Awake(); If not possible, toggle initOnStart off, and manually init after assignment; Otherwise, you end up with objects that haven't been had OnInstantiate called on them.
    public Action<Transform> OnInstantiate;
    public Action<Transform> OnReturnToPool;
    public Action<Transform> OnGet;

    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start() {
        if(initOnStart && prefab!=null) Init();
    }

    public void Init(bool rebuildPool=true) {
        if (poolContainer == null)  { poolContainer = transform; }
        if(rebuildPool) DestroyAllObjectsInPool();
        Populate(initAmount);
    }

    public void DestroyAllObjectsInPool() {
        while(queue.Count>0) {
            Destroy(queue.Dequeue().gameObject);
        }
    }

    public void SetPrefab(Transform _prefab, bool rebuildPool=true) {
        if(queue.Count > 0 ) {
            Debug.LogWarning("Setting prefab when the pool is already populated. You may be ordering something incorrectly somewhere."); //TODO: Yes, there are other easy ways to handle this, such as adding an optional queue wipe upon setting new; But I'm avoid distractions atm for time.
        }
        prefab = _prefab;
        Init(rebuildPool);
    }

    void Populate(int amount)
    {
        if (queue.Count + amount > maxAmount) return;
        for (int i = 0; i < amount; i++)
        {
            Transform obj = GameObject.Instantiate(prefab, poolContainer);
            OnInstantiate?.Invoke(obj);
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public Transform Get(bool setActive=true)
    {
        if (queue.Count == 0) { Populate(1); }
        Transform next = queue.Dequeue();
        if (secondsBeforeReturnToPool > -1) pendingReturn.TryAdd(next, Time.time);
        OnGet?.Invoke(next);
        next.gameObject.SetActive(true);
        return next;
    }

    public void ReturnToPool(Transform obj)
    {
        OnReturnToPool?.Invoke(obj);
        if (obj.gameObject.activeSelf) { obj.gameObject.SetActive(false); }
        if (secondsBeforeReturnToPool > -1) pendingReturn.Remove(obj);
        queue.Enqueue(obj);
    }

    void Update()
    {
        CheckPendingReturns();
    }

    void CheckPendingReturns()
    {
        if (pendingReturn.Count == 0 || secondsBeforeReturnToPool < 0) return;
        float curTime = Time.time;
        List<Transform> willReturn = new List<Transform>();
        foreach (Transform k in pendingReturn.Keys)
        {
            if (curTime - pendingReturn[k] > secondsBeforeReturnToPool)
            {
                willReturn.Add(k);
                // ReturnToPool(k);
                // return; //going to return after first found for optimization; We can grab the next ones on the next update
            }
        }
        foreach (Transform obj in willReturn)
        {
            ReturnToPool(obj);
        }
    }
}
