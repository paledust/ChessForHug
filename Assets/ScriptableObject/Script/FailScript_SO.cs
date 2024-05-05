using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/FailScript_SO")]
public class FailScript_SO : ScriptableObject
{
    [SerializeField] private List<FailData> failDatas;
    public TextAsset GetFailData(CONTEXT_MOMENT moment){
        return failDatas.Find(x=>x.moment == moment).textAsset;
    }
    [System.Serializable]
    public class FailData{
        public CONTEXT_MOMENT moment;
        public TextAsset textAsset;
    }
}
