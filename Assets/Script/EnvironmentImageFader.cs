using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentImageFader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer imageRenderer;
    [SerializeField] private float imageFadeTime;
[Header("Sprite Swap")]
    [SerializeField] private Sprite[] envSprites;
    [SerializeField] private float spriteStayTime;
    [SerializeField] private float spriteSwapTime;

    private CoroutineExcuter envFader;
    private bool swapping = false;
    private float timer = 0;
    private int currentIndex = 0;
    private int spriteIndex = 0;

    private const string BrightnessName = "_ImageBrightness";

    void Start(){
        currentIndex = spriteIndex = 0;
        timer = 0;

        envFader = new CoroutineExcuter(this);
        imageRenderer.color = new Color(1,1,1,0);
        gameObject.SetActive(false);

    }
    void Update(){
        if(!swapping){
            timer += Time.deltaTime;
            if(timer > spriteStayTime){
                timer = 0;
                swapping = true;
                spriteIndex ++;
                if(spriteIndex>=envSprites.Length) spriteIndex = 0;
            }
        }
        else{
            timer += Time.deltaTime;
            imageRenderer.material.SetFloat(BrightnessName, 1-EasingFunc.Easing.CosPulse(timer/spriteSwapTime));
            if(timer > spriteSwapTime*0.5f){
                if(currentIndex!=spriteIndex){
                    imageRenderer.sprite = envSprites[spriteIndex];
                    currentIndex = spriteIndex;
                }
            }
            if(timer >= spriteSwapTime){
                swapping = false;
                timer = 0;
            }
        }
    }
    public void ShowEnviornmentSprites(){
        if(envFader==null) envFader = new CoroutineExcuter(this);
        gameObject.SetActive(true);
        envFader.Excute(coroutineFadeEnvElements(true, imageFadeTime));
    }
    public void ShowEnviornmentSprites(float transition){
        if(envFader==null) envFader = new CoroutineExcuter(this);
        gameObject.SetActive(true);
        envFader.Excute(coroutineFadeEnvElements(true, transition));
    }
    public void HideEnvironmentSprites(){
        if(envFader==null) envFader = new CoroutineExcuter(this);
        envFader.Excute(coroutineFadeEnvElements(false, imageFadeTime));
    }
    IEnumerator coroutineFadeEnvElements(bool isFadeIn, float transition){
        if(isFadeIn) gameObject.SetActive(true);
        Color imageInitColor = imageRenderer.color;
        Color imageTargetColor = isFadeIn?Color.white:new Color(1,1,1,0);

        yield return new WaitForLoop(transition, (t)=>{
            float _t = EasingFunc.Easing.SmoothInOut(t);
            imageRenderer.color = Color.Lerp(imageInitColor, imageTargetColor, _t);
        });
        if(!isFadeIn){
            gameObject.SetActive(false);
        }
    }
}
