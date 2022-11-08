using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Please make sure "Main" is excuted before every custom script but after "GameManager"
public class Main : MonoBehaviour
{
    protected virtual void Awake(){
        GameManager.mainCam = Camera.main;
    }
}
