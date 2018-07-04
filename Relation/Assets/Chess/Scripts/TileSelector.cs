using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour {
	public GameObject tileHighlightPrefeb;
	private GameObject tileHighlight;

	// Use this for initialization
	void Start () {
		Vector2Int gridPoint = Geometry.GridPoint(0,0);
		Vector3 point = Geometry.PointFromGrid(gridPoint);
		tileHighlight = Instantiate(tileHighlightPrefeb, point, Quaternion.identity, gameObject.transform);
		tileHighlight.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			Vector3 point = hit.point;
			Vector2Int gridPoint = Geometry.GridFromPoint(point);

			tileHighlight.SetActive(true);
			tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

			if(Input.GetMouseButtonDown(0)){
				GameObject seletectedPiece = 
					GameManager.instance.PieceAtGrid(gridPoint);
				if(GameManager.instance.DoesPieceBelongToCurrentPlayer(seletectedPiece)){
					GameManager.instance.SelectPiece(seletectedPiece);
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
	}
	public void ExitState(GameObject movingPieces){
		this.enabled = false;
		tileHighlight.SetActive(false);
		MoveSelector move = GetComponent<MoveSelector>();
		move.EnterState(movingPieces);
	}
}
