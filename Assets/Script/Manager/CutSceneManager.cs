using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    void OnEnable(){
        EventHandler.E_OnCapturePiece  += StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly += StartSmallCutSceneSequence;
    }
    void OnDisable(){
        EventHandler.E_OnCapturePiece  -= StartBigCutSceneSequence;
        EventHandler.E_OnMovePieceOnly -= StartSmallCutSceneSequence;
    }
    void StartBigCutSceneSequence(PIECE_TYPE piece){
        Debug.Log($"Start Sequence with {piece}.");
    }
    void StartSmallCutSceneSequence(){
        Debug.Log("Start Small Sequence.");
    }
}
