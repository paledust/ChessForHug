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
using UnityEngine.InputSystem;

public class MoveSelector : MonoBehaviour {
	public GameObject moveLocationPrefab;
	public GameObject tileHightlightPrefab;
	public GameObject attackLocationPrefab;

	private GameObject tileHightlight;
	private GameObject movingPiece;
	private List<Vector2Int> moveLocations;
	private List<GameObject> locationHightlights;
	// Use this for initialization
	void Start () {
		this.enabled = false;
		tileHightlight = Instantiate(tileHightlightPrefab, Geometry.PointFromGrid(new Vector2Int(0,0)), Quaternion.identity, gameObject.transform);
		tileHightlight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			Vector3 point = hit.point;
			Vector2Int gridPoint = Geometry.GridFromPoint(point);

			tileHightlight.SetActive(true);
			tileHightlight.transform.position = Geometry.PointFromGrid(gridPoint);
			if(Mouse.current.leftButton.wasPressedThisFrame){
				if(!moveLocations.Contains(gridPoint)) return;

				var piece = movingPiece.GetComponent<Piece>();
				ChessManager.Instance.MakeMove(movingPiece, gridPoint);

				ExitState();
			}
		}
		else{
			tileHightlight.SetActive(false);
		}

		if(Mouse.current.rightButton.wasPressedThisFrame){
			DeselectCurrentPiece();
		}
	}

	public void EnterState(GameObject piece){
		EventHandler.Call_OnGrabPiece(piece);
		piece.SetActive(false);

		movingPiece = piece;
		this.enabled = true;

		moveLocations = ChessManager.Instance.MovesForPiece(movingPiece);
		locationHightlights = new List<GameObject>();

		foreach(Vector2Int loc in moveLocations){
			GameObject hightlight;
			if(ChessManager.Instance.PieceAtGrid(loc)){
				hightlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc),
					Quaternion.identity, gameObject.transform);
			}
			else{
				hightlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc),
					Quaternion.identity, gameObject.transform);
			}
			locationHightlights.Add(hightlight);
		}
	}
	public void ExitState(){
		DeselectCurrentPiece();
		ChessManager.Instance.EndTurn();
	}
	protected void DeselectCurrentPiece(){
		this.enabled = false;
		tileHightlight.SetActive(false);
		movingPiece.SetActive(true);
		ChessManager.Instance.DeselectPiece(movingPiece);
		movingPiece = null;
		TileSelector selector = GetComponent<TileSelector>();
		selector.EnterState();

		foreach(GameObject hightlight in locationHightlights){
			Destroy(hightlight);
		}
	}
}
