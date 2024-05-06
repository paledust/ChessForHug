using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiator_Title : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(coroutineLoadNext());
    }
    IEnumerator coroutineLoadNext(){
        yield return new WaitForSeconds(3);
        GameManager.Instance.SwitchingScene("Chess", false);
    }
}
