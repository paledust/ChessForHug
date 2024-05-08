using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class ChessAudio : MonoBehaviour
{
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private string pickClips;
    [SerializeField] private string putClips;
    [SerializeField] private string moveClips;
    public void PlayMovePiece()=>AudioManager.Instance.PlaySoundEffect(m_audio, moveClips, 1);
    public void PlayPickPiece()=>AudioManager.Instance.PlaySoundEffect(m_audio, pickClips, 1);
    public void PlayPutPiece()=>AudioManager.Instance.PlaySoundEffect(m_audio, putClips, 1);
}