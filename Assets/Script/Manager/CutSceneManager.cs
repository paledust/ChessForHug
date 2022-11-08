using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class CutSceneManager : MonoBehaviour
{
[Header("Camera Zoom Group")]
    [SerializeField] private CinemachineTargetGroup VC_target;
    [SerializeField] private float targetBlendSpeed = 1;
[Header("Transition timeline")]
    [SerializeField] private PlayableDirector zoomInDirector;
    [SerializeField] private PlayableDirector zoomOutDirector; 
[Header("Story")]
    [SerializeField] private GameObject smallStory;
    [SerializeField] private GameObject bigStory;
    void OnEnable(){
        EventHandler.E_OnCapturePiece  += StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly += StartSmallCutSceneSequence;
    }
    void OnDisable(){
        EventHandler.E_OnCapturePiece  -= StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly -= StartSmallCutSceneSequence;
    }
    void StartBigCutSceneSequence(Piece piece){
        VC_target.m_Targets[1].target = piece.transform;
        zoomOutDirector.Play();
        bigStory.SetActive(true);
        StartCoroutine(coroutineBigCutSceneSequence());
    }
    void StartSmallCutSceneSequence(Piece piece){
        VC_target.m_Targets[1].target = piece.transform;
        zoomInDirector.Play();
        smallStory.SetActive(true);
        StartCoroutine(coroutineBigCutSceneSequence());
    }
    IEnumerator coroutineBigCutSceneSequence(){
        for(float t=0; t<1; t+=Time.deltaTime*targetBlendSpeed){
            VC_target.m_Targets[0].weight = Mathf.Lerp(1, 0, EasingFunc.Easing.SmoothInOut(t));
            VC_target.m_Targets[1].weight = Mathf.Lerp(0, 1, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        VC_target.m_Targets[1].weight = 1;
    }
}
