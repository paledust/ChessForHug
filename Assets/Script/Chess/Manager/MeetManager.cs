using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetManager : MonoBehaviour
{
    [SerializeField] private GameObject hugGroupPrefab;
    void OnEnable(){
        EventHandler.E_OnPiecesHug += StartPiecesHugSequence;
    }
    void OnDisable(){
        EventHandler.E_OnPiecesHug -= StartPiecesHugSequence;
    }
    void StartPiecesHugSequence(GameObject huggerPiece, GameObject huggeePiece, Vector2Int gridPoint){
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

        Debug.Log($"Hugger:{pieceDataHugger.Age} and Huggee:{pieceDataHuggee.Age} are {context.relationship},\nthey hugged At {context.environment} for {context.moment}.");
    }
}
