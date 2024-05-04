using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DataDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform rectTrans;
[Header("Line")]
    [SerializeField] private RectTransform lineTrans;
    [SerializeField] private TextMeshProUGUI dataText;
    [SerializeField] private CanvasGroup canvas;
[Header("Timing")]
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 0.5f;
    private CoroutineExcuter displayer;
    private Transform displayRoot;
    private Camera mainCam;
    void Awake()
    {
        mainCam = Camera.main;
        canvas.alpha = 0;
        lineTrans.sizeDelta = new Vector2(1, 100);
    }
    void Update(){
        Vector3 pos = mainCam.WorldToScreenPoint(displayRoot.position+Vector3.up);
        pos.z = 0;
        rectTrans.position = pos;
    }
    public void ShowData(string content, Transform root, float height){
        Initialize(root);

        dataText.text = content;
        displayer.Excute(coroutineShowContent(height, fadeInTime));
    }
    public void ShowData(int num, Transform root, float height){
        Initialize(root);

        dataText.text = string.Empty;
        displayer.Excute(coroutineShowContent(num, height, fadeInTime));
    }
    public void HideData()=>displayer.Excute(coroutineFadeOut(fadeOutTime));
    void Initialize(Transform root){
        if(displayer==null) displayer = new CoroutineExcuter(this);
        lineTrans.sizeDelta = new Vector2(1, 100);
        mainCam = Camera.main;
        displayRoot = root;        
    }
    IEnumerator coroutineShowContent(float height, float duration){
        float initAlpha = canvas.alpha;
        Vector2 initSize = lineTrans.sizeDelta;
        Vector2 targetSize = new Vector2(1, height);
        
        yield return new WaitForLoop(duration, (t)=>{
            canvas.alpha = Mathf.Lerp(initAlpha, 1, EasingFunc.Easing.SmoothInOut(t*2));
            lineTrans.sizeDelta = Vector2.Lerp(initSize, targetSize, EasingFunc.Easing.SmoothInOut(t));
        });
    }
    IEnumerator coroutineShowContent(int num, float height, float duration){
        float initAlpha = canvas.alpha;
        Vector2 initSize = lineTrans.sizeDelta;
        Vector2 targetSize = new Vector2(1, height);
        
        yield return new WaitForLoop(duration, (t)=>{
            canvas.alpha = Mathf.Lerp(initAlpha, 1, EasingFunc.Easing.SmoothInOut(Mathf.Clamp01(t*2)));
            lineTrans.sizeDelta = Vector2.Lerp(initSize, targetSize, EasingFunc.Easing.SmoothInOut(t));
            dataText.text = Mathf.CeilToInt(Mathf.Lerp(0, num, EasingFunc.Easing.QuadEaseOut(t))+0.5f).ToString()+"<size=28>岁";
        });
    }
    IEnumerator coroutineFadeOut(float duration){
        float initAlpha = canvas.alpha;
        yield return new WaitForLoop(duration,(t)=>
            canvas.alpha = Mathf.Lerp(initAlpha, 0, EasingFunc.Easing.SmoothInOut(t))
        );
    }
}
