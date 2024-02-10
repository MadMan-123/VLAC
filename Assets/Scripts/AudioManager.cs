using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Audio
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private static Dictionary<string, Audio> audioClips = new Dictionary<string, Audio>();
    
    public List<Audio> AudioList = new List<Audio>();
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
            //add the audio clips to the dictionary
            for (int i = 0; i < AudioList.Count; i++)
            {
                audioClips.Add(AudioList[i].name, AudioList[i]);
            }
    }

    public static void PlayAudioClip(string name,Vector3 position, float volume)
    {
        if (audioClips.ContainsKey(name))
        {
            AudioSource.PlayClipAtPoint(audioClips[name].clip, position, volume);
        }
    }


}
