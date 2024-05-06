using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AgeFollower : MonoBehaviour
{
    [SerializeField] private Transform followTrans;
    [SerializeField] private float offset;

    private RectTransform rectTransform;
    private Camera mainCam;

    void Start(){
        mainCam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        Vector3 screenPos = mainCam.WorldToScreenPoint(followTrans.position + Vector3.up*offset);
        rectTransform.position = screenPos;
    }
}
