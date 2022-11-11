using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChessAI : MonoBehaviour
{
    [SerializeField] private float moveDelay = 1f;   
    private List<Moves> moves = new List<Moves>();
    public async void MakeMove(){
        GetMoves();
        await Task.Delay(Mathf.FloorToInt(moveDelay*1000));
        MakeRandomMove();
    }
    void MakeRandomMove(){
        var rnd = Random.Range(0, moves.Count);
        Debug.Log($"{ChessManager.Instance.PieceAtGrid(moves[rnd].from).name} move to {moves[rnd].to}.");
        ChessManager.Instance.MakeMove(ChessManager.Instance.PieceAtGrid(moves[rnd].from), moves[rnd].to);
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