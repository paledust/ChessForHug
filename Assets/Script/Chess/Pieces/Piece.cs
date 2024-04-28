﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PersonData{
    public int Age;
    public const int BABY_AGE = 5;
    public const int YOUTH_AGE = 12;
    public const int TEEN_AGE = 18;
    public const int GROWN_AGE = 40;
    public const int MIDDLE_AGE = 65;
    public const int OLD_AGE = 100;
    public bool IsBaby{get{return Age<=BABY_AGE;}}
    public bool IsYouth{get{return Age<=YOUTH_AGE && Age>BABY_AGE;}}
    public bool IsTeen{get{return Age<=TEEN_AGE && Age>YOUTH_AGE;}}
    public bool IsGrown{get{return Age<=GROWN_AGE && Age>TEEN_AGE;}}
    public bool IsMiddle{get{return Age<=MIDDLE_AGE && Age>GROWN_AGE;}}
    public bool IsOld{get{return Age<=OLD_AGE && Age>MIDDLE_AGE;}}
    public bool IsMiracle{get{return Age>OLD_AGE;}}
}
public abstract class Piece : MonoBehaviour
{
    public PIECE_TYPE type;
    public PersonData personData;

    protected static Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0), 
        new Vector2Int(0, -1), new Vector2Int(-1, 0)};
    protected static Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1), 
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
}