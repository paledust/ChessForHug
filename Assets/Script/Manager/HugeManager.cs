using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeManager : MonoBehaviour
{
    void Enable(){
        EventHandler.E_OnPiecesHuge += StartPiecesHugeSequence;
    }
    void OnDisable(){
        EventHandler.E_OnPiecesHuge -= StartPiecesHugeSequence;
    }
    void StartPiecesHugeSequence(GameObject huggerPiece, GameObject huggeePiece){
        
    }
}
