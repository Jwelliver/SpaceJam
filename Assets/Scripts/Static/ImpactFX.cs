using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFX : MonoBehaviour
{

    [SerializeField] ImpactAudioClipStore impactAudioClipStore;
    [SerializeField] PrefabPool prefabPool;
    public static ImpactFX Instance;
    ImpactFX()
    {
        ImpactFX.Instance = this;
    }

    public static void CreateImpact(Transform sender, Collision col)
    {
        AudioClip impactAudio = Instance.impactAudioClipStore.GetImpactAudioClip(sender.tag, col.transform.tag);
        Vector3 pos = col.contacts[0].point;
        //TODO: optionally handle volume based on impact force;
        Transform obj = Instance.prefabPool.Get();
        obj.transform.position = pos;
        obj.GetComponent<AudioSource>().PlayOneShot(impactAudio);
    }
}
