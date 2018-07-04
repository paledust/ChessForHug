using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			Vector3 point = hit.point;
			Vector2Int gridPoint = Geometry.GridFromPoint(point);

			tileHightlight.SetActive(true);
			tileHightlight.transform.position = Geometry.PointFromGrid(gridPoint);
			if(Input.GetMouseButtonDown(0)){
				if(!moveLocations.Contains(gridPoint)) return;

				if(GameManager.instance.PieceAtGrid(gridPoint) == null){
					GameManager.instance.Move(movingPiece, gridPoint);
				}
				else{
					GameManager.instance.CapturePieceAt(gridPoint);
					GameManager.instance.Move(movingPiece, gridPoint);
				}
				ExitState();
			}
		}
		else{
			tileHightlight.SetActive(false);
		}
	}

	public void EnterState(GameObject piece){
		movingPiece = piece;
		this.enabled = true;

		moveLocations = GameManager.instance.MovesForPiece(movingPiece);
		locationHightlights = new List<GameObject>();

		foreach(Vector2Int loc in moveLocations){
			GameObject hightlight;
			if(GameManager.instance.PieceAtGrid(loc)){
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
		this.enabled = false;
		tileHightlight.SetActive(false);
		GameManager.instance.DeselectPiece(movingPiece);
		movingPiece = null;
		TileSelector selector = GetComponent<TileSelector>();
		GameManager.instance.NextPlayer();
		selector.EnterState();

		foreach(GameObject hightlight in locationHightlights){
			Destroy(hightlight);
		}
	}

}
