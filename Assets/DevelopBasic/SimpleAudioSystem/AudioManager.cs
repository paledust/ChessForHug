using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField]
        private AudioInfo_SO audioInfo;
        [SerializeField]
        private AudioSource ambience_loop;
        [SerializeField]
        private AudioSource music_loop;
        string current_ambience_name = string.Empty;
        string current_music_name = string.Empty;

        protected override void Awake(){
            base.Awake();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        public void PlayMusic(string audio_name){
            current_music_name = audio_name;
            if(audio_name == string.Empty) music_loop.Stop();

            music_loop.clip = audioInfo.GetAudioClipByName(audio_name);
            if(music_loop.clip!=null)
                music_loop.Play();
        }
        public void PlayAmbience(string audio_name){
            current_ambience_name = audio_name;
            if(audio_name == string.Empty) ambience_loop.Stop();

            ambience_loop.clip = audioInfo.GetAudioClipByName(audio_name);
            if(ambience_loop.clip != null)
                ambience_loop.Play();
        }
        //TO DO: Maybe adding a function to do cross fading between two different clips
    }
}
