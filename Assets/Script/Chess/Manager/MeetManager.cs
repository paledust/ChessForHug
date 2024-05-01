using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetManager : MonoBehaviour
{
    [SerializeField] private GameObject hugGroupPrefab;
    [SerializeField] private HugEventScript_SO hugEventScript_SO;
    void OnEnable(){
        EventHandler.E_OnPiecesHug += StartPiecesHugSequence;
    }
    void OnDisable(){
        EventHandler.E_OnPiecesHug -= StartPiecesHugSequence;
    }
    void StartPiecesHugSequence(GameObject huggerPiece, GameObject huggeePiece, Vector2Int gridPoint){
        int huggerAge = huggerPiece.GetComponent<Piece>().personData.Age;
        int huggeeAge = huggeePiece.GetComponent<Piece>().personData.Age;
        var contex = ChessManager.Instance.GetTileData(gridPoint);

        var hugData = hugEventScript_SO.GetHugData(new HugCondition(){env = contex.environment, moment = contex.moment}, PersonData.GetGeneration(huggerAge), PersonData.GetGeneration(huggeeAge));
        EventHandler.Call_UI_ShowDescrip(hugData.script.text);
        StartCoroutine(coroutinePiecesHugSeguence(huggerPiece, huggeePiece, gridPoint));
    }
    IEnumerator coroutinePiecesHugSeguence(GameObject huggerPiece, GameObject huggeePiece,Vector2Int gridPoint){
        yield return new WaitForSeconds(1f);
        huggerPiece.SetActive(false);
        huggeePiece.SetActive(false);

        Animation hugGroupAnime = GameObject.Instantiate(hugGroupPrefab, Geometry.PointFromGrid(gridPoint), Quaternion.identity).GetComponent<Animation>();
        hugGroupAnime.Play();

        yield return new WaitForSeconds(hugGroupAnime.clip.length);
        EventHandler.Call_OnCapturePiece(huggerPiece.GetComponent<Piece>(), ChessManager.Instance.currentPlayer.side);

        var pieceDataHugger = huggerPiece.GetComponent<Piece>().personData;
        var pieceDataHuggee = huggeePiece.GetComponent<Piece>().personData;
        var context = ChessManager.Instance.GetTileData(gridPoint);
        
        EventHandler.Call_OnBackToChessGame();
        Debug.Log($"Hugger:{pieceDataHugger.Age} and Huggee:{pieceDataHuggee.Age},\nthey hugged At {context.environment} for {context.moment}.");
    }
}
