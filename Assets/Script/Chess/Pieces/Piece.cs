using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PersonData{
    public int Age;
    public const int BABY_AGE = 6;
    public const int YOUTH_AGE = 12;
    public const int TEEN_AGE = 20;
    public const int GROWN_AGE = 40;
    public const int MIDDLE_AGE = 68;
    public const int DEATH_AGE = 95;
    public bool IsBaby{get{return Age<=BABY_AGE;}}
    public bool IsYouth{get{return Age<=YOUTH_AGE && Age>BABY_AGE;}}
    public bool IsTeen{get{return Age<=TEEN_AGE && Age>YOUTH_AGE;}}
    public bool IsGrown{get{return Age<=GROWN_AGE && Age>TEEN_AGE;}}
    public bool IsMiddle{get{return Age<=MIDDLE_AGE && Age>GROWN_AGE;}}
    public bool IsOld{get{return Age>MIDDLE_AGE;}}
    public const float Age_Start_Percentage = 0.23f;
    public const float Age_RND_Percentage = 0.4f;

    public static GENERATION GetGeneration(int age){
        if(age<=BABY_AGE) return GENERATION.BABY;
        else if(age<=YOUTH_AGE) return GENERATION.YOUTH;
        else if(age<=TEEN_AGE) return GENERATION.TEEN;
        else if(age<=GROWN_AGE) return GENERATION.GROWN;
        else if(age<=MIDDLE_AGE) return GENERATION.MIDDLE;
        return GENERATION.OLD;
    }
    public static int InitAge(int age){
        if(age<=BABY_AGE) return Mathf.FloorToInt(BABY_AGE*Age_Start_Percentage) + Random.Range(0, Mathf.FloorToInt(BABY_AGE*Age_RND_Percentage));
        else if(age<=YOUTH_AGE) return BABY_AGE+Mathf.FloorToInt(6*Age_Start_Percentage)+Random.Range(0, Mathf.FloorToInt(6*Age_RND_Percentage));
        else if(age<=TEEN_AGE) return YOUTH_AGE+Mathf.FloorToInt(8*Age_Start_Percentage)+Random.Range(0, Mathf.FloorToInt(8*Age_RND_Percentage));
        else if(age<=GROWN_AGE) return TEEN_AGE+Mathf.FloorToInt(20*Age_Start_Percentage)+Random.Range(0, Mathf.FloorToInt(20*Age_RND_Percentage));
        else if(age<=MIDDLE_AGE) return GROWN_AGE+Mathf.FloorToInt(28*Age_Start_Percentage)+Random.Range(0, Mathf.FloorToInt(28*Age_RND_Percentage));
        else return MIDDLE_AGE+Mathf.FloorToInt(32*Age_Start_Percentage)+Random.Range(0, Mathf.FloorToInt(32*Age_RND_Percentage));
    }
}
public abstract class Piece : MonoBehaviour
{
    public PLAYER_SIDE playerSide;
    public PIECE_TYPE type;
    public PersonData personData;
    public void InitAge()=>personData.Age = PersonData.InitAge(personData.Age);
    protected static Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0), 
        new Vector2Int(0, -1), new Vector2Int(-1, 0)};
    protected static Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1), 
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
}