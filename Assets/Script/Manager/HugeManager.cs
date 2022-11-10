using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeManager : MonoBehaviour
{
    [SerializeField] private Animation hugAnime;
    void Enable(){
        EventHandler.E_OnPiecesHuge += StartPiecesHugSequence;
    }
    void OnDisable(){
        EventHandler.E_OnPiecesHuge -= StartPiecesHugSequence;
    }
    void StartPiecesHugSequence(GameObject huggerPiece, GameObject huggeePiece){
        StartCoroutine(coroutinePiecesHugSeguence(huggerPiece, huggeePiece));
    }
    IEnumerator coroutinePiecesHugSeguence(GameObject huggerPiece, GameObject huggeePiece){
        yield return new WaitForSeconds(1f);
        Destroy(huggerPiece);
        Destroy(huggeePiece);
        hugAnime.Play();
        yield return new WaitForSeconds(hugAnime.clip.length);
    }
}
