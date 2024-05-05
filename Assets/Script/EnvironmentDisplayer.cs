using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDisplayer : MonoBehaviour
{
    [SerializeField] private List<DisplayerGroup> displayerGroups;
[Header("Environment Sphere")]
    [SerializeField] private MeshRenderer environmentSphere;
    [SerializeField] private float fadeTime = 1;

    private string ExposerName = "_Exposure";
    private DisplayerGroup lastGroup;
    private CoroutineExcuter envSphereFader;
    private bool isShowEnv = false;

    void Start(){
        envSphereFader = new CoroutineExcuter(this);
    }
    void OnEnable(){
        EventHandler.E_OnShowEnvironment += ShowEnviornment;
        EventHandler.E_OnShowEnvironment_Custom += ShowEnviornment;
        EventHandler.E_OnHideEnvironment += HideEnvironment;
    }
    void OnDisable(){
        EventHandler.E_OnShowEnvironment -= ShowEnviornment;
        EventHandler.E_OnShowEnvironment_Custom -= ShowEnviornment;
        EventHandler.E_OnHideEnvironment -= HideEnvironment;
    }
    void ShowEnviornment(CONTEXT_ENVIRONMENT env){
        if(!isShowEnv){
            isShowEnv = true;
            envSphereFader.Excute(coroutineFadeEnvSphere(true, fadeTime));
        }
        var group = displayerGroups.Find(x=>x.environment == env);
        if(lastGroup!=null){
            if(lastGroup!=group){
                group.imageFader.ShowEnviornmentSprites();
                lastGroup.imageFader.HideEnvironmentSprites();
                lastGroup = group;
            }
        }
        else{
            group.imageFader.ShowEnviornmentSprites();
            lastGroup = group;
        }
    }
    void ShowEnviornment(CONTEXT_ENVIRONMENT env, float transition){
        if(!isShowEnv){
            isShowEnv = true;
            envSphereFader.Excute(coroutineFadeEnvSphere(true, transition));
        }
        var group = displayerGroups.Find(x=>x.environment == env);
        if(lastGroup!=null){
            if(lastGroup!=group){
                group.imageFader.ShowEnviornmentSprites(transition);
                lastGroup.imageFader.HideEnvironmentSprites();
                lastGroup = group;
            }
        }
        else{
            group.imageFader.ShowEnviornmentSprites(transition);
            lastGroup = group;
        }
    }
    void HideEnvironment(){
        if(isShowEnv){
            isShowEnv = false;
            envSphereFader.Excute(coroutineFadeEnvSphere(false, fadeTime));
            if(lastGroup!=null){
                lastGroup.imageFader.HideEnvironmentSprites();
                lastGroup = null;
            }
        }
    }
    IEnumerator coroutineFadeEnvSphere(bool isFadeIn, float transition){
        float initExpo = environmentSphere.material.GetFloat(ExposerName);
        float targetExpo = isFadeIn?0.2f:1;
        yield return new WaitForLoop(transition, (t)=>{
            environmentSphere.material.SetFloat(ExposerName, Mathf.Lerp(initExpo, targetExpo, EasingFunc.Easing.SmoothInOut(t)));
        });
    }
    [System.Serializable]
    private class DisplayerGroup{
        public CONTEXT_ENVIRONMENT environment;
        public EnvironmentImageFader imageFader;
    }
}
