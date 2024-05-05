using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
[Header("Line")]
    [SerializeField] private RectTransform dataDisplayerGroup;
    [SerializeField] private GameObject dataDisplayerPrefab;

[Header("Years")]
    [SerializeField] private RectTransform yearRoot;
    [SerializeField] private TextMeshProUGUI yearUp;
    [SerializeField] private TextMeshProUGUI yearDown;
[Header("Description")]
    [SerializeField] private TypewriterByCharacter descriptionWriter;
    [SerializeField] private Button back;
    private Dictionary<Transform, UI_DataDisplayer> dataDisplayer_Dict = new Dictionary<Transform, UI_DataDisplayer>();
    private CoroutineExcuter buttonFader;

    void OnEnable(){
        EventHandler.E_UI_ShowData += ShowData;
        EventHandler.E_UI_ShowNum  += ShowNum;
        EventHandler.E_UI_HideData += HideData;
        EventHandler.E_UI_CleanDisplayer += CleanDataDisplayer;
        EventHandler.E_UI_ShowDescrip += ShowDescription;
        EventHandler.E_UI_StepYear += StepYear;
    }
    void Start(){
        back.interactable = false;
        back.image.color = new Color(1,1,1,0);
        buttonFader = new CoroutineExcuter(this);
    }
    void OnDisable(){
        EventHandler.E_UI_ShowData -= ShowData;
        EventHandler.E_UI_ShowNum  -= ShowNum;
        EventHandler.E_UI_HideData -= HideData;
        EventHandler.E_UI_CleanDisplayer -= CleanDataDisplayer;
        EventHandler.E_UI_ShowDescrip -= ShowDescription;
        EventHandler.E_UI_StepYear -= StepYear;
    }
//Unity Event
    public void OnBackButtonClick(){
        buttonFader.Excute(coroutineHideButton(0.1f));
        EventHandler.Call_UI_ShowDescrip(string.Empty);
        EventHandler.Call_OnBackToChessGame();
        EventHandler.Call_OnHideEnvironment();
    }
    void OnTextShowed(){
        descriptionWriter.onTextShowed.RemoveListener(OnTextShowed);
        buttonFader.Excute(coroutineShowButton(0.1f));
    }
    public void StepYear(float step){
        StartCoroutine(coroutineStepYear(step));
    }
    public void ShowDescription(string content, bool isFocus){
        if(content==string.Empty){
            descriptionWriter.StopShowingText();
            descriptionWriter.StartDisappearingText();
        }
        else{
            if(isFocus){
                descriptionWriter.onTextShowed.AddListener(OnTextShowed);
            }
            descriptionWriter.ShowText(content);
        }
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
        if(dataDisplayer_Dict.ContainsKey(root))
            dataDisplayer_Dict[root].HideData();
    }
    public void CleanDataDisplayer(Transform root){
        if(dataDisplayer_Dict.ContainsKey(root)){
            var displayer = dataDisplayer_Dict[root];
            dataDisplayer_Dict.Remove(root);
            Destroy(displayer);
        }        
    }
    IEnumerator coroutineStepYear(float step){
        Vector3 initPos = yearRoot.localPosition;
        Vector3 targetPos = initPos + Vector3.up*120*step;
        yield return new WaitForLoop(1, (t)=>{
            yearRoot.localPosition = Vector3.LerpUnclamped(initPos, targetPos, EasingFunc.Easing.BackEaseInOut(t));
        });
        if(targetPos.y >= 120){
            yearUp.text = yearDown.text;
            yearDown.text = (int.Parse(yearDown.text)+5).ToString();

            yearRoot.localPosition = Vector3.zero;
        }
    }
    IEnumerator coroutineShowButton(float duration){
        Color initColor = Color.white;
        initColor.a = 0;
        yield return new WaitForLoop(duration,(t)=>{
            back.image.color = Color.Lerp(initColor, Color.white, EasingFunc.Easing.SmoothInOut(t));
        });
        back.interactable = true;
    }
    IEnumerator coroutineHideButton(float duration){
        back.interactable = false;     
        Color initColor = Color.white;
        Color targetColor = initColor;
        targetColor.a = 0;
        yield return new WaitForLoop(duration,(t)=>{
            back.image.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.SmoothInOut(t));
        });
    }
}
