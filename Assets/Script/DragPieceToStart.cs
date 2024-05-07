using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using SimpleAudioSystem;
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
[Header("Audio")]
    [SerializeField] private float maxVolome = 0.5f;
    [SerializeField] private float offsetThreashold = 1f;
    [SerializeField] private AudioSource sfx_loop;
    [SerializeField] private string sfx_final;
    [SerializeField] private string sfx_woosh;
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
    private GENERATION currentGen;
    private MeshFilter meshFilter;
    private Vector3 initPos;
    private Camera mainCam;
    private Ray mouseRay;
    private RaycastHit hit;
    private CoroutineExcuter ageFader;
    private CoroutineExcuter preHugFader;

    void Start(){
        currentGen = GENERATION.BABY;
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
                    //On Holding
                        if(Mouse.current.leftButton.isPressed){
                            isGrabbing = true;
                            sfx_loop.Play();
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
                {
                    float offset = Mathf.Abs(pos.x - transform.position.x);
                    if(offset>offsetThreashold) sfx_loop.volume = Mathf.Lerp(sfx_loop.volume, maxVolome, Time.deltaTime * 5);
                    else sfx_loop.volume = Mathf.Lerp(sfx_loop.volume, 0, Time.deltaTime * 5);
                }
            //On Release Holding
                if(!Mouse.current.leftButton.isPressed){
                    isGrabbing = false;
                    sfx_loop.volume = 0;
                    sfx_loop.Stop();
                    if(hugRevealed){
                        hugGroup.SetActive(true);
                        AudioManager.Instance.PlaySoundEffectDefault(sfx_final, 1);
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
            if(currentGen!=PersonData.GetGeneration(age)){
                currentGen = PersonData.GetGeneration(age);
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
            }

            if(age >= 99){
                if(!hugRevealed){
                    hugRevealed = true;
                    AudioManager.Instance.PlaySoundEffectDefault(sfx_woosh, 1);

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