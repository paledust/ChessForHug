using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour{
    [SerializeField] private GameObject hand_Select;
    [SerializeField] private GameObject hand_Grab;
    [SerializeField] private Transform grabTrans;
    private Vector3 targetPos;
    private GameObject grabPiece;
    void OnEnable(){
        EventHandler.E_OnSelectGrid += UpdateHandRootTargetPos;
        EventHandler.E_OnGrabPiece  += GrabPiece;
    }
    void OnDisable(){
        EventHandler.E_OnSelectGrid -= UpdateHandRootTargetPos;
        EventHandler.E_OnGrabPiece  -= GrabPiece;
    }
    void GrabPiece(GameObject piece){
        if(piece == null){
            hand_Select.SetActive(true);
            hand_Grab.SetActive(false);
            Destroy(grabPiece);
        }
        else{
            targetPos = piece.transform.position;
            targetPos.y = 0;
            transform.position = transform.position;

            grabPiece = GameObject.Instantiate(piece, grabTrans.position, grabTrans.rotation);

            hand_Select.SetActive(false);
            hand_Grab.SetActive(true);
        }
    }
    void UpdateHandRootTargetPos(Vector3 pos){
        targetPos = pos;
        targetPos.y = 0;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5);
    }

}
