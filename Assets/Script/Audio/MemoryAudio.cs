using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class MemoryAudio : MonoBehaviour
{
    [System.Serializable]
    public struct EnvironmentAmbience{
        public CONTEXT_ENVIRONMENT environment;
        public string amb;
    }
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private List<EnvironmentAmbience> environmentAmbiences;
    public void PlayAmbience(CONTEXT_ENVIRONMENT env){
        string clip = environmentAmbiences.Find(x=>x.environment == env).amb;
        AudioManager.Instance.PlayAmbience(clip, true, 1);
    }
}
