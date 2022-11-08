using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleCommand
{
    public bool loopRumble = false;
    public float rumbleCycle = 1f;
    public float rumblePause = 1f;
    public float rumbleLowFreq = 0.25f;
    public float rumbleHighFreq= 0.75f;
    public static RumbleCommand ShortSmallRumble{get;} = new RumbleCommand(false, 0.2f, 0.0f, 0.25f, 0f);
    public static RumbleCommand ShortStrongRumble{get;} = new RumbleCommand(false, 0.2f, 0.0f, 0.25f, 0.75f);
    public static RumbleCommand LongStrongRumble{get;} = new RumbleCommand(false, .75f, 0f, 0.75f, 0.75f);
    public static RumbleCommand CycleSmallPulsingRumble{get;} = new RumbleCommand(true, 0.25f, 1f, 0.25f, 0.005f);
    public RumbleCommand(bool loop = false, float cycle = 0.2f, float pause = 0.1f, float lowfreq = 0.25f, float highfreq = 0.25f){
        this.loopRumble      = loop;
        this.rumbleCycle     = cycle;
        this.rumblePause     = pause;
        this.rumbleLowFreq   = lowfreq;
        this.rumbleHighFreq  = highfreq;
    }
    public IEnumerator excuteRumble(){
        do{
            Gamepad.current?.SetMotorSpeeds(rumbleLowFreq, rumbleHighFreq);
            yield return new WaitForSeconds(rumbleCycle);
            
            if(rumblePause!=0 || !loopRumble){
                Gamepad.current?.SetMotorSpeeds(0, 0);
                yield return new WaitForSeconds(rumblePause);
            }
        }while(loopRumble);
        
        yield return null;
    }
}