using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Hand : MonoBehaviour{
    [SerializeField] private GameObject hand_Select;
    [SerializeField] private GameObject hand_Grab;
    [SerializeField] private Transform grabTrans;
    private Vector3 targetPos;
    private GameObject grabPiece;
    private bool updatingHandPos = true;
    void OnEnable(){
        EventHandler.E_OnGrabPiece  += GrabPiece;
    }
    void OnDisable(){
        EventHandler.E_OnGrabPiece  -= GrabPiece;
    }
    void Update(){
        if(!updatingHandPos) return;
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

		RaycastHit hit;

        if(Physics.Raycast(ray, out hit)){
            targetPos = hit.point;
            targetPos.y = 0;
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5);
    }
    void GrabPiece(GameObject piece){
        if(piece == null){
            hand_Select.SetActive(true);
            hand_Grab.SetActive(false);
            Destroy(grabPiece);
            updatingHandPos = true;
        }
        else{
            updatingHandPos = false;
            targetPos = piece.transform.position;
            targetPos.y = 0;
            transform.position = targetPos;

            grabPiece = GameObject.Instantiate(piece, grabTrans.position, grabTrans.rotation);

            hand_Select.SetActive(false);
            hand_Grab.SetActive(true);
        }
    }
    void UpdateHandRootTargetPos(Vector3 pos){
        targetPos = pos;
        targetPos.y = 0;
    }

}
