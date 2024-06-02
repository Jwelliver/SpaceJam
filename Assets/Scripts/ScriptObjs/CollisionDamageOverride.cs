using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObjs/CollisionDamageOverride")]
public class CollisionDamageOverride : ScriptableObject
{
    [SerializeField] List<KVPair<string, float>> damageByTagList = new List<KVPair<string, float>>(); // for inspector
    static Dictionary<string, float> damageByTag = new Dictionary<string, float>(); // this will be used during runtime

    static CollisionDamageOverride Instance;

    CollisionDamageOverride()
    {
        CollisionDamageOverride.Instance = this;
    }

    void OnEnable()
    {
        foreach (KVPair<string, float> kv in damageByTagList)
        {
            damageByTag.Add(kv.key, kv.value);
        }
    }

    void OnDisable()
    {
        Instance = null;
    }

    public static float CheckDamageOverrideByTag(string tag)
    {
        if (damageByTag.ContainsKey(tag))
        {
            return damageByTag[tag];
        }
        return -1;
    }

}

[Serializable]
class KVPair<TKey, TValue>
{
    public TKey key;
    public TValue value;
}