using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Extension
public static class ExtensionMethods{
    public static float GetRndValueInVector2Range(this Vector2 range){return Random.Range(range.x, range.y);}
    public static int GetRndValueInVector2Range(this Vector2Int range){return Random.Range(range.x, range.y);}
}
#endregion
