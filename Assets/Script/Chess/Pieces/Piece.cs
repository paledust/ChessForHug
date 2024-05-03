using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PersonData{
    public int Age;
    public const int BABY_AGE = 5;
    public const int YOUTH_AGE = 12;
    public const int TEEN_AGE = 20;
    public const int GROWN_AGE = 40;
    public const int MIDDLE_AGE = 68;
    public bool IsBaby{get{return Age<=BABY_AGE;}}
    public bool IsYouth{get{return Age<=YOUTH_AGE && Age>BABY_AGE;}}
    public bool IsTeen{get{return Age<=TEEN_AGE && Age>YOUTH_AGE;}}
    public bool IsGrown{get{return Age<=GROWN_AGE && Age>TEEN_AGE;}}
    public bool IsMiddle{get{return Age<=MIDDLE_AGE && Age>GROWN_AGE;}}
    public bool IsOld{get{return Age>MIDDLE_AGE;}}
    public static GENERATION GetGeneration(int age){
        if(age<=BABY_AGE) return GENERATION.BABY;
        else if(age<=YOUTH_AGE) return GENERATION.YOUTH;
        else if(age<=TEEN_AGE) return GENERATION.TEEN;
        else if(age<=GROWN_AGE) return GENERATION.GROWN;
        else if(age<=MIDDLE_AGE) return GENERATION.MIDDLE;
        return GENERATION.OLD;
    }
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