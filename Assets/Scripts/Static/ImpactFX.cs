using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ImpactFX : MonoBehaviour
{

    // [SerializeField] ImpactAudioClipStore impactAudioClipStore;
    [SerializeField] Transform impactFxPrefab;
    [SerializeField] Transform objPoolParent;

    static ImpactAudioClipStore impactAudioClipStore;
    static ObjectPool<Transform> objectPool;
    public static ImpactFX Instance;
    ImpactFX()
    {
        ImpactFX.Instance = this;
        objectPool = new ObjectPool<Transform>(() => GameObject.Instantiate(Instance.impactFxPrefab, objPoolParent));
    }

    public static void CreateImpact(Transform sender, Collision col)
    {
        if (!impactAudioClipStore)
        {
            try { impactAudioClipStore = Resources.FindObjectsOfTypeAll<ImpactAudioClipStore>()[0]; }
            catch { throw new System.Exception("ImpactAudioClipStore not found."); }
        }
        AudioClip impactAudio = impactAudioClipStore.GetImpactAudioClip(sender.tag, col.transform.tag);
        Vector3 pos = col.contacts[0].point;
        //get impact obj from obj pool (should hold audio and can also hold particlefx for later)
        //position it at point and play one shot;
        //optionally handle volume based on impact force;
        //TODO: Pull these from an objectPool and remove lifespan from impactFxPrefab;
        Transform newTransform = objectPool.Get();
        // Transform newTransform = GameObject.Instantiate(Instance.impactFxPrefab, pos, Quaternion.identity, Instance.objPoolParent);
        newTransform.position = pos;
        newTransform.GetComponent<AudioSource>().PlayOneShot(impactAudio);
    }
}
