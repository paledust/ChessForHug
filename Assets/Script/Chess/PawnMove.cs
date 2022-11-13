using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMoves: Moves{
    public bool firstMove;
    public bool promotion;
    public PawnMoves(Vector2Int _from, Vector2Int _to):base(_from, _to){
        if(!movePieces.GetComponent<Pawn>().Moved){
            firstMove = true;
        }
        else{
            firstMove = false;
        }

        if(takenPieces == null && to.y == ChessManager.Instance.currentPlayer.buttomLine){
            promotion = true;
        }
        else{
            promotion = false;
        }
    }
    public override void Undo()
    {
        if(takenPieces == null){
            ChessManager.Instance.MovePiece(movePieces, from);
            if(promotion) ChessManager.Instance.UndoPawnPromote(movePieces, ChessManager.Instance.currentPlayer.side);
        }
        else{
            GameObject.Destroy(ChessManager.Instance.PieceAtGrid(to));

            ChessManager.Instance.currentPlayer.pieces.Add(movePieces);
            ChessManager.Instance.currentPlayer.capturedPieces.Remove(takenPieces);

            ChessManager.Instance.otherPlayer.pieces.Add(takenPieces);
            ChessManager.Instance.otherPlayer.capturedPieces.Remove(movePieces);

            takenPieces.SetActive(true);
            movePieces.SetActive(true);

            ChessManager.Instance.UndoHugMove(movePieces, takenPieces, from, to);
        }

        if(firstMove){movePieces.GetComponent<Pawn>().Moved = false;}
    }
    public override void Excute(){
        if(takenPieces == null){
            ChessManager.Instance.MovePiece(movePieces, to);
            if(promotion) ChessManager.Instance.PromotePawn(movePieces, ChessManager.Instance.currentPlayer.side);
        }
        else{
            ChessManager.Instance.HugPieces(movePieces, to);
        }
        if(firstMove){movePieces.GetComponent<Pawn>().Moved = true;}
    }
}
