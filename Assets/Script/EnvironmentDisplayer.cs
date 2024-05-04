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
    public void ShowEnviornment(CONTEXT_ENVIRONMENT env){
        if(!isShowEnv){
            isShowEnv = true;
            envSphereFader.Excute(coroutineFadeEnvSphere(true));
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
    public void HideEnvironment(){
        if(isShowEnv){
            isShowEnv = false;
            envSphereFader.Excute(coroutineFadeEnvSphere(false));
            if(lastGroup!=null){
                lastGroup.imageFader.HideEnvironmentSprites();
                lastGroup = null;
            }
        }
    }
    IEnumerator coroutineFadeEnvSphere(bool isFadeIn){
        float initExpo = environmentSphere.material.GetFloat(ExposerName);
        float targetExpo = isFadeIn?0.2f:1;
        yield return new WaitForLoop(fadeTime, (t)=>{
            environmentSphere.material.SetFloat(ExposerName, Mathf.Lerp(initExpo, targetExpo, EasingFunc.Easing.SmoothInOut(t)));
        });
    }
    [System.Serializable]
    private class DisplayerGroup{
        public CONTEXT_ENVIRONMENT environment;
        public EnvironmentImageFader imageFader;
    }
}
