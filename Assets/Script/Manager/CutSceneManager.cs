using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class CutSceneManager : MonoBehaviour
{
[Header("Camera Zoom Group")]
    [SerializeField] private CinemachineVirtualCamera BoardCam;
    [SerializeField] private CinemachineTargetGroup VC_target;
    [SerializeField] private float targetBlendSpeed = 1;
[Header("Transition timeline")]
    [SerializeField] private PlayableDirector zoomInDirector;
    [SerializeField] private PlayableDirector zoomOutDirector; 
    [SerializeField] private PlayableDirector backFromStoryDirector;
[Header("Story")]
    [SerializeField] private GameObject smallStory;
    [SerializeField] private GameObject bigStory;
    void Awake(){
        EventHandler.E_OnCapturePiece  += StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly += StartSmallCutSceneSequence;
    }
    void OnDestroy(){
        EventHandler.E_OnCapturePiece  -= StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly -= StartSmallCutSceneSequence;
    }
    public void GoBackFromStory(){
    //Reverse the camera fov
        BoardCam.m_Lens.FieldOfView = 25;
    //Play timeline
        backFromStoryDirector.Play();
        VC_target.m_Targets[0].weight = 1;
        VC_target.m_Targets[1].weight = 0;
    //Deactive Story
        bigStory.SetActive(false);
        smallStory.SetActive(false);
    }
    void StartBigCutSceneSequence(Piece piece){
        VC_target.m_Targets[1].target = piece.transform;
        zoomOutDirector.Play();
        bigStory.SetActive(true);
        StartCoroutine(coroutineCameraTargetTransition());
    }
    void StartSmallCutSceneSequence(Piece piece){
        VC_target.m_Targets[1].target = piece.transform;
        zoomInDirector.Play();
        smallStory.SetActive(true);
        StartCoroutine(coroutineCameraTargetTransition());
    }
    IEnumerator coroutineCameraTargetTransition(){
        for(float t=0; t<1; t+=Time.deltaTime*targetBlendSpeed){
            VC_target.m_Targets[0].weight = Mathf.Lerp(1, 0, EasingFunc.Easing.SmoothInOut(t));
            VC_target.m_Targets[1].weight = Mathf.Lerp(0, 1, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        VC_target.m_Targets[1].weight = 1;
    }
}
