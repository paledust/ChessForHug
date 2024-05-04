using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDisplayer : MonoBehaviour
{
[Header("Sprites")]
    [SerializeField] private SpriteRenderer imageRenderer;
    [SerializeField] private SpriteRenderer blackScreen;
    [SerializeField] private float imageFadeTime;
    [SerializeField] private float imageStayTime;
[Header("EnvSprites SO")]
    [SerializeField] private EnvSprites_SO envSprites_SO;
    private Sprite[] envSprites;
    private CoroutineExcuter envFader;
    void Start(){
        envFader = new CoroutineExcuter(this);
        imageRenderer.color = new Color(1,1,1,0);
        blackScreen.color = new Color(0,0,0,0);
        imageRenderer.enabled = false;
        blackScreen.enabled = false;
    }
    public void LoadEnvironmentSprites(CONTEXT_ENVIRONMENT env){
        envSprites = envSprites_SO.GetSpritesByEnvironment(env);
        imageRenderer.sprite = envSprites[0];
    }
    public void HideEnvironmentSprites(){
        envFader.Excute(coroutineFadeEnvElements(false));
    }
    IEnumerator coroutineFadeEnvElements(bool isFadeIn){
        Color imageInitColor = imageRenderer.color;
        Color blackScreenInitColor = blackScreen.color;
        Color imageTargetColor = isFadeIn?Color.white:new Color(1,1,1,0);
        Color blackScreenTargetColor = isFadeIn?Color.black:Color.clear;

        yield return new WaitForLoop(imageFadeTime, (t)=>{
            float _t = EasingFunc.Easing.SmoothInOut(t);
            imageRenderer.color = Color.Lerp(imageInitColor, imageTargetColor, _t);
            blackScreen.color = Color.Lerp(blackScreenInitColor, blackScreenTargetColor, _t);
        });
    }
}
