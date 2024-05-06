/*
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChessManager : Singleton<ChessManager>
{
[Header("Layout")]
    [SerializeField] private TextAsset boardLayout_File;
    [SerializeField] private LayoutInfo_SO layoutInfo;
[Header("Game Configuration")]
    [SerializeField] private CONTROL_TYPE white_Control;
    [SerializeField] private CONTROL_TYPE black_Control;
    public Board board;
[Header("Game")]
    [SerializeField] public int AgeUpAmount = 5;
    [SerializeField] private int AgeUpTurn = 6;
    [SerializeField] private GameObject chessPlayer;
    [SerializeField] private ChessAI chessAI;
[Header("Special Piece")]
    [SerializeField] private GameObject neutralPiece;
    [SerializeField] private GameObject tileInfoMarker;
[Header("Piece Died")]
    [SerializeField] private FailScript_SO failScript_SO;
    private GameObject[,] pieces;
    private TileData[,] tileDatas;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    private MOVE_TYPE lastTurnMoveType;
    private Stack<Moves> moveStack;

    [SerializeField, ShowOnly] private int TurnCount = 0;
    [SerializeField, ShowOnly] private int DeadCount = 0;
    public int originalPieceCount{get; private set;} = 0;

    public static readonly Dictionary<GENERATION, char> generationToPieceChar_Dict = new Dictionary<GENERATION, char>(){
        {GENERATION.BABY, 'P'}, {GENERATION.YOUTH, 'R'}, {GENERATION.TEEN, 'Q'},
        {GENERATION.GROWN, 'B'}, {GENERATION.MIDDLE, 'N'}, {GENERATION.OLD, 'K'},
        {GENERATION.DEAD, 'T'}
    };

    private const string DEFAULT_SETUP = "rnbkqbnr\n"+
                                         "pppppppp\n"+
                                         "xxxxxxxx\n"+
                                         "xxxxxxxx\n"+
                                         "xxxxxxxx\n"+
                                         "xxxxxxxx\n"+
                                         "PPPPPPPP\n"+
                                         "RNBKQBNR\n";
    protected override void Awake (){
        base.Awake();

        moveStack = new Stack<Moves>();
        
        pieces = new GameObject[8, 8];
        tileDatas = new TileData[8, 8];

        white = new Player(PLAYER_SIDE.WHITE, true, white_Control == CONTROL_TYPE.AI);
        black = new Player(PLAYER_SIDE.BLACK, false, black_Control == CONTROL_TYPE.AI);

        currentPlayer = white;
        otherPlayer = black;

    //Generate Board
        if(boardLayout_File == null)
            SetUp(DEFAULT_SETUP);
        else
            SetUp(boardLayout_File.text);
        
    //Generate Tile Data
        for(int i=0; i<8; i++){
            for(int j=0; j<8; j++){
                tileDatas[i,j] = new TileData();
                tileDatas[i,j].RND_TileData();
            }
        }
    }

    private void SetUp(string layoutText){
        string[] lines = layoutText.Split('\n');

        for(int i=0; i<lines.Length; i++){
            for(int j=0; j<lines[i].Length; j++){
                LayoutInfo info = layoutInfo.GetInfoByKey(lines[i][j]);
                if(info == null) continue;

                AddPiece(info.prefab, info.side == PLAYER_SIDE.WHITE?white:black, j, 7-i);
                originalPieceCount++;
            }
        }
    }
    public GameObject AddPiece(GameObject prefab, Player player, int col, int row){
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        var piece = pieceObject.GetComponent<Piece>();
        piece.InitAge();
    //We need to do this test, because we also want to add neutral piece on board, which doesn't belong to any player.
        if(player!=null) player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
        if(pieceObject.GetComponent<Pawn>()){
        //When place down pawn, check if the pawn is at the start position and set its moved value
            bool isMoved = false;
            if(player.side == PLAYER_SIDE.WHITE) isMoved = row != 1;
            else isMoved = row != 6;

            pieceObject.GetComponent<Pawn>().Moved = isMoved;
        }

        return pieceObject;
    }
    public bool AgeUpToPiece(Piece piece, PLAYER_SIDE playerSide, int AddAge){
        if(piece.type == PIECE_TYPE.TOMB) return false;

        int newAge = piece.personData.Age + AddAge;
        GENERATION originalGen = PersonData.GetGeneration(piece.personData.Age);
        GENERATION newGen = PersonData.GetGeneration(newAge);

        if(originalGen == newGen){
            piece.personData.Age = newAge;
            return false;
        }
        else{
            char pieceChar = generationToPieceChar_Dict[newGen];
            if(playerSide == PLAYER_SIDE.BLACK) pieceChar = char.ToLower(generationToPieceChar_Dict[newGen]);

            GameObject piecePrefab = layoutInfo.GetInfoByKey(pieceChar).prefab;
            Vector2Int gridPoint = Geometry.GridFromPoint(piece.transform.position);

            var player = playerSide==PLAYER_SIDE.WHITE?white:black;
            player.pieces.Remove(piece.gameObject);
            EventHandler.Call_UI_CleanDisplayer(piece.transform);
            Destroy(piece.gameObject);

            GameObject newPiece = AddPiece(piecePrefab, player, gridPoint.x, gridPoint.y);
            newPiece.GetComponentInChildren<Animation>()?.Play();
            newPiece.GetComponent<Piece>().personData.Age = newAge;

        //Update Some Info for dead people
            if(newGen == GENERATION.DEAD){
                var data = tileDatas[gridPoint.x, gridPoint.y].moment;
                newPiece.GetComponent<Tomb>().lastContent = $"享年{newPiece.GetComponent<Tomb>().personData.Age}岁\n" + ScriptParser.ParseFailScript(failScript_SO.GetFailData(data));
                DeadCount ++;

                if(MeetManager.Instance.HugCounter*2+DeadCount>=originalPieceCount){
                    EventHandler.Call_OnGameEnd(END_CONDITON.NO_PIECE);
                }
            }
            return true;
        }
    }
#region Helper Function
    public void SelectPieceAtGrid(Vector2Int gridPoint){
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
            board.SelectPiece(selectedPiece);
    }
    public Piece GetPieceAtGrid(Vector2Int gridPoint){
        var piece = pieces[gridPoint.x, gridPoint.y];
        if(piece==null) return null;
        else return piece.GetComponent<Piece>();
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
                    if(piece.type == PIECE_TYPE.NEUTRAL){value += (piece as Neutral).extraValue;}
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

    public TileData GetTileData(Vector2Int gridPoint){
        return tileDatas[gridPoint.x, gridPoint.y];
    }
    public void ExposedRNDTile(){
        int[] index = new int[64];
        for(int i=0;i<index.Length;i++){
            index[i] = i;
        }
        Service.Shuffle(ref index);

        int flag = 0;
        for(int i=0; i<64; i++){
            Vector2Int _gridpoint = new Vector2Int(index[i]%8,index[i]/8);
            if(pieces[_gridpoint.x, _gridpoint.y]==null && !tileDatas[_gridpoint.x,_gridpoint.y].IsExposed){
                var _marker = Instantiate(tileInfoMarker).GetComponent<TileInfoMarker>();
                _marker.transform.position = Geometry.PointFromGrid(_gridpoint);
                tileDatas[_gridpoint.x, _gridpoint.y].ExposeTileData(_marker);
                flag ++;
            }

            if(flag >= 3) break;
        }
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
#endregion
#region Turn
    public async void NextTurn(){
    //Add Turn Count and Age Up Pieces
        TurnCount ++;
        EventHandler.Call_UI_StepYear(1.0f/AgeUpTurn);

        if(TurnCount>=AgeUpTurn){
            TurnCount = 0;
            for(int x=0; x<8; x++){
                for(int y=0; y<8; y++){
                    if(pieces[x,y]!=null){
                        var piece = pieces[x,y].GetComponent<Piece>();
                        if(piece.type!=PIECE_TYPE.NEUTRAL){
                            if(AgeUpToPiece(piece,piece.playerSide, AgeUpAmount)){
                                await Task.Delay(250);
                            }
                        }
                    }
                }
            }
        }

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
            EventHandler.Call_OnPiecesHug(lastTurnMove.movePieces, lastTurnMove.takenPieces, lastTurnMove.to);
            
            if(!currentPlayer.IsAI) chessPlayer.SetActive(false);
            PauseChessGameBeforeEndTurn();
        }
        else{
            NextTurn();
        }
    }
#endregion
#region Moves
    public void MakeMoves(Moves move){
        moveStack.Push(move);
        move.Excute();
    }
    public void MakeMoves(GameObject piece, Vector2Int gridPoint){
        if(piece.GetComponent<Piece>().type == PIECE_TYPE.PAWN){
            MakeMoves(new PawnMoves(GridForPiece(piece), gridPoint));
        }
        else{
            MakeMoves(new Moves(GridForPiece(piece), gridPoint));
        }
    }
    public void UndoMoves(){
        moveStack.Pop().Undo();
    }
//Pieces Move rule
    public void MovePiece(GameObject piece, Vector2Int gridPoint){
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;

        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);

        lastTurnMoveType = MOVE_TYPE.MOVE;
    }
    public void HugPieces(GameObject takerPiece, Vector2Int gridPoint){
        Vector2Int startGridPoint = GridForPiece(takerPiece);
        pieces[startGridPoint.x, startGridPoint.y] = null;

        GameObject takenPiece = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(takenPiece);
        currentPlayer.pieces.Remove(takerPiece);
        otherPlayer.capturedPieces.Add(takerPiece);
        otherPlayer.pieces.Remove(takenPiece);

        pieces[gridPoint.x, gridPoint.y] = null;
        GameObject temp = AddPiece(neutralPiece, null, gridPoint.x, gridPoint.y);
        temp.GetComponent<Neutral>().extraValue = Service.PieceValueDict[takenPiece.GetComponent<Piece>().type];

        takerPiece.transform.position = Geometry.PointFromGrid(gridPoint);
        takerPiece.transform.position += new Vector3(0.3f*currentPlayer.forward, 0, 0.3f*otherPlayer.forward);
        takenPiece.transform.position += new Vector3(0.3f*otherPlayer.forward, 0, 0.3f*currentPlayer.forward);

        lastTurnMoveType = MOVE_TYPE.CAPTURE;
    }
    public void UndoHugMove(GameObject movePiece, GameObject takenPiece, Vector2Int from, Vector2Int to){
        pieces[to.x, to.y] = takenPiece;
        board.MovePiece(takenPiece, to);
        pieces[from.x, from.y] = movePiece;
        board.MovePiece(movePiece, from);
    }
#endregion
    void PauseChessGameBeforeEndTurn(){
        EventHandler.E_OnBackToChessGame += ResumeChessGameBeforeEndTurn;
    }
    void ResumeChessGameBeforeEndTurn(){
        EventHandler.E_OnBackToChessGame -= ResumeChessGameBeforeEndTurn;
        NextTurn();
    }

#if UNITY_EDITOR
    void OnDrawGizmos(){
        if(UnityEditor.EditorApplication.isPlaying){
            TileData data = new TileData();
            for(int i=0; i<8; i++){
                for(int j=0; j<8; j++){
                    data = tileDatas[i,j];
                    Handles.Label(Geometry.PointFromGrid(new Vector2Int(i,j)),$"Env:{data.environment}\n"+
                                                                              $"Mom:{data.moment}");
                }
            }
        }
    }
#endif
}
