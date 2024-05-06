using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DragPieceToStart : MonoBehaviour
{
    
    [SerializeField] private MeshRenderer[] pieceRenderers;
    [SerializeField] private Texture2D clickTex;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private Mesh[] pieceMeshes;
    [SerializeField] private float dragDistance = 8.5f;
[Header("Pre Meet")]
    [SerializeField] private CinemachineVirtualCamera meetCam;
    [SerializeField] private CanvasGroup ui_canvas;
[Header("Meet")]
    [SerializeField] private GameObject meetObject;
    [SerializeField] private GameObject hugGroup;
    [SerializeField] private GameObject preHugGroup;
[Header("Timeline")]
    [SerializeField] private PlayableDirector director;
    private bool isGrabbing = false;
    private bool isHovering = false;
    private bool hugRevealed = false;
    private MeshFilter meshFilter;
    private Vector3 initPos;
    private Camera mainCam;
    private Ray mouseRay;
    private RaycastHit hit;
    private CoroutineExcuter ageFader;
    private CoroutineExcuter preHugFader;

    void Start(){
        initPos = transform.position;
        isGrabbing = false;
        mainCam = Camera.main;
        ageText.color = new Color(1,1,1,0);
        meshFilter = GetComponent<MeshFilter>();
        ageFader = new CoroutineExcuter(this);
        preHugFader = new CoroutineExcuter(this);
    }
    void Update(){
    //Update Drag Piece
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            if(!isGrabbing){
                mouseRay = mainCam.ScreenPointToRay(mousePos);
                if(Physics.Raycast(mouseRay, out hit, Mathf.Infinity)){
                    if(hit.collider.gameObject == gameObject){
                        if(!isHovering){
                            isHovering = true;
                            ageFader.Excute(coroutineFadeAge(1));
                            Cursor.SetCursor(clickTex, Vector2Int.up*32, CursorMode.Auto);
                        }
                        if(Mouse.current.leftButton.isPressed){
                            isGrabbing = true;
                            Cursor.SetCursor(null, Vector2Int.zero, CursorMode.Auto);
                        }
                    }
                    else{
                        if(isHovering){
                            isHovering = false;
                            ageFader.Excute(coroutineFadeAge(0));
                            Cursor.SetCursor(null, Vector2Int.zero, CursorMode.Auto);
                        }
                    }
                }
                else{
                    if(isHovering){
                        isHovering = false;
                        ageFader.Excute(coroutineFadeAge(0));
                        Cursor.SetCursor(null, Vector2Int.zero, CursorMode.Auto);
                    }
                }
            }
            else{
                Vector3 pos = transform.position;
                pos.x = mainCam.ScreenToWorldPoint(mousePos).x;
                pos.x = Mathf.Clamp(pos.x, initPos.x, initPos.x + dragDistance);
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime*10);
                if(!Mouse.current.leftButton.isPressed){
                    isGrabbing = false;
                    if(hugRevealed){
                        hugGroup.SetActive(true);
                        preHugGroup.SetActive(false);
                        this.enabled = false;
                        StartCoroutine(coroutineNextLevel());
                    }
                }
            }
        }
    //Update UI Age Info
        {
            float ratio = (transform.position.x - initPos.x)/dragDistance;
            int age = Mathf.CeilToInt(ratio * 100);
            ageText.text = age + "<size=25>岁";

            switch(PersonData.GetGeneration(age)){
                case GENERATION.BABY:
                    meshFilter.mesh = pieceMeshes[0];
                    break;
                case GENERATION.YOUTH:
                    meshFilter.mesh = pieceMeshes[1];
                    break;
                case GENERATION.TEEN:
                    meshFilter.mesh = pieceMeshes[2];
                    break;
                case GENERATION.GROWN:
                    meshFilter.mesh = pieceMeshes[3];
                    break;
                case GENERATION.MIDDLE:
                    meshFilter.mesh = pieceMeshes[4];
                    break;
                case GENERATION.OLD:
                    meshFilter.mesh = pieceMeshes[5];
                    break;
            }

            if(age >= 99){
                if(!hugRevealed){
                    hugRevealed = true;
                    preHugFader.Excute(coroutineFadeEffect(1));
                    meetObject.SetActive(true);
                    for(int i=0; i<pieceRenderers.Length; i++){
                        pieceRenderers[i].enabled = false;
                    }
                }
            }
            else{
                if(hugRevealed){
                    hugRevealed = false;
                    preHugFader.Excute(coroutineFadeEffect(0));
                    meetObject.SetActive(false);
                    for(int i=0; i<pieceRenderers.Length; i++){
                        pieceRenderers[i].enabled = true;
                    }                
                }
            }
        }
    }
    IEnumerator coroutineFadeAge(float targetAlpha){
        Color initCol = ageText.color;
        Color targetCol = new Color(1,1,1,targetAlpha);
        yield return new WaitForLoop(0.2f, (t)=>
            ageText.color = Color.Lerp(initCol, targetCol, EasingFunc.Easing.SmoothInOut(t))
        );
    }
    IEnumerator coroutineFadeEffect(float scale){
        if(scale == 0)
            meetCam.enabled = false;
        else
            meetCam.enabled = true;
        
        float initAlpha = ui_canvas.alpha;
        yield return new WaitForLoop(0.2f, (t)=>
            ui_canvas.alpha = Mathf.Lerp(initAlpha, 1-scale, EasingFunc.Easing.SmoothInOut(t))
        );
    }
    IEnumerator coroutineNextLevel(){
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        GameManager.Instance.SwitchingScene("Chess", false);
    }
}