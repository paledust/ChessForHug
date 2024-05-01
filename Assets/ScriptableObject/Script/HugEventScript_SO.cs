using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/HugEventScript_SO")]
public class HugEventScript_SO : ScriptableObject
{
    public HugData defaultHug;
    public List<HugGroupInfo> hugGroupList;
    public HugData GetHugData(HugCondition condition, GENERATION huggerGen, GENERATION huggeegen){
        var groupInfo = hugGroupList.Find(x=>x.hugCondition.Match(condition));
        if(groupInfo == null) return defaultHug;

        var data = groupInfo.hugDatas.Find(x=>(x.Hug1Gen == huggerGen && x.Hug2Gen==huggeegen) || (x.Hug1Gen==huggeegen && x.Hug2Gen==huggerGen));
        if(data == null) return defaultHug;
        else return data;
    }

    [System.Serializable]
    public class HugGroupInfo{
        public HugCondition hugCondition;
        public List<HugData> hugDatas;
    }
}
[System.Serializable]
public class HugData{
    public GENERATION Hug1Gen;
    public GENERATION Hug2Gen;
    public CONTEXT_RELATION rel;
    public TextAsset script;
}
[System.Serializable]
public struct HugCondition{
    public CONTEXT_ENVIRONMENT env;
    public CONTEXT_MOMENT moment;
    public bool Match(HugCondition condition){
        return condition.env == this.env && condition.moment == this.moment;
    }
}