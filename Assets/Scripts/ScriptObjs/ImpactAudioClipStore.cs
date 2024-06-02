using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObjs/ImpactAudioClipStore")]
public class ImpactAudioClipStore : ScriptableObject
{

    [SerializeField] AudioClip defaultCatchAllClip;
    [SerializeField] List<Interactions<AudioClip>> interactionsRef = new List<Interactions<AudioClip>>();
    // {tag:{otherTag, interactionAudioClip}} // e.g. interactions[asteroid][asteroid] or interactions[ship][asteroid]
    Dictionary<string, Dictionary<string, AudioClip>> interactions = new Dictionary<string, Dictionary<string, AudioClip>>();

    void OnEnable()
    {
        //populate interactions dict using interactionsRef
        interactions = new Dictionary<string, Dictionary<string, AudioClip>>();
        foreach (Interactions<AudioClip> i in interactionsRef)
        {
            if (!interactions.ContainsKey(i.tag)) { interactions.Add(i.tag, i.GetAsDict()); }
        }
    }

    public AudioClip GetImpactAudioClip(string tag, string otherTag)
    {
        // If we have primary tag;
        if (interactions.ContainsKey(tag))
        {   // check if we have a defined interaction with otherTag
            if (interactions[tag].ContainsKey(otherTag)) { return interactions[tag][otherTag]; }
            // if not, try to find a default clip
            else if (interactions[tag].ContainsKey("default")) { return interactions[tag]["default"]; }
        }

        // If we haven't found something, try reversing tags
        if (interactions.ContainsKey(otherTag))
        {
            if (interactions[otherTag].ContainsKey(tag)) { return interactions[otherTag][tag]; }
            else if (interactions[otherTag].ContainsKey("default")) { return interactions[otherTag]["default"]; }
        }

        // If nothing found, use catchAll clip
        return defaultCatchAllClip;
    }
}

[Serializable]
class Interaction<T>
{
    public string otherTag;
    public T value;
}

[Serializable]
class Interactions<T>
{
    public string tag;
    public T defaultValue;
    public List<Interaction<T>> interactions = new List<Interaction<T>>();

    public Dictionary<string, T> GetAsDict()
    {
        Dictionary<string, T> dict = new Dictionary<string, T>();
        foreach (Interaction<T> interaction in interactions)
        {
            dict.Add(interaction.otherTag, interaction.value);
        }
        return dict;
    }
}





//========== this version attempts to use straight dictionary; but forgot dict is not serializable
// [CreateAssetMenu(menuName = "ScriptObjs/ImpactAudioClipStore")]
// public class ImpactAudioClipStore : ScriptableObject
// {
//     // {tag:{otherTag, interactionAudioClip}} // e.g. interactions[asteroid][asteroid] or interactions[ship][asteroid]
//     [SerializeField] AudioClip defaultCatchAllClip;
//     [SerializeField] Dictionary<string, Dictionary<string, AudioClip>> interactions = new Dictionary<string, Dictionary<string, AudioClip>>();


//     public AudioClip GetImpactAudioClip(string tag, string otherTag)
//     {

//         // If we have primary tag;
//         if (interactions.ContainsKey(tag))
//         {   // check if we have a defined interaction with otherTag
//             if (interactions[tag].ContainsKey(otherTag)) { return interactions[tag][otherTag]; }
//             // if not, try to find a default clip
//             else if (interactions[tag].ContainsKey("default")) { return interactions[tag]["default"]; }
//         }

//         // If we haven't found something, try reversing tags
//         if (interactions.ContainsKey(otherTag))
//         {
//             if (interactions[otherTag].ContainsKey(tag)) { return interactions[otherTag][tag]; }
//             else if (interactions[otherTag].ContainsKey("default")) { return interactions[otherTag]["default"]; }
//         }

//         // If nothing found, use catchAll clip
//         return defaultCatchAllClip;
//     }
// }
