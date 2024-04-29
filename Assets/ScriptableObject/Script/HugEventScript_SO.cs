using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/HugEventScript_SO")]
public class HugEventScript_SO : ScriptableObject
{
    public HugData defaultHug;
    public List<HugGroupInfo> hugGroupList;
    public HugData GetHugData(HugCondition condition, int huggerAge, int huggeeAge){
        var groupInfo = hugGroupList.Find(x=>x.hugCondition.Match(condition));
        if(groupInfo == null) return defaultHug;

        var data = groupInfo.hugDatas.Find(x=>(x.Hug1Age == huggerAge && x.Hug2Age==huggeeAge) || (x.Hug1Age==huggeeAge && x.Hug2Age==huggerAge));
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
    public int Hug1Age;
    public int Hug2Age;
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
        // flag = flag && ((condition.Hug1Age == this.Hug1Age && condition.Hug1Age == this.Hug2Age) 
        //             || (condition.Hug1Age == this.Hug2Age && condition.Hug2Age == this.Hug1Age));
