using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LoadBG : MonoBehaviour
{
    [SerializeField] List<Sprite> backGrounds = new List<Sprite>();
    [SerializeField] GameObject backGround;
    [SerializeField] Background bgList;
    [SerializeField] int  LevelPoint= 0;
    [SerializeField] int Level;

    void Start()
    {
        Level = GameManager.Instance.Level;
        int backGroundIndex = (Level + 1) / 5;
        LevelPoint = backGroundIndex * 5;
        backGrounds = bgList.backGrounds;
        LoadBGround(Level);
    }

    private void LoadBGround(int level)
    {
        if (level >= LevelPoint)
        {
            int backGroundIndex = (level + 1) / 5;
            LevelPoint = backGroundIndex * 5;
            backGround.GetComponent<SpriteRenderer>().sprite = backGrounds[LevelPoint];
        }
    }
}
