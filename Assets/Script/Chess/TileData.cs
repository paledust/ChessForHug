using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileData
{
    public CONTEXT_ENVIRONMENT environment;
    public CONTEXT_MOMENT moment;
    public bool IsExposed;
    
    private static int[] env_rnd = {0,1,2,3};
    private static int[] mom_rnd = {0,1,2,3};
    private static int env_count = 0;
    private static int mom_count = 0;
    public void RND_TileData(){
        IsExposed = false;
        environment = (CONTEXT_ENVIRONMENT)env_rnd[env_count];
        moment = (CONTEXT_MOMENT)mom_rnd[mom_count];
        env_count ++;
        mom_count ++;

        if(env_count>=env_rnd.Length){
            env_count = 0;
            Service.Shuffle(ref env_rnd);
        }
        if(mom_count>=mom_rnd.Length){
            mom_count = 0;
            Service.Shuffle(ref mom_rnd);
        }
    }
}