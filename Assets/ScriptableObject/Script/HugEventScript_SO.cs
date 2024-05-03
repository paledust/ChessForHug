using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/HugEventScript_SO")]
public class HugEventScript_SO : ScriptableObject
{
    public List<EnvironmentScriptData> environmentScriptDatas;
    public List<MomentScriptData> momentScriptDatas;
    public List<GenerationsScriptData> generationsScriptDatas;

    public HugData GetHugData(HugCondition condition, GENERATION huggerGen, GENERATION huggeegen){
        HugData personScript = ScriptParser.ParseGenerationScript(generationsScriptDatas.Find(x=>x.IsMatch(huggerGen, huggeegen)).genScript);
        string envScript = ScriptParser.ParseEnvironmentScript(environmentScriptDatas.Find(x=>x.environment == condition.env).envScript);
        string momScript = ScriptParser.ParseMomentScript(momentScriptDatas.Find(x=>x.moment == condition.moment).momScript);
        personScript.script = envScript + personScript + momScript;

        return personScript;
    }
}
[System.Serializable]
public class HugData{
    public CONTEXT_RELATION rel;
    public string script;
}
[System.Serializable]
public struct HugCondition{
    public CONTEXT_ENVIRONMENT env;
    public CONTEXT_MOMENT moment;
}
[System.Serializable]
public struct GenerationsScriptData{
    public GENERATION huggerGen;
    public GENERATION huggeeGen;
    public TextAsset genScript;
    public bool IsMatch(GENERATION _huggerGen, GENERATION _huggeeGen){
        return (huggerGen == _huggerGen && huggeeGen == _huggeeGen) || (huggerGen == _huggeeGen && huggeeGen == _huggerGen);
    }
}
[System.Serializable]
public struct EnvironmentScriptData{
    public CONTEXT_ENVIRONMENT environment;
    public TextAsset envScript;
}
[System.Serializable]
public struct MomentScriptData{
    public CONTEXT_MOMENT moment;
    public TextAsset momScript;
}