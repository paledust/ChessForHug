using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Service
{
#region Parameter
    public const string debugTag = "Debug";
    public static LayerMask InteractableLayer = 1 << LayerMask.NameToLayer("Interactable"); //TO Do: Name whatever the interactable layer should be
    public static Dictionary<PIECE_TYPE, int> PieceValueDict = new Dictionary<PIECE_TYPE, int>(){
        {PIECE_TYPE.PAWN, 100},
        {PIECE_TYPE.KNIGHT, 350},
        {PIECE_TYPE.BISHOP, 350},
        {PIECE_TYPE.ROOK, 525},
        {PIECE_TYPE.QUEEN, 1000},
        {PIECE_TYPE.KING, 10000},
        {PIECE_TYPE.NEUTRAL, 0}
    };
#endregion

#region HelpFunction
    /// <summary>
    /// Return a list of all active and inactive objects of T type in loaded scenes.
    /// </summary>
    /// <typeparam name="T">Object Type</typeparam>
    /// <returns></returns>
    public static T[] FindComponentsOfTypeIncludingDisable<T>(){
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
        var MatchObjects = new List<T> ();

        for(int i=0; i<sceneCount; i++){
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);
            
            var RootObjects = scene.GetRootGameObjects ();

            foreach (var obj in RootObjects) {
                var Matches = obj.GetComponentsInChildren<T> (true);
                MatchObjects.AddRange (Matches);
            }
        }

        return MatchObjects.ToArray ();
    }
    public static void Shuffle<T>(ref T[] elements){
        var rnd = new System.Random();
        for(int i=0; i<elements.Length; i++){
            int index = rnd.Next(i+1);
            T tmp = elements[i];
            elements[i] = elements[index];
            elements[index] = tmp;
        }
    }
#endregion
}