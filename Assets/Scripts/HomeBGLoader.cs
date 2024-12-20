using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeBGLoader : MonoBehaviour
{
    [SerializeField] List<Sprite> homeBGs;
    [SerializeField] SpriteRenderer homeBGRenderer;

    void Start()
    {
        homeBGRenderer.sprite = homeBGs[Random.Range(0, homeBGs.Count)];
    }
   
}
