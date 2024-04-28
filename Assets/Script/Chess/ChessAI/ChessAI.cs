using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ChessAI : MonoBehaviour
{
    [SerializeField] private float moveDelay = 1f;   
    private List<Moves> moves = new List<Moves>();
    public async Task MakeMove(){
        GetMoves();
        await Task.Delay(Mathf.FloorToInt(moveDelay*1000));
        MakeHighestScoreMove();
    }
    void MakeRandomMove(){
        var rnd = Random.Range(0, moves.Count);
        ChessManager.Instance.MakeMoves(ChessManager.Instance.PieceAtGrid(moves[rnd].from), moves[rnd].to);
    }
//This will evaluate the board and check which move has the best score
    void MakeHighestScoreMove(){
    //Shuffle the moves, so the same move isn't always picked on ties
        Moves[] possibleMoves = moves.ToArray();
        Service.Shuffle<Moves>(ref possibleMoves);

        if(possibleMoves.Length == 0) return;

        Moves bestMove  = null;
        float bestScore = Mathf.NegativeInfinity;
        for(int i=0; i<possibleMoves.Length; i++){
            ChessManager.Instance.MakeMoves(possibleMoves[i]);
            float score = ChessManager.Instance.EvaluateBoard(ChessManager.Instance.currentPlayer);
            if(score > bestScore){
                bestScore = score;
                bestMove = possibleMoves[i];
            }
            ChessManager.Instance.UndoMoves();
        }
        ChessManager.Instance.MakeMoves(ChessManager.Instance.PieceAtGrid(bestMove.from), bestMove.to);
    }
    void GetMoves(){
        moves.Clear();

        var pieces = ChessManager.Instance.currentPlayer.pieces;
        for(int i=0; i<pieces.Count; i++){
            Piece piece = pieces[i].GetComponent<Piece>();
            Vector2Int from = ChessManager.Instance.GridForPiece(pieces[i]);
            var locations = pieces[i].GetComponent<Piece>().MoveLocations(from);
            ChessManager.Instance.CheckLegalMove(ref locations);

            for(int j=0; j<locations.Count; j++){
                moves.Add(new Moves(from, locations[j]));
            }
        }
    }
}