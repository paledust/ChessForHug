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
 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelector : MonoBehaviour {
	public GameObject tileHighlightPrefeb;
	private GameObject tileHighlight;
	private Vector2Int hoverGridPoint;
	private ChessManager chessManager;
	
	void Start () {
		chessManager = ChessManager.Instance;
		hoverGridPoint = new Vector2Int(1,1)*-1;
		Vector2Int gridPoint = Geometry.GridPoint(0,0);
		Vector3 point = Geometry.PointFromGrid(gridPoint);
		tileHighlight = Instantiate(tileHighlightPrefeb, point, Quaternion.identity, gameObject.transform);
		tileHighlight.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			Vector3 point = hit.point;
			Vector2Int gridPoint = Geometry.GridFromPoint(point);
			if(hoverGridPoint != gridPoint){
				tileHighlight.SetActive(true);
				tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

				if(Geometry.ValidPoint(hoverGridPoint)){
					var lastPiece = chessManager.GetPieceAtGrid(hoverGridPoint);
					if(lastPiece!=null) {
						EventHandler.Call_UI_HideData(lastPiece.transform);
						EventHandler.Call_UI_ShowDescrip(string.Empty);
					}
				}

				var piece = chessManager.GetPieceAtGrid(gridPoint);
				if(piece!=null){
					if(piece.type == PIECE_TYPE.NEUTRAL){
						var tile = chessManager.GetTileData(gridPoint);
						EventHandler.Call_OnShowEnvironment(tile.environment);
						EventHandler.Call_UI_ShowDescrip(piece.GetComponent<Neutral>().content);
					}
					else {
						EventHandler.Call_UI_ShowData(piece.personData.Age, 220, piece.transform);
						EventHandler.Call_OnHideEnvironment();
					}
				}
				else{
					EventHandler.Call_OnHideEnvironment();
				}

				hoverGridPoint = gridPoint;
			}

			if(Mouse.current.leftButton.wasPressedThisFrame){
				GameObject seletectedPiece = chessManager.PieceAtGrid(gridPoint);
				if(chessManager.DoesPieceBelongToCurrentPlayer(seletectedPiece)){
					EventHandler.Call_UI_HideData(seletectedPiece.transform);
					chessManager.SelectPiece(seletectedPiece);
					ExitState(seletectedPiece);
				}
			}
		}
		else{
			tileHighlight.SetActive(false);
		}
	}
	public void EnterState(){
		enabled = true;
		EventHandler.Call_OnGrabPiece(null);
	}
	public void ExitState(GameObject movingPieces){
		this.enabled = false;
		tileHighlight.SetActive(false);
		MoveSelector move = GetComponent<MoveSelector>();
		move.EnterState(movingPieces);
	}
}
