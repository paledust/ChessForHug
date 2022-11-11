﻿/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using UnityEngine;
public class ChessManager : Singleton<ChessManager>
{
[Header("Board Configuration")]
    [SerializeField] private CONTROL_TYPE white_Control;
    [SerializeField] private CONTROL_TYPE black_Control;
    public Board board;
[Header("Game")]
    [SerializeField] private GameObject chessPlayer;
    [SerializeField] private ChessAI chessAI;
[Header("Special Piece")]
    public GameObject neutralPiece;
[Header("Pieces")]
    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    private GameObject[,] pieces;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    private MOVE_TYPE lastTurnMoveType;
    private Stack<Moves> moveStack;
    protected override void Awake (){
        base.Awake();

        moveStack = new Stack<Moves>();
        
        pieces = new GameObject[8, 8];

        white = new Player(PLAYER_SIDE.WHITE, true, white_Control == CONTROL_TYPE.AI);
        black = new Player(PLAYER_SIDE.BLACK, false, black_Control == CONTROL_TYPE.AI);

        currentPlayer = white;
        otherPlayer = black;

        InitialSetup();
    }

    private void InitialSetup(){
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 1, 0);
        AddPiece(whiteBishop, white, 2, 0);
        AddPiece(whiteQueen, white, 3, 0);
        AddPiece(whiteKing, white, 4, 0);
        AddPiece(whiteBishop, white, 5, 0);
        AddPiece(whiteKnight, white, 6, 0);
        AddPiece(whiteRook, white, 7, 0);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(whitePawn, white, i, 1);
        }

        AddPiece(blackRook, black, 0, 7);
        AddPiece(blackKnight, black, 1, 7);
        AddPiece(blackBishop, black, 2, 7);
        AddPiece(blackQueen, black, 3, 7);
        AddPiece(blackKing, black, 4, 7);
        AddPiece(blackBishop, black, 5, 7);
        AddPiece(blackKnight, black, 6, 7);
        AddPiece(blackRook, black, 7, 7);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(blackPawn, black, i, 6);
        }
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row){
        GameObject pieceObject = board.AddPiece(prefab, col, row);
    //We need to do this test, because we also want to add neutral piece on board, which doesn't belong to any player.
        if(player!=null) player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint){
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }

    public void SelectPiece(GameObject piece){
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece){
        board.DeselectPiece(piece);
    }
    public int EvaluateBoard(Player player){
        int value = 0;
        Piece piece = null;
        for(int x=0; x<8; x++){
            for(int y=0; y<8; y++){
                piece = PieceAtGrid(new Vector2Int(x, y))?.GetComponent<Piece>();
                if(piece != null){
                    value += Service.PieceValueDict[piece.type] * (player.pieces.Contains(piece.gameObject)?1:-1);
                }
            }
        }
        return value;
    }
    public GameObject PieceAtGrid(Vector2Int gridPoint){
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece){
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint){
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece){
        return currentPlayer.pieces.Contains(piece);
    }

    public void CheckLegalMove(ref List<Vector2Int> moveLocations){
        moveLocations.RemoveAll(tile => tile.x < 0 || tile.x > 7 || tile.y < 0 || tile.y > 7);
        moveLocations.RemoveAll(tile => FriendlyPieceAt(tile));
    }
    public List<Vector2Int> MovesForPiece(GameObject pieceObject){
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);

        var locations = piece.MoveLocations(gridPoint);
        CheckLegalMove(ref locations);

        return locations;
    }
    public async void NextPlayer(){
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;

        if(currentPlayer.IsAI){
            chessPlayer.SetActive(false);
            await chessAI.MakeMove();
            EndTurn();
        }
        else{
            chessPlayer.SetActive(true);
        }
    }
    public void EndTurn(){
        if(lastTurnMoveType == MOVE_TYPE.CAPTURE){
            Moves lastTurnMove = moveStack.Peek();
            EventHandler.Call_OnPiecesHuge(lastTurnMove.movePieces, lastTurnMove.takenPieces, lastTurnMove.to);
        }
        else{
			// EventHandler.Call_OnMovePieceOnly(piece, ChessManager.Instance.currentPlayer.side);
        }
        NextPlayer();
    }
    public void MakeMove(Moves move){
        moveStack.Push(move);
        move.Excute();
    }
    public void MakeMove(GameObject piece, Vector2Int gridPoint){
        if(piece.GetComponent<Piece>().type == PIECE_TYPE.PAWN){
            MakeMove(new PawnMoves(GridForPiece(piece), gridPoint));
        }
        else{
            MakeMove(new Moves(GridForPiece(piece), gridPoint));
        }
    }
    public void UndoMove(){
        moveStack.Pop().Undo();
    }
//Pieces Move rule
    public void Move(GameObject piece, Vector2Int gridPoint){
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;

        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);

        lastTurnMoveType = MOVE_TYPE.MOVE;
    }
    public void UndoHugMove(GameObject movePiece, GameObject takenPiece, Vector2Int from, Vector2Int to){
        pieces[to.x, to.y] = takenPiece;
        board.MovePiece(takenPiece, to);
        pieces[from.x, from.y] = movePiece;
        board.MovePiece(movePiece, from);
    }
    public void HugPieces(GameObject piece, Vector2Int gridPoint){
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;

        GameObject otherPiece = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(otherPiece);
        currentPlayer.pieces.Remove(piece);
        otherPlayer.capturedPieces.Add(piece);
        otherPlayer.pieces.Remove(otherPiece);

        pieces[gridPoint.x, gridPoint.y] = null;
        AddPiece(neutralPiece, null, gridPoint.x, gridPoint.y);

        piece.transform.position = Geometry.PointFromGrid(gridPoint);
        piece.transform.position += new Vector3(0.3f*currentPlayer.forward, 0, 0.3f*otherPlayer.forward);
        otherPiece.transform.position += new Vector3(0.3f*otherPlayer.forward, 0, 0.3f*currentPlayer.forward);

        lastTurnMoveType = MOVE_TYPE.CAPTURE;
    }
    public void PromotePawn(GameObject pawn){
        Destroy(pawn.GetComponent<Pawn>());
        pawn.GetComponentInChildren<MeshFilter>().mesh = whiteQueen.GetComponentInChildren<MeshFilter>().mesh;
        pawn.AddComponent<Queen>().type = PIECE_TYPE.QUEEN;
    }
    public void UndoPawnPromote(GameObject pawn){
        Destroy(pawn.GetComponent<Queen>());
        pawn.GetComponentInChildren<MeshFilter>().mesh = whitePawn.GetComponentInChildren<MeshFilter>().mesh;
        pawn.AddComponent<Pawn>().type = PIECE_TYPE.PAWN;        
    }
}
