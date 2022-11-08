using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem{
    [CreateAssetMenu(fileName = "AudioInfo_SO", menuName = "AudioSystem/AudioInfo_SO")]
    public class AudioInfo_SO : ScriptableObject
    {
        public List<AudioInfo> audio_info_list;
        public AudioClip GetAudioClipByName(string audio_name){
            return audio_info_list.Find(x=>x.audio_name == audio_name).audio_clip;
        }
    }
    [System.Serializable]
    public struct AudioInfo{
        public string audio_name;
        public AudioClip audio_clip;
    }
}
