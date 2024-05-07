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
    [SerializeField] private CanvasGroup yearGroup;
    [SerializeField] private TextMeshProUGUI yearUp;
    [SerializeField] private TextMeshProUGUI yearDown;
[Header("Description")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TypewriterByCharacter descriptionWriter;
    [SerializeField] private Button back;

[Header("Restart")]
    [SerializeField] private Button restartButton;
    private Dictionary<Transform, UI_DataDisplayer> dataDisplayer_Dict = new Dictionary<Transform, UI_DataDisplayer>();
    private CoroutineExcuter buttonFader;
    private CoroutineExcuter textFader;

    void OnEnable(){
        EventHandler.E_UI_ShowData += ShowData;
        EventHandler.E_UI_ShowNum  += ShowNum;
        EventHandler.E_UI_HideData += HideData;
        EventHandler.E_UI_CleanDisplayer += CleanDataDisplayer;
        EventHandler.E_UI_ShowDescrip += ShowDescription;
        EventHandler.E_UI_StepYear += StepYear;
        EventHandler.E_OnGameEnd += ShowRestartButton;
        EventHandler.E_UI_ShowYear += ShowYear;
        EventHandler.E_BeforeUnloadScene += CleanUp;
    }
    void Start(){
        back.interactable = false;
        back.image.color = new Color(1,1,1,0);
        textFader = new CoroutineExcuter(this);
        buttonFader = new CoroutineExcuter(this);
    }
    void OnDisable(){
        EventHandler.E_UI_ShowData -= ShowData;
        EventHandler.E_UI_ShowNum  -= ShowNum;
        EventHandler.E_UI_HideData -= HideData;
        EventHandler.E_UI_CleanDisplayer -= CleanDataDisplayer;
        EventHandler.E_UI_ShowDescrip -= ShowDescription;
        EventHandler.E_UI_StepYear -= StepYear;
        EventHandler.E_OnGameEnd -= ShowRestartButton;
        EventHandler.E_UI_ShowYear -= ShowYear;
        EventHandler.E_BeforeUnloadScene -= CleanUp;
    }
//Unity Event
    public void OnBackButtonClick(){
        buttonFader.Excute(coroutineHideButton(0.1f));
        EventHandler.Call_UI_ShowDescrip(string.Empty);
        EventHandler.Call_OnBackToChessGame();
        EventHandler.Call_OnHideEnvironment();
    }
    void ShowYear()=>StartCoroutine(new WaitForLoop(1, (t)=>yearGroup.alpha = Mathf.Lerp(0, 1, EasingFunc.Easing.SmoothInOut(t))));
    void OnTextShowed(){
        descriptionWriter.onTextShowed.RemoveListener(OnTextShowed);
        buttonFader.Excute(coroutineShowButton(0.1f));
    }
    void ShowRestartButton(END_CONDITON endCondition){
        StartCoroutine(coroutineRestartButton(1));
    }
    public void StepYear(float step){
        StartCoroutine(coroutineStepYear(step));
    }
    public void ShowDescription(string content, bool isFocus){
        if(content==string.Empty){
            textFader.Excute(coroutineFadeText(0, 0.2f));
            descriptionWriter.StopShowingText();
        }
        else{
            textFader.Excute(coroutineFadeText(1, 0.2f));
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
    void CleanUp(){
        foreach(var displayer in dataDisplayer_Dict){
            Destroy(displayer.Value);
        }
        back.interactable = false;
        back.image.color = new Color(1,1,1,0);

        yearUp.text = "1990";
        yearDown.text = "1995";
        yearRoot.localPosition = Vector3.zero;

        dataDisplayer_Dict.Clear();
        descriptionWriter.ShowText(string.Empty);
        StartCoroutine(coroutineRestartButton(0));
    }
    IEnumerator coroutineRestartButton(float alpha){
        Color initCol = restartButton.targetGraphic.color;
        Color targetCol = initCol;
        targetCol.a = alpha;

        restartButton.gameObject.SetActive(true);
        yield return new WaitForLoop(0.2f, (t)=>{
            restartButton.targetGraphic.color = Color.Lerp(initCol, targetCol, EasingFunc.Easing.SmoothInOut(t));
        });
        if(alpha == 0) restartButton.gameObject.SetActive(false);
    }
    IEnumerator coroutineStepYear(float step){
        Vector3 initPos = yearRoot.localPosition;
        Vector3 targetPos = initPos + Vector3.up*120*step;
        yield return new WaitForLoop(1, (t)=>{
            yearRoot.localPosition = Vector3.LerpUnclamped(initPos, targetPos, EasingFunc.Easing.BackEaseInOut(t));
        });
        if(targetPos.y >= 120){
            yearUp.text = yearDown.text;
            yearDown.text = (int.Parse(yearDown.text)+ChessManager.Instance.AgeUpAmount).ToString();

            yearRoot.localPosition = Vector3.zero;
        }
    }
    IEnumerator coroutineFadeText(float targetAlpha, float duration){
        Color initColor = descriptionText.color;
        Color targetColor = initColor;
        targetColor.a = targetAlpha;

        yield return new WaitForLoop(duration,(t)=>{
            descriptionText.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.SmoothInOut(t));
        });        
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
