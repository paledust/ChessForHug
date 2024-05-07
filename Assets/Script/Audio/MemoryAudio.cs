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
    [SerializeField] private string defaultAmb;
    [SerializeField] private List<EnvironmentAmbience> environmentAmbiences;
    void OnEnable(){
        EventHandler.E_OnShowEnvironment += PlayAmbience;
        EventHandler.E_OnShowEnvironment_Custom += PlayAmbience;
        EventHandler.E_OnHideEnvironment += FadeAmbience;
    }
    void Start(){
        AudioManager.Instance.PlayAmbience(defaultAmb, true, 1f);
    }
    void OnDisable(){
        EventHandler.E_OnShowEnvironment -= PlayAmbience;
        EventHandler.E_OnShowEnvironment_Custom -= PlayAmbience;
        EventHandler.E_OnHideEnvironment -= FadeAmbience;
    }
    void PlayAmbience(CONTEXT_ENVIRONMENT env){
        string clip = environmentAmbiences.Find(x=>x.environment == env).amb;
        AudioManager.Instance.PlayAmbience(clip, true, 1);
    }
    void PlayAmbience(CONTEXT_ENVIRONMENT env, float transition){
        string clip = environmentAmbiences.Find(x=>x.environment == env).amb;
        AudioManager.Instance.PlayAmbience(clip, true, 1);
    }
    void FadeAmbience(){
        AudioManager.Instance.PlayAmbience(defaultAmb, true, 1f);
    }
}
