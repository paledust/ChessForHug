using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/EnvSprites_SO")]
public class EnvSprites_SO : ScriptableObject
{
    [SerializeField] private List<EnvSpritesData> envSpritesData;
    public Sprite[] GetSpritesByEnvironment(CONTEXT_ENVIRONMENT env){
        return envSpritesData.Find(x=>x.environment == env).sprites;
    }
}
[System.Serializable]
public struct EnvSpritesData{
    public CONTEXT_ENVIRONMENT environment;
    public Sprite[] sprites;
}
