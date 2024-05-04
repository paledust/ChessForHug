using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentImageFader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer imageRenderer;
    [SerializeField] private float imageFadeTime;
    [SerializeField] private float imageStayTime;
    [SerializeField] private Sprite[] envSprites;
    private CoroutineExcuter envFader;
    void Start(){
        envFader = new CoroutineExcuter(this);
        imageRenderer.color = new Color(1,1,1,0);
        gameObject.SetActive(false);
    }
    public void ShowEnviornmentSprites(){
        if(envFader==null) envFader = new CoroutineExcuter(this);
        gameObject.SetActive(true);
        envFader.Excute(coroutineFadeEnvElements(true));
    }
    public void HideEnvironmentSprites(){
        if(envFader==null) envFader = new CoroutineExcuter(this);
        envFader.Excute(coroutineFadeEnvElements(false));
    }
    IEnumerator coroutineFadeEnvElements(bool isFadeIn){
        if(isFadeIn) gameObject.SetActive(true);
        Color imageInitColor = imageRenderer.color;
        Color imageTargetColor = isFadeIn?Color.white:new Color(1,1,1,0);

        yield return new WaitForLoop(imageFadeTime, (t)=>{
            float _t = EasingFunc.Easing.SmoothInOut(t);
            imageRenderer.color = Color.Lerp(imageInitColor, imageTargetColor, _t);
        });
        if(!isFadeIn) gameObject.SetActive(false);
    }
}
