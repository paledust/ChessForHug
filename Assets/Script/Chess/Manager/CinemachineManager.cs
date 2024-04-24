using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum VC_Blend_Style{Cut, Black_Fade, Ease}
public class CinemachineManager : Singleton<CinemachineManager>
{
    public CinemachineCommandManager vc_commandManager;

    private CinemachineBrain _camBrain;
    public const float DEFAULT_CAMERA_TRANSITION_TIME = 0.5f;

    public CinemachineBrain camBrain{get{
        if(_camBrain == null) FindCamBrain();
        return _camBrain;
    }}

    protected override void Awake(){
        base.Awake();
        EventHandler.E_AfterLoadScene += FindCamBrain;

    #if UNITY_EDITOR
        FindCamBrain();
    #endif
    }
    protected override void OnDestroy(){
        base.OnDestroy();
        EventHandler.E_AfterLoadScene -= FindCamBrain;
    }
    void FindCamBrain(){
        _camBrain = FindObjectOfType<CinemachineBrain>();
    }
    public void AlignCameraToPosition(CinemachineVirtualCamera targetVC, Vector3 position, bool WithOffset = true){
        if(targetVC.GetCinemachineComponent<CinemachineTransposer>()!=null)
            targetVC.transform.position = position+(WithOffset?Vector3.zero:targetVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset);
    }
    public void TransitionToCamera(CinemachineVirtualCamera targetVC, bool isToVC, VC_Blend_Style blendStyle, float easeTime = DEFAULT_CAMERA_TRANSITION_TIME){
        switch(blendStyle){
            case VC_Blend_Style.Cut:
                camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                if(camBrain.ActiveVirtualCamera!=null && canModifyCamera(camBrain.ActiveVirtualCamera)) camBrain.ActiveVirtualCamera.Priority = 10;
                targetVC.Priority = isToVC?11:10;
                if(isToVC) targetVC.enabled = true;
                else targetVC.enabled = false;
                break;
            case VC_Blend_Style.Black_Fade:
                StartCoroutine(CoroutineCamTransition_BlackFade(targetVC, isToVC, easeTime));
                break;
            case VC_Blend_Style.Ease:
                camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                camBrain.m_DefaultBlend.m_Time  = easeTime;
                if(canModifyCamera(camBrain.ActiveVirtualCamera)) camBrain.ActiveVirtualCamera.Priority = 10;
                targetVC.Priority = isToVC?11:10;
                if(isToVC) targetVC.enabled = true;
                else targetVC.enabled = false;
                break;
        }
    }
    bool canModifyCamera(ICinemachineCamera vc){
        if(vc==null){
            Debug.LogWarning("No Active Virtual Camera is founded!");
            return false;
        }
        return vc.VirtualCameraGameObject.tag != Service.debugTag;
    }
    IEnumerator CoroutineCamTransition_BlackFade(CinemachineVirtualCamera vc, bool isToVC, float duration){
        yield return GameManager.Instance.FadeInBlackScreen(duration);
        camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        if(canModifyCamera(camBrain.ActiveVirtualCamera)) camBrain.ActiveVirtualCamera.Priority = 10;
        
        vc.Priority = isToVC?11:10;
        if(isToVC) vc.enabled = true;
        else vc.enabled = false;
        yield return GameManager.Instance.FadeOutBlackScreen(duration);
    }
}
