using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral : Piece
{
    public int extraValue = 0;
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint){return null;}
}
