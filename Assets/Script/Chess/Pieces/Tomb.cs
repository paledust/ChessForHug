using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomb : Piece
{
    public string lastContent;
    public CONTEXT_MOMENT lastMoment;
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint){return null;}
}
