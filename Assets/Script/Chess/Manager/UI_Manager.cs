using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
[Header("Line")]
    [SerializeField] private RectTransform dataDisplayerGroup;
    [SerializeField] private Button Back_Button;
    [SerializeField] private GameObject dataDisplayerPrefab;
    private Dictionary<Transform, UI_DataDisplayer> dataDisplayer_Dict = new Dictionary<Transform, UI_DataDisplayer>();
    void OnEnable(){
        EventHandler.E_UI_ShowData += ShowData;
        EventHandler.E_UI_ShowNum  += ShowNum;
        EventHandler.E_UI_HideData += HideData;
    }
    void OnDisable(){
        EventHandler.E_UI_ShowData -= ShowData;
        EventHandler.E_UI_ShowNum  -= ShowNum;
        EventHandler.E_UI_HideData -= HideData;
    }
    public void ShowData(string content, float height, Transform root){
        if(!dataDisplayer_Dict.ContainsKey(root)){
            dataDisplayer_Dict[root] = GameObject.Instantiate(dataDisplayerPrefab).GetComponent<UI_DataDisplayer>();
            dataDisplayer_Dict[root].GetComponent<RectTransform>().SetParent(dataDisplayerGroup);
        }
        dataDisplayer_Dict[root].ShowData(content, root, height);
    }
    public void ShowNum(int num, float height, Transform root){
        if(!dataDisplayer_Dict.ContainsKey(root)){
            dataDisplayer_Dict[root] = GameObject.Instantiate(dataDisplayerPrefab).GetComponent<UI_DataDisplayer>();
            dataDisplayer_Dict[root].GetComponent<RectTransform>().SetParent(dataDisplayerGroup);
        }
        dataDisplayer_Dict[root].ShowData(num, root, height);
    }
    public void HideData(Transform root){
        if(dataDisplayer_Dict.ContainsKey(root)){
            dataDisplayer_Dict[root].HideData();
        }
    }
    public void FadeOutBackButton()=>StartCoroutine(coroutineSwitchBackButton(false));
    public void FadeInBackButton()=>StartCoroutine(coroutineSwitchBackButton(true));
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
