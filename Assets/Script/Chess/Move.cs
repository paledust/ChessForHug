using System.Collections;
using UnityEngine;

public class Moves{
    public Vector2Int from;
    public Vector2Int to;
    public GameObject takenPieces;
    public GameObject movePieces;
    public Moves(Vector2Int _from, Vector2Int _to){
        this.from = _from;
        this.to   = _to;

        movePieces = ChessManager.Instance.PieceAtGrid(_from);
        takenPieces= ChessManager.Instance.PieceAtGrid(_to);
    }
    public virtual void Undo(){
        if(takenPieces == null){
            ChessManager.Instance.MovePiece(movePieces, from);
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
    }
    public virtual void Excute(){
        if(takenPieces == null){
            ChessManager.Instance.MovePiece(movePieces, to);
        }
        else{
            ChessManager.Instance.HugPieces(movePieces, to);
        }
    }
}