using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    public Transform prefab;
    public Transform poolContainer;
    public int initAmount = 10;
    public int maxAmount = 1000;
    public int secondsBeforeReturnToPool = -1; // -1 = don'Transform return to pool;
    public Queue<Transform> queue = new Queue<Transform>();
    public Dictionary<Transform, float> pendingReturn = new Dictionary<Transform, float>();

    public Action<Transform> OnInstantiate;
    public Action<Transform> OnReturnToPool;
    public Action<Transform> OnGet;

    // Start is called before the first frame update
    void Awake()
    {
        if (poolContainer == null)
        {
            poolContainer = transform;
        }
        Populate(initAmount);

    }

    void Populate(int amount)
    {
        if (queue.Count + amount > maxAmount) return;
        for (int i = 0; i < amount; i++)
        {
            Transform obj = GameObject.Instantiate(prefab, poolContainer).GetComponent<Transform>();
            OnInstantiate?.Invoke(obj);
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public Transform Get()
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
