using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class RumbleManager : Singleton<RumbleManager>
{
    private RumbleCommand currentRumble;
    protected override void Awake(){
        base.Awake();
    }
    protected override void OnDestroy(){
        base.OnDestroy();
        Gamepad.current?.SetMotorSpeeds(0,0);
    }
    void ClearRumbles(){
        if(currentRumble!=null){
            StopRumble(currentRumble);
        }
    }
    public void QueueRumble(RumbleCommand rumble){
        if(currentRumble!=null){
            StopAllCoroutines();
        }
        currentRumble = rumble;
        Debug.Log(currentRumble);
        StartCoroutine(currentRumble.excuteRumble());
    }
    public void StopRumble(RumbleCommand rumble){
        if(currentRumble==rumble){
            StopAllCoroutines();
            Gamepad.current?.SetMotorSpeeds(0,0);
            currentRumble = null;
        }
    }
    public void FadeInRumble(RumbleCommand rumble, float fadeTime){
        if(currentRumble != null){StopAllCoroutines();}
        currentRumble = rumble;
        Debug.Log(currentRumble);
        StartCoroutine(coroutineFadeInRumble(rumble, fadeTime));
    }
    public void FadeOutRumble(RumbleCommand rumble, float fadeTime){
        if(currentRumble==rumble){
            StopAllCoroutines();
            StartCoroutine(coroutineFadeOutRumble(rumble, fadeTime));
            currentRumble = null;
        }
    }
    IEnumerator coroutineFadeInRumble(RumbleCommand rumble, float fadeTime){
        Debug.Assert(fadeTime!=0,"The Rumble fadeTime is Zero, Please change to any positive time value or use QueueRumble Instead");

        float lowfreq, highfreq, _t;
        lowfreq = highfreq = _t = 0;
        for(float t=0; t<1; t+=Time.deltaTime/fadeTime){
            _t = EasingFunc.Easing.SmoothInOut(t);
            lowfreq  = Mathf.Lerp(0, rumble.rumbleLowFreq, t);
            highfreq = Mathf.Lerp(0, rumble.rumbleHighFreq, t);
            Gamepad.current?.SetMotorSpeeds(lowfreq, highfreq);
            yield return null;
        }

        yield return currentRumble.excuteRumble();
    }
    IEnumerator coroutineFadeOutRumble(RumbleCommand rumble, float fadeTime){
        Debug.Assert(fadeTime!=0,"The Rumble fadeTime is Zero, Please change to any positive time value or use StopRumble Instead");

        float lowfreq, highfreq, _t = 0;
        lowfreq  = rumble.rumbleLowFreq;
        highfreq = rumble.rumbleHighFreq;
        for(float t=0; t<1; t+=Time.deltaTime/fadeTime){
            _t = EasingFunc.Easing.SmoothInOut(t);
            lowfreq  = Mathf.Lerp(rumble.rumbleLowFreq, 0, t);
            highfreq = Mathf.Lerp(rumble.rumbleHighFreq, 0, t);
            Gamepad.current?.SetMotorSpeeds(lowfreq, highfreq);
            yield return null;
        }
        Gamepad.current?.SetMotorSpeeds(0, 0);
    }
}
