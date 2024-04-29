using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileData
{
    public CONTEXT_ENVIRONMENT environment;
    public CONTEXT_MOMENT moment;
    public void RND_TileData(){
        environment = (CONTEXT_ENVIRONMENT)Random.Range(0, 7);
        moment = (CONTEXT_MOMENT)Random.Range(0, 6);
    }
}
