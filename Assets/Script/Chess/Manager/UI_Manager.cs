using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
    [SerializeField] private Button Back_Button;
    public void FadeOutBackButton(){
        StartCoroutine(coroutineSwitchBackButton(false));
    }
    public void FadeInBackButton(){
        StartCoroutine(coroutineSwitchBackButton(true));
    }
    IEnumerator coroutineSwitchBackButton(bool isOn){
        if(!isOn) Back_Button.interactable = false;
        Color initColor, targetColor;
        initColor = targetColor = Color.white;
        initColor.a = isOn?0:1;
        targetColor.a = isOn?1:0;

        for(float t=0; t<1; t+=Time.deltaTime*4){
            Back_Button.image.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        Back_Button.image.color = targetColor;
        if(isOn) Back_Button.interactable = true;
    }
}
