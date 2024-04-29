using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/LayoutInfo")]
public class LayoutInfo_SO : ScriptableObject
{
    public char emptyKey = 'x';
    public List<LayoutInfo> layoutInfos;
    public LayoutInfo GetInfoByKey(char key){
        if(key == emptyKey) return null;
        return layoutInfos.Find(x=>x.key == key);
    }
}
[System.Serializable]
public class LayoutInfo{
    public char key;
    public GameObject prefab;
    public PLAYER_SIDE side;
}
