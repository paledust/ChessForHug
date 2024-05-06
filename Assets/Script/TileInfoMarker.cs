using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoMarker : MonoBehaviour
{
    [SerializeField] private Animator m_anime;
    private const string fadeName = "FadeOut";
    public void FadeOut()=>m_anime.SetTrigger(fadeName);
}
