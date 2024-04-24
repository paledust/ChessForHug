using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[System.Serializable]
public struct VC_ImpulseData{
    public CinemachineVirtualCamera targetVC;
    public AnimationCurve impulseCurve; 
    public float targetAmp;
    public float targetFreq;
    public float duration;
}

public class CinemachineCommand : Command<CinemachineManager>{}
public class CinemaC_Impulse : CinemachineCommand{
    public VC_ImpulseData vc_ImpulseData;

    private CinemachineBasicMultiChannelPerlin vc_noise;
    private float timer;
    private float initAmp;
    private float initFreq;

    public CinemaC_Impulse(VC_ImpulseData vcData){
        vc_ImpulseData = vcData;
        vc_noise = vc_ImpulseData.targetVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        initAmp  = vc_noise.m_AmplitudeGain;
        initFreq = vc_noise.m_FrequencyGain;

        timer = 0;
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        vc_noise.m_AmplitudeGain = initAmp;
        vc_noise.m_FrequencyGain = initFreq;
    }
    protected override void Init()
    {
        base.Init();
        if(vc_ImpulseData.duration == 0){
            SetStatus(CommandStatus.Success);
            return;
        }
    }
    internal override void CommandUpdate(CinemachineManager context)
    {
        base.CommandUpdate(context);
        timer += Time.deltaTime/vc_ImpulseData.duration;
        if(timer >= 1){
            timer = 1;
            UpdateVCNoise(timer);
            SetStatus(CommandStatus.Success);
            return;
        }
        UpdateVCNoise(timer);
    }
    void UpdateVCNoise(float t){
        if(vc_ImpulseData.impulseCurve!=null){
            vc_noise.m_AmplitudeGain = Mathf.Lerp(initAmp, vc_ImpulseData.targetAmp, vc_ImpulseData.impulseCurve.Evaluate(t));
            vc_noise.m_FrequencyGain = Mathf.Lerp(initFreq, vc_ImpulseData.targetFreq, vc_ImpulseData.impulseCurve.Evaluate(t));
        }
    }
}